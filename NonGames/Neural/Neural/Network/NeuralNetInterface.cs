
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Neural.Network;

/// <summary>
/// Threaded façade around NeuralNetwork.
/// - Starts a dedicated simulation thread (1 kHz fixed dt).
/// - Accepts input characters via a queue.
/// - Exposes emitted strings via a queue and an event.
/// - Lets you provide reward/approval asynchronously (thread-safe).
/// - Schedules 2-burst token injection (0 ms and +3 ms).
/// </summary>
public sealed class NeuralNetInterface : IDisposable {
	// ---- user-facing events & queues
	public event Action<string>? Emitted;                // when the net "Commit"s a string
	public event Action<NetworkMetrics>? MetricsUpdated; // once per metrics window

	private readonly ConcurrentQueue<char> _inputQueue = new();
	private readonly ConcurrentQueue<string> _emissionQueue = new();
	private readonly ConcurrentQueue<float> _pendingRewards = new();
	private readonly ConcurrentQueue<NetworkMetrics> _metricsQueue = new();

	// second-burst schedule (char, sim-time 'when' to deliver)
	private readonly ConcurrentQueue<(char c, float when)> _pendingBursts = new();

	// emission counter for current metrics window
	private int _emissionsInWindow = 0;

	// ---- network & thread state
	private readonly NeuralNetwork _net;
	private readonly Thread _thread;
	private readonly CancellationTokenSource _cts = new();
	private readonly float _dt = 0.001f; // 1 ms sim step

	// ---- configuration
	public string Alphabet { get; }
	public float FirstBurstAmplitude { get; set; } = 0.8f;
	public float SecondBurstDelaySec { get; set; } = 0.003f; // +3 ms
	public float SecondBurstAmplitude { get; set; } = 0.8f;

	// metrics sampling interval (seconds)
	public float MetricsIntervalSec { get; set; } = 0.01f;

	// ---- input feeding mode & schedule
	public InputFeedingMode FeedingMode { get; private set; } = InputFeedingMode.Forced;
	public float InputRateTokensPerSec { get; set; } = 50f;   // e.g., 50 tokens/s => one every 20 ms
	private float _nextInputTimeSec = 0f;                     // next scheduled injection (sim-time)

	public NeuralNetInterface(
		string alphabet = "abcdefghijklmnopqrstuvwxyz .,!?",
		(int input, int resExc, int resInh, int pred, int action)? sizes = null,
		int seed = 12345 ) {
		Alphabet = alphabet;

		var s = sizes ?? (input: 2048, resExc: 4096, resInh: 1024, pred: 2048, action: 512);
		_net = new NeuralNetwork( Alphabet.ToCharArray(), s.input, s.resExc, s.resInh, s.pred, s.action );
		_net.SDR_K_Input = 300;
		_net.FanoutInput = 200;
		_net.FaninOut = 200;
		
		_net.WExcRec = 0.50f;  // reservoir recurrent
		_net.WInFeed = 3.00f;  // input -> reservoir
		_net.WOut = 2.00f;  // reservoir -> (prediction, action)
		
		//_net.InhibitoryWeightScale = 1f / 5f;  // because NeuronType.Inhibitory = -5
		_net.BackgroundRateHz = 6f;
		_net.BackgroundAmp = 0.10f;
		_net.WireAll( seed );

		// Hook adapters (OnDemand mode uses OnReadNext; Forced sets it to null)
		SetFeedingMode( InputFeedingMode.Forced );

		_net.OnEmitString = OnEmitFromNetwork;

		// Start simulation thread
		_thread = new Thread( RunLoop ) {
			IsBackground = true,
			Name = "AutonomousSNN-Thread",
			Priority = ThreadPriority.AboveNormal
		};
		_thread.Start();
	}

	// ---------------------
	// Public API
	// ---------------------

	/// <summary>Force or allow on-demand input. Forced is push-sensing (recommended).</summary>
	public void SetFeedingMode( InputFeedingMode mode ) {
		FeedingMode = mode;
		// Only used by the sim thread; setting here is fine—delegate assignment is atomic.
		_net.OnReadNext = (mode == InputFeedingMode.OnDemand) ? OnReadNextRequested : null;
		if (mode == InputFeedingMode.Forced) {
			// schedule the first forced injection soon (by sim-time)
			_nextInputTimeSec = _net.TimeSec; // inject as soon as the loop sees it
		}
	}

	/// <summary>Set the forced input rate (tokens/second). Default 50 t/s.</summary>
	public void SetInputRate( float tokensPerSecond ) {
		InputRateTokensPerSec = Math.Max( 1e-3f, tokensPerSecond );
	}

	/// <summary>Enqueue characters to be provided to the network (forced or on-demand).</summary>
	public void EnqueueInput( string text ) {
		foreach (var c in text)
			_inputQueue.Enqueue( c );
	}

	/// <summary>Try to retrieve the next emitted string (non-blocking).</summary>
	public bool TryDequeueEmission( out string emitted ) => _emissionQueue.TryDequeue( out emitted );

	/// <summary>Try to retrieve the latest metrics snapshot (non-blocking).</summary>
	public bool TryDequeueMetrics( out NetworkMetrics m ) => _metricsQueue.TryDequeue( out m );

	/// <summary>Provide scalar reward/approval for the most recent emission (thread-safe).</summary>
	public void ProvideReward( float reward ) => _pendingRewards.Enqueue( reward );

	/// <summary>Simulation time (seconds).</summary>
	public float SimulationTimeSec => _net.TimeSec;

	public string BufferSnapshot => _net.BufferSnapshot;

	public void Dispose() {
		_cts.Cancel();
		try { if (Thread.CurrentThread != _thread) _thread.Join(); } catch { /* ignored on shutdown */ }
		_cts.Dispose();
	}

	// ---------------------
	// Internal plumbing
	// ---------------------

	// Only used in OnDemand mode
	private void OnReadNextRequested() {
		if (_inputQueue.TryDequeue( out var ch )) {
			_net.InjectToken( ch, bursts: 1, targetDeltaPerBurst: FirstBurstAmplitude );
			_pendingBursts.Enqueue( (ch, _net.TimeSec + SecondBurstDelaySec) );
		}
	}

	private void OnEmitFromNetwork( string s ) {
		_emissionQueue.Enqueue( s );
		Interlocked.Increment( ref _emissionsInWindow );
		Emitted?.Invoke( s );
	}

	private void RunLoop() {
		var sw = Stopwatch.StartNew();
		float lastWall = 0f;
		float nextMetricsTime = 0f; // measured in simulation time

		try {
			while (!_cts.IsCancellationRequested) {
				float nowWall = (float) sw.Elapsed.TotalSeconds;

				if (nowWall - lastWall >= _dt) {
					// ---------------- Forced input feeder ----------------
					if (FeedingMode == InputFeedingMode.Forced && _inputQueue.Count > 0 && InputRateTokensPerSec > 0f) {
						float period = 1f / InputRateTokensPerSec;

						// Catch up if sim is behind: inject as many as fit up to current sim-time
						while (_net.TimeSec >= _nextInputTimeSec && _inputQueue.TryDequeue( out var ch )) {
							_net.InjectToken( ch, bursts: 1, targetDeltaPerBurst: FirstBurstAmplitude );
							_pendingBursts.Enqueue( (ch, _net.TimeSec + SecondBurstDelaySec) );

							_nextInputTimeSec = _net.TimeSec + period;
						}
					}

					// Second-burst injections due by sim-time
					while (_pendingBursts.TryPeek( out var ev ) && _net.TimeSec >= ev.when) {
						_pendingBursts.TryDequeue( out var fire );
						_net.InjectToken( fire.c, bursts: 1, targetDeltaPerBurst: SecondBurstAmplitude );
					}

					// Apply any queued rewards (from host approvals)
					while (_pendingRewards.TryDequeue( out var r ))
						_net.ApplyOutcome( r );

					// Step simulation
					_net.Tick( _dt );
					lastWall += _dt;

					// Metrics window
					if (_net.TimeSec >= nextMetricsTime) {
						int emissions = Interlocked.Exchange( ref _emissionsInWindow, 0 );
						int pendingInput = _inputQueue.Count;

						var snap = _net.CollectMetricsAndReset(
							windowSec: Math.Max( 1e-3f, MetricsIntervalSec ),
							emissionsInWindow: emissions,
							pendingInputCount: pendingInput
						);

						_metricsQueue.Enqueue( snap );
						MetricsUpdated?.Invoke( snap );

						nextMetricsTime = _net.TimeSec + MetricsIntervalSec;
					}
				} else {
					Thread.Sleep( 0 );
				}
			}
		} catch (ThreadAbortException) { /* shutdown */ } catch (Exception ex) {
			Console.Error.WriteLine( $"NeuralNetInterface loop exception: {ex}" );
		}
	}
}
