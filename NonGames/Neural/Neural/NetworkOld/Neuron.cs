using System.Collections.Concurrent;

namespace Neural.NetworkOld;

public enum NeuronType {
	Inhibitory = -5,
	Excitatory = 1
}
public sealed class Neuron {

	public int IncomingSynapses { get; private set; }
	public float K { get; private set; }
	public float ExhaustionLevel { get; private set; }
	public float ActivationLevel { get; private set; }
	public float RefractoryRate { get; set; }
	public float ActivationDamperPeriod { get; set; }
	public NeuronType NeuronType { get; set; }

	public float IncomingActivation { get; private set; }

	public bool ActivatedThisTick { get; private set; }
	public event Action<Neuron>? Activated;
	private readonly Axon _axon;

	public Neuron( NeuronType type ) {
		_axon = new();
		ExhaustionLevel = 0; //Seconds of exhaustion
		RefractoryRate = 1; //0.001 per second
		ActivationDamperPeriod = 0.015f; //15 millisecond period
		K = 1;
		NeuronType = type;
		ActivatedThisTick = false;
	}

	public IReadOnlyCollection<Synapse> Synapses => _axon.Synapses;

	/// <param name="time">Time in seconds</param>
	/// <param name="dt">Delta time in seconds</param>
	public void Tick( float time, float dt ) {
		ExhaustionLevel -= dt * RefractoryRate;
		ActivationLevel *= float.Exp( -dt / ActivationDamperPeriod );
		ActivationLevel = float.Max( 0, ActivationLevel );
		if (ExhaustionLevel > 0)
			return;
		ExhaustionLevel = 0;
		if (ActivationLevel >= 1) {
			Activated?.Invoke( this );
			ActivatedThisTick = true;
			ExhaustionLevel += 0.002f;
			ActivationLevel = 0;
		}
	}

	public void TriggerAxon() {
		if (ActivatedThisTick)
			_axon.Trigger( this );
	}

	public void PostTick() {
		ActivationLevel += IncomingActivation * K;
		IncomingActivation = 0;
		ActivatedThisTick = false;
	}

	public void Excite( float input ) {
		if (ExhaustionLevel > 0)
			return;
		IncomingActivation += input;
	}

	public void ConnectTo( Neuron target, float weight ) {
		_axon.AddSynapse( target, weight );
	}

	public void AddIncomingSynapse() {
		IncomingSynapses++;
		UpdateK();
	}

	public void RemoveIncomingSynapse() {
		if (IncomingSynapses == 0)
			throw new InvalidOperationException( "Already at 0 incoming synapses!" );
		IncomingSynapses--;
		UpdateK();
	}

	private void UpdateK() => K = IncomingSynapses == 0 ? 1 : 1f / float.Sqrt( IncomingSynapses );
}

public sealed class Axon {

	private readonly Dictionary<Neuron, Synapse> _synapsesByTarget;

	public Axon() {
		_synapsesByTarget = [];
	}

	public IReadOnlyCollection<Synapse> Synapses => _synapsesByTarget.Values;

	public void AddSynapse( Neuron target, float weight ) {
		if (_synapsesByTarget.ContainsKey( target )) // TODO: Check if synapse already exists and update weight instead of creating a new one
			return;
		Synapse synapse = new( target, weight );
		_synapsesByTarget.Add( target, synapse );
	}

	public void RemoveSynapse( Neuron target ) {
		if (_synapsesByTarget.Remove( target, out Synapse? synapse ))
			synapse.Dispose();
	}

	public void Trigger( Neuron neuron ) {
		foreach (var synapse in Synapses) {
			synapse.OnPreSpike();
			synapse.TriggerConnection( neuron );
		}
	}
}

public sealed class Synapse : IDisposable {

	public const float Eta = 1;
	public const float TauPre = 0.020f;
	public const float TauPost = 0.020f;
	public const float A_plus = 0.005f;
	public const float A_minus = -0.006f;
	public const float W_min = 0.0f;
	public const float W_max = 10.0f;
	public const float TauEligibility = 0.5f;
	public const float kEligibility = 0.05f;
	public const float EtaR = 1e-3f;

	private float _preTrace;
	private float _postTrace;
	private float _eligibility;
	private bool _preSpikedThisTick = false;
	private bool _postSpikedThisTick = false;

	public Neuron Target { get; private set; }
	/// <summary>
	/// Ranges from 0 to infinity. The sign is handled by the Neuron
	/// </summary>
	public float Weight { get; set; }

	public Synapse( Neuron target, float weight ) {
		if (weight < 0)
			throw new ArgumentException( $"{nameof( weight )} cannot be negative" );
		Target = target;
		Weight = weight;
		Target.AddIncomingSynapse();
		target.Activated += OnPostSpike;
	}

	public void TriggerConnection( Neuron source ) {
		Target.Excite( Weight * (int) source.NeuronType );
	}

	// call this when the SOURCE (pre) neuron spikes
	public void OnPreSpike() {
		Weight += Eta * A_minus * _postTrace;
		if (Weight < W_min)
			Weight = W_min;
		if (Weight > W_max)
			Weight = W_max;

		_preTrace += 1f;
		_preSpikedThisTick = true;
	}

	// call this when the TARGET (post) neuron spikes
	public void OnPostSpike( Neuron neuron ) {
		Weight += Eta * A_plus * _preTrace;
		if (Weight < W_min)
			Weight = W_min;
		if (Weight > W_max)
			Weight = W_max;

		_postTrace += 1f;
		_postSpikedThisTick = true;
	}

	public void Tick( float dt ) {
		// Start-of-tick housekeeping
		_preSpikedThisTick = false;
		_postSpikedThisTick = false;

		// Decay fast traces (tens of ms) and long eligibility (hundreds of ms)
		_preTrace *= float.Exp( -dt / TauPre );
		_postTrace *= float.Exp( -dt / TauPost );
		_eligibility *= float.Exp( -dt / TauEligibility );
	}

	// Call this AFTER spikes have been processed for the tick
	public void Consolidate( float dt, float delta ) {
		// Tag recent coincidences using the "other" trace at spike times
		if (_preSpikedThisTick)
			_eligibility += kEligibility * _postTrace; // post→pre
		if (_postSpikedThisTick)
			_eligibility += kEligibility * _preTrace;  // pre→post

		// Apply the *third factor* only when a dopamine-like pulse is present
		if (delta != 0f) {
			Weight += EtaR * delta * _eligibility;
			Weight = float.Clamp( Weight, W_min, W_max );
		}
	}

	public void Dispose() {
		Target.RemoveIncomingSynapse();
		Target.Activated -= OnPostSpike;
		Target = null!;
		GC.SuppressFinalize( this );
	}
}

public sealed class NeuralNetwork {

	private readonly List<Synapse> _allSynapses;
	private readonly List<Neuron> _neurons;
	private readonly List<Neuron> _inputNeurons;
	private readonly List<Neuron> _outputNeurons;

	public float CurrentDelta { get; private set; } = 0f;
	private float _deltaTimeRemaining = 0;

	public IReadOnlyList<Neuron> Neurons => _neurons;
	public IReadOnlyList<Neuron> InputNeurons => _inputNeurons;
	public IReadOnlyList<Neuron> OutputNeurons => _outputNeurons;

	public NeuralNetwork( Random random, int inputNeurons, int intermediaryNeurons, int outputNeurons ) {
		_allSynapses = [];
		_neurons = [];
		_inputNeurons = [];
		_outputNeurons = [];

		for (int i = 0; i < inputNeurons; i++) {
			var neuron = CreateNeuron( random );
			_inputNeurons.Add( neuron );
			neuron.NeuronType = NeuronType.Excitatory;
		}

		for (int i = 0; i < intermediaryNeurons; i++)
			CreateNeuron( random );

		for (int i = 0; i < outputNeurons; i++) {
			var neuron = CreateNeuron( random );
			_outputNeurons.Add( neuron );
			neuron.NeuronType = NeuronType.Excitatory;
		}
	}

	public Neuron CreateNeuron( Random random ) {
		float v = random.NextSingle();
		NeuronType type = NeuronType.Excitatory;
		if (v < 0.2f)
			type = NeuronType.Inhibitory;
		var neuron = new Neuron( type );
		_neurons.Add( neuron );
		return neuron;
	}

	public void StartDopaminePulse( float delta, float durationSeconds = 0.1f ) {
		CurrentDelta = delta;
		_deltaTimeRemaining = durationSeconds;
	}

	public void RebuildSynapseCache() {
		_allSynapses.Clear();
		_allSynapses.AddRange( _neurons.SelectMany( n => n.Synapses ) );
	}

	public void Tick( float time, float dt ) {
		foreach (var synapse in _allSynapses)
			synapse.Tick( dt );

		foreach (var neuron in _neurons)
			neuron.Tick( time, dt );

		foreach (var neuron in _neurons)
			neuron.TriggerAxon();

		float delta = _deltaTimeRemaining > 0 ? CurrentDelta : 0f;
		foreach (var s in _allSynapses)
			s.Consolidate( dt, delta );

		if (_deltaTimeRemaining > 0f) {
			_deltaTimeRemaining -= dt;
			if (_deltaTimeRemaining <= 0f) {
				_deltaTimeRemaining = 0f;
				CurrentDelta = 0f;
			}
		}
	}

	public void PostTick() {
		foreach (var neuron in _neurons)
			neuron.PostTick();
	}

}

public sealed class DopamineEvaluator {
	private readonly NeuralNetwork _net;
	private float _baseline;           // EWMA of rewards
	private readonly float _alpha = 1f / 2000f; // ~2 s time constant at 1 ms ticks

	public DopamineEvaluator( NeuralNetwork net ) { _net = net; }

	// Call once per character frame, after you know the outcome
	public void OnOutcome( bool correct ) {
		float r = correct ? 1f : 0f;   // or a graded score
		_baseline = (1 - _alpha) * _baseline + _alpha * r;
		float delta = r - _baseline;   // centered RPE
		_net.StartDopaminePulse( delta, 0.1f ); // 100 ms pulse
	}
}

public sealed class CharacterInputProcessor {
	private readonly NeuralNetwork _network;
	private readonly DopamineEvaluator _evaluator;
	private readonly OutputReadout _readout;

	private readonly Dictionary<char, List<Neuron>> _inputNeuronsByCharacter;
	private readonly List<(List<Neuron> list, char character)> _outputCharacterByNeurons;

	private readonly ConcurrentQueue<char> _inputQueue;
	private readonly ConcurrentQueue<char> _outputQueue;

	private float _frameStartTime = 0f;
	private int _currentBurst;
	private char _currentCharacter = '\0';
	public float FrameDurationSec { get; set; } = 0.02f; // 20 ms
	public float TimeBetweenBursts { get; set; } = 0.003f; // 3 ms
	public int BurstsPerFrame { get; set; } = 2;
	public float TargetDeltaPerFrame { get; set; } = 1.2f;

	public event Action<char>? OnCharacterSupplied;

	public CharacterInputProcessor( NeuralNetwork network, int seed, string outputCharacters, int neuronsPerCharacter = 100 ) {
		this._network = network;
		_evaluator = new DopamineEvaluator( network );

		_inputNeuronsByCharacter = [];
		_outputCharacterByNeurons = [];
		Random random = new( seed );
		for (int i = 0; i < char.MaxValue; i++) {
			HashSet<Neuron> inputNeurons = [];
			while (inputNeurons.Count < neuronsPerCharacter)
				inputNeurons.Add( _network.InputNeurons[ random.Next( _network.InputNeurons.Count ) ] );
			_inputNeuronsByCharacter.Add( (char) i, inputNeurons.ToList() );
		}

		foreach (var c in outputCharacters) {
			HashSet<Neuron> outputNeurons = [];
			while (outputNeurons.Count < neuronsPerCharacter)
				outputNeurons.Add( _network.OutputNeurons[ random.Next( _network.OutputNeurons.Count ) ] );
			_outputCharacterByNeurons.Add( (outputNeurons.ToList(), c) );
		}

		_readout = new OutputReadout( _outputCharacterByNeurons );

		_inputQueue = [];
		_outputQueue = [];

		_currentBurst = BurstsPerFrame;
		_readout.BeginFrame(); // initialize
	}

	// inject the input pattern for a char (you can add bursts at 0 and +3 ms if desired)
	private void InjectChar( char c ) {
		OnCharacterSupplied?.Invoke( c );
		foreach (var n in _inputNeuronsByCharacter[ c ])            // scale by 1/K so it crosses threshold at least once per frame
			n.Excite( TargetDeltaPerFrame / Math.Max( 1e-6f, n.K ) );
	}

	public void Tick( float time, float dt ) {
		// if we’re still inside the frame, optionally deliver a second burst ~3 ms after the first:
		// left as an exercise (track last-burst time and call InjectChar again)

		if (_frameStartTime == 0f && _inputQueue.TryDequeue( out var first )) {
			_currentCharacter = first;
			InjectChar( _currentCharacter );
			_currentBurst = 1;                 // we just injected burst #1
			_frameStartTime = time;
			_readout.BeginFrame();
			return;
		}

		// if the frame ended, do predict → compare → δ pulse → inject next char → new frame
		if (time - _frameStartTime >= FrameDurationSec) {
			_readout.EndFrame();
			var (pred, _) = _readout.Predict();

			if (_inputQueue.TryDequeue( out _currentCharacter )) {
				bool correct = (pred == _currentCharacter);
				_evaluator.OnOutcome( correct );

				// optional: emit prediction
				_outputQueue.Enqueue( pred );

				// start next frame with the first burst now
				_readout.BeginFrame();
				_frameStartTime = time;
				InjectChar( _currentCharacter );
				_currentBurst = 1;
			} else {
				// no new char; still start a new frame window to keep time moving
				_readout.BeginFrame();
				_frameStartTime = time;
				_currentBurst = BurstsPerFrame;
			}
			return; // we've handled this tick's frame boundary work
		}

		if (time - _frameStartTime >= TimeBetweenBursts * _currentBurst && _currentBurst < BurstsPerFrame) {
			InjectChar( _currentCharacter );
			_currentBurst++;
		}
	}

	public bool ReceiveNext( out char character ) => _outputQueue.TryDequeue( out character );

	public void Send( string text ) {
		foreach (var character in text)
			_inputQueue.Enqueue( character );
	}
}

public sealed class NeuralInterface {

	private BackgroundDrive _backgroundDrive;
	private NeuralNetwork _network;
	private CharacterInputProcessor _characterInputProcessor;
	private readonly Thread _neuralThread;

	private readonly ConcurrentQueue<char> _outputQueue;

	public float NetworkTime { get; private set; }
	public float AverageActivity { get; private set; }

	public event Action? OnNeuralNetworkReady;
	public event Action? OnOutputReceived;
	public event Action<Neuron>? OnNeuronActivated;
	public event Action<char>? OnCharacterSupplied; // for debugging>

	public NeuralInterface() {
		NetworkTime = 0;
		_backgroundDrive = null!;
		_network = null!;
		_characterInputProcessor = null!;
		_outputQueue = [];
		_neuralThread = new Thread( RunNeuralThread ) {
			Name = "NeuralThread",
			IsBackground = true
		};
		_neuralThread.Start();
	}

	private void RunNeuralThread() {
		float time = 0;
		float deltaTime = 0.001f;

		Random networkRandom = new();
		this._network = new( networkRandom, 16384, 32768, 1024 );
		_backgroundDrive = new( _network, networkRandom.Next() );
		Wiring.WireAll( networkRandom, _network, wERec: 1.5f, wIn: 3, wOut: 1.75f );
		this._characterInputProcessor = new CharacterInputProcessor( _network, 42, "abcdefghijklmnopqrstuvwxyz.!? " );

		foreach (var n in _network.Neurons) {
			n.Activated += NeuronActivated;
		}

		_characterInputProcessor.OnCharacterSupplied += CharacterSupplied;

		OnNeuralNetworkReady?.Invoke();

		while (true) {
			_characterInputProcessor.Tick( time, deltaTime );
			_backgroundDrive.Tick();
			_network.Tick( time, deltaTime );
			time += deltaTime;
			NetworkTime = time;
			//_characterInputProcessor.PostTick();
			_network.PostTick();

			float avg = 0;
			foreach (var n in _network.Neurons) {
				avg += n.ActivationLevel;
			}
			AverageActivity = avg / _network.Neurons.Count;

			if (_characterInputProcessor.ReceiveNext( out var character )) {
				_outputQueue.Enqueue( character );
				OnOutputReceived?.Invoke();
			}
		}
	}

	private void CharacterSupplied( char character ) => OnCharacterSupplied?.Invoke( character );

	private void NeuronActivated( Neuron neuron ) {
		OnNeuronActivated?.Invoke( neuron );
	}

	public void Send( string text ) {
		_characterInputProcessor?.Send( text );
	}

	public bool ReceiveNext( out char character ) => _outputQueue.TryDequeue( out character );
}

public sealed class OutputReadout {
	private readonly List<(List<Neuron> list, char character)> _groups;
	private readonly Dictionary<Neuron, List<int>> _neuronGroups; // which char-group a neuron belongs to
	private readonly float[] _scores; // one score per group (normalized 0..1)
	private readonly int[] _spikeCounts; // spikes observed this frame per group
	private readonly int[] _groupSizes;

	public OutputReadout( List<(List<Neuron> list, char character)> groups ) {
		_groups = groups;
		_scores = new float[ _groups.Count ];
		_spikeCounts = new int[ _groups.Count ];
		_groupSizes = [ .. _groups.Select( g => g.list.Count ) ];

		_neuronGroups = [];
		for (int gi = 0; gi < _groups.Count; gi++) {
			int g = gi; // capture copy
			foreach (var n in _groups[ g ].list)
				AddNeuronToGroup( n, g );
		}

		foreach (var n in groups.SelectMany( g => g.list ).Distinct())
			n.Activated += NeuronActivated;
	}

	private void AddNeuronToGroup( Neuron n, int gi ) {
		if (!_neuronGroups.TryGetValue( n, out var list ))
			_neuronGroups[ n ] = list = [];
		list.Add( gi );
	}

	private void NeuronActivated( Neuron n ) {
		var groups = _neuronGroups[ n ];
		foreach (var g in groups)
			_spikeCounts[ g ]++;
	}

	public void BeginFrame() {
		Array.Clear( _spikeCounts, 0, _spikeCounts.Length );
		Array.Clear( _scores, 0, _scores.Length );
	}

	public void EndFrame() {
		for (int gi = 0; gi < _groups.Count; gi++)          // normalize by group size → approx fraction of neurons that spiked
			_scores[ gi ] = (float) _spikeCounts[ gi ] / Math.Max( 1, _groupSizes[ gi ] );
	}

	public (char predicted, float score) Predict() {
		int argmax = 0;
		float best = _scores[ 0 ];
		for (int i = 1; i < _scores.Length; i++)
			if (_scores[ i ] > best) { best = _scores[ i ]; argmax = i; }
		return (_groups[ argmax ].character, best);
	}
}

public static class Wiring {
	// Top-level convenience: wire reservoir recurrent, then input→reservoir, then reservoir→output
	public static void WireAll( Random rng, NeuralNetwork net,
							   int kExcRec = 100, int kInhRec = 25, float wERec = 0.12f,
							   int fanoutInput = 80, float wIn = 0.15f,
							   int faninOutput = 120, float wOut = 0.12f ) {
		var (reservoir, excReservoir, inhReservoir) = GetReservoirSets( net );

		// 1) Reservoir recurrent: E/I → reservoir targets only
		ConnectPopulation( rng, excReservoir, reservoir, kExcRec, wERec, avoidAutapse: true );
		ConnectPopulation( rng, inhReservoir, reservoir, kInhRec, wERec, avoidAutapse: true ); // inhibitory strength comes from your NeuronType = -5

		// 2) Inputs drive reservoir (no input→output)
		WireInputFanout( rng, net, excTargets: reservoir, fanout: fanoutInput, wIn: wIn );

		// 3) Reservoir (usually excitatory) drives outputs
		WireReservoirToOutput( rng, excReservoir, net.OutputNeurons, faninOutput, wOut );

		// Build flat synapse cache once
		net.RebuildSynapseCache();
	}

	// Build reservoir sets once
	private static (Neuron[] reservoir, Neuron[] excRes, Neuron[] inhRes) GetReservoirSets( NeuralNetwork net ) {
		var inputSet = new HashSet<Neuron>( net.InputNeurons );
		var outputSet = new HashSet<Neuron>( net.OutputNeurons );

		var reservoir = net.Neurons.Where( n => !inputSet.Contains( n ) && !outputSet.Contains( n ) ).ToArray();
		var excRes = reservoir.Where( n => n.NeuronType == NeuronType.Excitatory ).ToArray();
		var inhRes = reservoir.Where( n => n.NeuronType == NeuronType.Inhibitory ).ToArray();

		return (reservoir, excRes, inhRes);
	}

	// Random sparse connections from srcPool to targets (distinct presyn sources; no autapse)
	private static void ConnectPopulation( Random rng, IReadOnlyList<Neuron> srcPool, IReadOnlyList<Neuron> targets,
										  int k, float w, bool avoidAutapse ) {
		int n = srcPool.Count;
		foreach (var trg in targets) {
			var used = new HashSet<int>();
			int needed = Math.Min( k, n );
			while (used.Count < needed) {
				int i = rng.Next( n );
				var src = srcPool[ i ];
				if (avoidAutapse && ReferenceEquals( src, trg ))
					continue;
				if (used.Add( i ))
					src.ConnectTo( trg, w );
			}
		}
	}

	// Inputs → reservoir (fan-out per input). We target reservoir EXC neurons by default.
	public static void WireInputFanout( Random rng, NeuralNetwork net, IReadOnlyList<Neuron> excTargets,
									   int fanout = 80, float wIn = 0.15f ) {
		int nT = excTargets.Count;
		foreach (var src in net.InputNeurons) {
			var used = new HashSet<int>();
			int needed = Math.Min( fanout, nT );
			while (used.Count < needed) {
				int j = rng.Next( nT );
				var trg = excTargets[ j ];
				if (used.Add( j ))
					src.ConnectTo( trg, wIn );
			}
		}
	}

	// Reservoir (usually excitatory only) → output (fan-in per output)
	public static void WireReservoirToOutput( Random rng, IReadOnlyList<Neuron> excReservoir,
											 IReadOnlyList<Neuron> outputs,
											 int faninPerOutput = 120, float wOut = 0.12f ) {
		int nS = excReservoir.Count;
		foreach (var trg in outputs) {
			var used = new HashSet<int>();
			int needed = Math.Min( faninPerOutput, nS );
			while (used.Count < needed) {
				int i = rng.Next( nS );
				var src = excReservoir[ i ];
				if (ReferenceEquals( src, trg ))
					continue; // should never be equal, but be safe
				if (used.Add( i ))
					src.ConnectTo( trg, wOut );
			}
		}
	}
}
public sealed class BackgroundDrive {
	private readonly NeuralNetwork _net;
	private readonly Random _rng;
	public float RateHz { get; set; } = 3f;     // per neuron
	public float Amplitude { get; set; } = 0.06f; // in same units as weight (will be multiplied by target K in PostTick)

	public BackgroundDrive( NeuralNetwork net, int seed ) {
		_net = net;
		_rng = new Random( seed );
	}

	public void Tick() {
		float p = RateHz / 1000f;
		foreach (var n in _net.Neurons) {
			if (_rng.NextSingle() < p)
				n.Excite( Amplitude );
		}
	}
}