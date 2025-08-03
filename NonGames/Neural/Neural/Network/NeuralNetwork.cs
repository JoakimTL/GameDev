using Neural.NetworkOld;

namespace Neural.Network;

// ------------------------------------------
// 5) The autonomous neural network itself
// ------------------------------------------
public sealed class NeuralNetwork {
	// --- Neuron pools
	private readonly List<Neuron> _neurons = new();
	private readonly List<Neuron> _inputs = new();
	private readonly List<Neuron> _reservoirExc = new();
	private readonly List<Neuron> _reservoirInh = new();
	private readonly List<Neuron> _predictionNeurons = new();
	private readonly List<Neuron> _actionNeurons = new();

	// --- Fast synapse cache (no per-tick LINQ)
	private readonly List<Synapse> _allSynapses = new();
	private bool _synCacheDirty = true;

	// --- Dopamine pulse state
	public float CurrentDelta { get; private set; } = 0f;
	private float _deltaTimeRemaining = 0f;

	// --- Token groups (prediction head)
	private readonly char[] _alphabet;
	private readonly Dictionary<char, int> _tokIndex = new();
	private readonly List<(List<Neuron> list, char token)> _predGroups = new();
	private readonly Dictionary<Neuron, List<int>> _predGroupsForNeuron = new(); // inverted index

	// --- Action groups
	private readonly string[] _actions = new[] { "Append", "Insert", "Delete", "Left", "Right", "Commit", "ReadNext", "Clear" };
	private readonly Dictionary<string, int> _actIndex = new();
	private readonly List<(List<Neuron> list, string action)> _actGroups = new();
	private readonly Dictionary<Neuron, List<int>> _actGroupsForNeuron = new();

	// --- Gates
	private Gate _predGate;   // chooses token continuously
	private Gate _actGate;    // chooses actions continuously

	// --- Short-term participation accumulators (for bias nudges)
	private readonly float[] _predParticipation;
	private readonly float[] _actParticipation;

	// --- Internal critic
	private readonly InternalCritic _critic;

	// --- Host-facing adapters
	public Action? OnReadNext;             // when "ReadNext" wins, host injects next input char
	public Action<string>? OnEmitString;   // when "Commit" wins, host receives the emitted string

	// --- Working memory (string buffer)
	private readonly StringBuffer _buffer = new();

	// --- Timing
	public float TimeSec { get; private set; } = 0f;

	// --- Background drive
	private readonly Random _rng = new( 12345 );
	public float BackgroundRateHz { get; set; } = 6f;     // per neuron
	public float BackgroundAmp { get; set; } = 0.1f;

	// --- Wiring config defaults
	public int KExcRec = 100, KInhRec = 25;
	public float WExcRec = 0.12f, WInFeed = 0.15f, WOut = 0.12f;
	public int FanoutInput = 80, FaninOut = 120;
	public int SDR_K_Input = 100, SDR_K_Pred = 100, SDR_K_Act = 50;

	// --- Metrics counters (reset every window)
	private long _spikesInput = 0, _spikesResExc = 0, _spikesResInh = 0, _spikesPred = 0, _spikesAct = 0;

	// NeuralNetwork fields
	public bool BootstrapInputToPrediction = true;
	public float BootstrapLeakAmp = 0.08f; // per burst, scaled by K

	public float TargetRateHz = 6;
	public float EtaScale = 5e-4f;

	private float _Rbar = 0f;             // EWMA of episode reward
	private readonly float _alphaR = 1f / 2000f; // ~2 s EWMA

												 // --- Construction
	public NeuralNetwork( char[] alphabet,
						 int nInput, int nReservoirExc, int nReservoirInh,
						 int nPrediction, int nAction ) {
		_alphabet = alphabet;
		for (int i = 0; i < alphabet.Length; i++)
			_tokIndex[ alphabet[ i ] ] = i;

		// build neurons
		for (int i = 0; i < nInput; i++)
			_inputs.Add( AddNeuron( NeuronType.Excitatory ) );
		for (int i = 0; i < nReservoirExc; i++)
			_reservoirExc.Add( AddNeuron( NeuronType.Excitatory ) );
		for (int i = 0; i < nReservoirInh; i++)
			_reservoirInh.Add( AddNeuron( NeuronType.Inhibitory ) );
		for (int i = 0; i < nPrediction; i++)
			_predictionNeurons.Add( AddNeuron( NeuronType.Excitatory ) );
		for (int i = 0; i < nAction; i++)
			_actionNeurons.Add( AddNeuron( NeuronType.Excitatory ) );

		// build prediction groups (token SDRs over prediction neurons)
		foreach (char t in _alphabet) {
			var grp = TokenEncoder.PickGroup( t, _predictionNeurons, SDR_K_Pred );
			_predGroups.Add( (grp, t) );
		}
		BuildInvertedIndex( _predGroups, _predGroupsForNeuron );

		// build action groups (random SDRs over action neurons)
		for (int ai = 0; ai < _actions.Length; ai++) {
			var grp = TokenEncoder.PickGroup( (char) (ai + 1), _actionNeurons, SDR_K_Act );
			_actIndex[ _actions[ ai ] ] = ai;
			_actGroups.Add( (grp, _actions[ ai ]) );
		}
		BuildInvertedIndex( _actGroups, _actGroupsForNeuron );

		// subscribe once per neuron to increment group participation (and evidence in gates)
		_predParticipation = new float[ _predGroups.Count ];
		_actParticipation = new float[ _actGroups.Count ];

		foreach (var kv in _predGroupsForNeuron) {
			var n = kv.Key;
			var groups = kv.Value;
			n.Activated += () => {
				for (int i = 0; i < groups.Count; i++) {
					int gi = groups[ i ];
					_predParticipation[ gi ] += 1f;
					_predGate.AddEvidence( gi, 1f );
				}
			};
		}
		foreach (var kv in _actGroupsForNeuron) {
			var n = kv.Key;
			var groups = kv.Value;
			n.Activated += () => {
				for (int i = 0; i < groups.Count; i++) {
					int gi = groups[ i ];
					_actParticipation[ gi ] += 1f;
					_actGate.AddEvidence( gi, 1f );
				}
			};
		}

		//_predGate = new Gate( _predGroups.Count, tau: 0.05f, bound: 0.30f, margin: 0.05f, dwellSec: 0.010f );
		//_actGate = new Gate( _actGroups.Count, tau: 0.05f, bound: 0.25f, margin: 0.05f, dwellSec: 0.010f );
		//_predGate = new Gate( _predGroups.Count, tau: 0.05f, bound: 0.18f, margin: 0.01f, dwellSec: 0.003f );
		//_actGate = new Gate( _actGroups.Count, tau: 0.05f, bound: 0.18f, margin: 0.01f, dwellSec: 0.003f );
		_predGate = new Gate( _predGroups.Count, tau: 0.05f, bound: 0.18f, margin: 0.008f, dwellSec: 0.003f );
		_actGate = new Gate( _actGroups.Count, tau: 0.05f, bound: 0.18f, margin: 0.008f, dwellSec: 0.003f );

		_actGate.AddBias( _actIndex[ "Append" ], 0.30f );
		_actGate.AddBias( _actIndex[ "Commit" ], 0.05f );

		_critic = new InternalCritic( this );

		// Count spikes per population (metrics)
		foreach (var n in _inputs)
			n.Activated += () => { _spikesInput++; };
		foreach (var n in _reservoirExc)
			n.Activated += () => { _spikesResExc++; };
		foreach (var n in _reservoirInh)
			n.Activated += () => { _spikesResInh++; };
		foreach (var n in _predictionNeurons)
			n.Activated += () => { _spikesPred++; };
		foreach (var n in _actionNeurons)
			n.Activated += () => { _spikesAct++; };

	}

	private Neuron AddNeuron( NeuronType t ) { var n = new Neuron( t ); _neurons.Add( n ); return n; }

	public (float actTop, float actGap, float predTop, float predGap) ProbeGates() {
		var (ai, ab, as2) = _actGate.PeekTop();
		var (pi, pb, ps2) = _predGate.PeekTop();
		return (ab, ab - as2, pb, pb - ps2);
	}

	// expose a helper to emit a dopamine pulse; you already have StartDopaminePulse(delta, dur)
	public void ReinforceEpisode( float reward ) {
		// Centered RPE: δ = R - \bar R
		_Rbar = (1 - _alphaR) * _Rbar + _alphaR * reward;
		float delta = reward - _Rbar;
		StartDopaminePulse( delta, 0.10f ); // 100 ms pulse gates plasticity via Consolidate
	}

	private static void BuildInvertedIndex<T>( List<(List<Neuron> list, T tag)> groups, Dictionary<Neuron, List<int>> inv ) {
		for (int gi = 0; gi < groups.Count; gi++)
			foreach (var n in groups[ gi ].list) {
				if (!inv.TryGetValue( n, out var lst ))
					inv[ n ] = lst = new List<int>( 4 );
				lst.Add( gi );
			}
	}

	public string BufferSnapshot => _buffer.Snapshot;

	// ---- PUBLIC control: wiring / input injection / dopamine
	public void WireAll( int seed = 7 ) {
		var rng = new Random( seed );
		var reservoir = _reservoirExc.Concat( _reservoirInh ).ToArray();

		// 1) reservoir recurrent (Dale’s law)
		ConnectPopulation( rng, _reservoirExc, reservoir, KExcRec, WExcRec, avoidAutapse: true );
		ConnectPopulation( rng, _reservoirInh, reservoir, KInhRec, WExcRec, avoidAutapse: true );

		// 2) input → reservoir (fanout per input)
		WireInputFanout( rng, _inputs, _reservoirExc, FanoutInput, WInFeed );

		// 3) reservoir (exc) → prediction & action heads
		WireReservoirToHead( rng, _reservoirExc, _predictionNeurons, FaninOut, WOut );
		WireReservoirToHead( rng, _reservoirExc, _actionNeurons, FaninOut, WOut );

		RebuildSynapseCache();
	}

	public void RebuildSynapseCache() {
		_allSynapses.Clear();
		foreach (var n in _neurons)
			_allSynapses.AddRange( n.Synapses );
		_synCacheDirty = false;
	}

	public void StartDopaminePulse( float delta, float durationSeconds = 0.08f ) {
		CurrentDelta = delta;
		_deltaTimeRemaining = Math.Max( _deltaTimeRemaining, durationSeconds );
	}

	// Inject a token SDR into the INPUT layer (neural control should trigger OnReadNext)
	public void InjectToken( char token, int bursts = 2, float burstSpacingSec = 0.003f, float targetDeltaPerBurst = 0.6f ) {
		var inGrp = TokenEncoder.PickGroup( token, _inputs, SDR_K_Input );

		for (int b = 0; b < bursts; b++) {
			foreach (var n in inGrp)
				n.Excite( targetDeltaPerBurst / Math.Max( 1e-6f, n.K ) );

			if (BootstrapInputToPrediction && _tokIndex.TryGetValue( token, out int gi )) {
				var predGrp = _predGroups[ gi ].list;
				foreach (var n in predGrp)
					n.Excite( BootstrapLeakAmp / Math.Max( 1e-6f, n.K ) );
			}
		}
	}

	// ---- Main tick (call at 1 kHz or similar)
	public void Tick( float dt ) {
		TimeSec += dt;
		if (_synCacheDirty)
			RebuildSynapseCache();

		foreach (var n in _neurons.Where(p => p.NeuronType == NeuronType.Excitatory)) {
			float err = n.EwmaFiringRateHz - TargetRateHz;          // compute elsewhere
			float scale = 1f - EtaScale * err;                        // EtaScale ~ 1e-3..1e-4 per second
			foreach (var syn in n.Synapses)
				syn.Weight = Math.Clamp( syn.Weight * scale, Synapse.W_min, Synapse.W_max );
		}

		// Background Poisson drive to keep reservoir alive
		float p = BackgroundRateHz * dt;
		foreach (var n in _reservoirExc)
			if (_rng.NextDouble() < p)
				n.Excite( BackgroundAmp / n.K );
		foreach (var n in _predictionNeurons)
			if (_rng.NextDouble() < p)
				n.Excite( BackgroundAmp / n.K );
		foreach (var n in _actionNeurons)
			if (_rng.NextDouble() < p)
				n.Excite( BackgroundAmp / n.K );

		// 1) Synapse housekeeping
		for (int i = 0; i < _allSynapses.Count; i++)
			_allSynapses[ i ].Tick( dt );

		// 2) Neuron dynamics
		for (int i = 0; i < _neurons.Count; i++)
			_neurons[ i ].Tick( TimeSec, dt );
		for (int i = 0; i < _neurons.Count; i++)
			_neurons[ i ].PostTick();

		// 3) Gates (prediction & action) compete continuously
		_predGate.Decay( dt );
		_actGate.Decay( dt );

		if (_actGate.TryDecide( TimeSec, dt, out int actIdx, out _ )) {
			// decode token (current best prediction) if needed by the action
			char tok = DecodeCurrentToken( out bool hasTok );
			string act = _actions[ actIdx ];
			switch (act) {
				case "Append":
					var (idx, best, second) = _predGate.PeekTop();
					bool ok = best > 0.06f && (best - second) >= 0.006f; // start loose
					if (ok)
						_buffer.Append( _alphabet[ idx ] );

					break;
				case "Insert":
					if (hasTok)
						_buffer.Insert( tok );
					break;
				case "Delete":
					_buffer.Delete();
					break;
				case "Left":
					_buffer.MoveLeft();
					break;
				case "Right":
					_buffer.MoveRight();
					break;
				case "Clear":
					_buffer.Clear();
					break;
				case "ReadNext":
					OnReadNext?.Invoke();
					break;
				case "Commit": {
						string s = _buffer.Snapshot;
						OnEmitString?.Invoke( s );        // host evaluates; call Critic below from host
						_buffer.Clear();
						ZeroEpisodeParticipation();     // clear accumulators
														// 2) get a scalar reward R for the whole sequence (0..1 or -1..+1)
						float R = 0f;
						// (A) INTERNAL task scoring (e.g., exact match to a known answer):
						if (_episodeScorer != null)
							R = _episodeScorer.Score( CurrentPrompt, output );

						// (B) or EXTERNAL: your UI/host can call net.ProvideEpisodeReward(R) later.
						// If you want to do it synchronously here, compute R now (from a deterministic checker).

						// 3) convert to δ pulse
						ReinforceEpisode( R );
					}
					break;
			}
		}

		// 4) Plasticity consolidation gated by internal DA pulse state
		float delta = _deltaTimeRemaining > 0f ? CurrentDelta : 0f;
		for (int i = 0; i < _allSynapses.Count; i++)
			_allSynapses[ i ].Consolidate( dt, delta );

		if (_deltaTimeRemaining > 0f) {
			_deltaTimeRemaining -= dt;
			if (_deltaTimeRemaining <= 0f) { _deltaTimeRemaining = 0f; CurrentDelta = 0f; }
		}
	}

	private char DecodeCurrentToken( out bool ok ) {
		// read the current winner from the prediction gate by doing a dry decision step:
		// We can approximate by peeking at internal evidence via a tiny hack:
		// In this minimal version we re-run TryDecide BUT with zero dwell by duplicating logic.
		// Simpler: keep a tiny rolling "last predicted" updated via the event handlers.
		// Here, we choose the token whose neurons spiked most recently in this tick window,
		// approximated by the participation accumulator: the argmax of _predParticipation.
		int argmax = 0;
		float best = _predParticipation[ 0 ];
		for (int i = 1; i < _predParticipation.Length; i++)
			if (_predParticipation[ i ] > best) { best = _predParticipation[ i ]; argmax = i; }
		ok = best > 0f;
		return _alphabet[ argmax ];
	}

	// Host calls this after it decides reward for an emitted string.
	public void ApplyOutcome( float reward ) {
		_critic.OnOutcome( reward, 0.08f );
		// Short-term bias nudge toward groups that participated
		const float kappa = 0.02f;
		for (int i = 0; i < _predParticipation.Length; i++)
			if (_predParticipation[ i ] > 0f)
				_predGate.AddBias( i, kappa * reward * _predParticipation[ i ] );

		for (int i = 0; i < _actParticipation.Length; i++)
			if (_actParticipation[ i ] > 0f)
				_actGate.AddBias( i, kappa * reward * _actParticipation[ i ] );
	}

	private void ZeroEpisodeParticipation() {
		Array.Clear( _predParticipation, 0, _predParticipation.Length );
		Array.Clear( _actParticipation, 0, _actParticipation.Length );
	}

	// --- Wiring helpers (sparse, Dale’s law)
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
	private static void WireInputFanout( Random rng, IReadOnlyList<Neuron> inputs, IReadOnlyList<Neuron> excTargets,
										int fanout, float wIn ) {
		int nT = excTargets.Count;
		foreach (var src in inputs) {
			var used = new HashSet<int>();
			int need = Math.Min( fanout, nT );
			while (used.Count < need) {
				int j = rng.Next( nT );
				var trg = excTargets[ j ];
				if (used.Add( j ))
					src.ConnectTo( trg, wIn );
			}
		}
	}
	private static void WireReservoirToHead( Random rng, IReadOnlyList<Neuron> excReservoir, IReadOnlyList<Neuron> head,
											int faninPerHead, float w ) {
		int nS = excReservoir.Count;
		foreach (var trg in head) {
			var used = new HashSet<int>();
			int need = Math.Min( faninPerHead, nS );
			while (used.Count < need) {
				int i = rng.Next( nS );
				var src = excReservoir[ i ];
				if (ReferenceEquals( src, trg ))
					continue;
				if (used.Add( i ))
					src.ConnectTo( trg, w );
			}
		}
	}

	public NetworkMetrics CollectMetricsAndReset( float windowSec, int emissionsInWindow, int pendingInputCount ) {
		// Averages at sampling time
		float avgActivation = 0f;
		for (int i = 0; i < _neurons.Count; i++)
			avgActivation += _neurons[ i ].ActivationLevel;
		avgActivation /= Math.Max( 1, _neurons.Count );

		// Spike rates (per second in this window)
		long sIn = _spikesInput;
		long sRE = _spikesResExc;
		long sRI = _spikesResInh;
		long sPred = _spikesPred;
		long sAct = _spikesAct;
		long sTot = sIn + sRE + sRI + sPred + sAct;

		float rIn = sIn / Math.Max( 1e-6f, windowSec );
		float rRE = sRE / Math.Max( 1e-6f, windowSec );
		float rRI = sRI / Math.Max( 1e-6f, windowSec );
		float rPred = sPred / Math.Max( 1e-6f, windowSec );
		float rAct = sAct / Math.Max( 1e-6f, windowSec );
		float rTot = sTot / Math.Max( 1e-6f, windowSec );

		// Firing rate per neuron (Hz)
		float meanFR = rTot / Math.Max( 1, _neurons.Count );

		// Synapse saturation
		int totalSyn = _allSynapses.Count;
		int satLo = 0, satHi = 0;
		if (totalSyn > 0) {
			const float eps = 1e-6f;
			for (int i = 0; i < _allSynapses.Count; i++) {
				var w = _allSynapses[ i ].Weight;
				if (w <= Synapse.W_min + eps)
					satLo++;
				if (w >= Synapse.W_max - eps)
					satHi++;
			}
		}
		float satLoPct = totalSyn > 0 ? (100f * satLo / totalSyn) : 0f;
		float satHiPct = totalSyn > 0 ? (100f * satHi / totalSyn) : 0f;

		var (ab, ag, pb, pg) = ProbeGates(); // expose a public passthrough if needed
		// Build snapshot
		var snapshot = new NetworkMetrics(
			SimTimeSec: TimeSec,
			AvgActivation: avgActivation,
			SpikesPerSec_Total: rTot,
			SpikesPerSec_Input: rIn,
			SpikesPerSec_ResExc: rRE,
			SpikesPerSec_ResInh: rRI,
			SpikesPerSec_Pred: rPred,
			SpikesPerSec_Act: rAct,
			MeanFiringRateHz: meanFR,
			DopamineDelta: CurrentDelta,
			DopamineRemainingSec: _deltaTimeRemaining,
			WeightSaturationLowPct: satLoPct,
			WeightSaturationHighPct: satHiPct,
			EmissionsInWindow: emissionsInWindow,
			PendingInputCount: pendingInputCount,
			ActGap: ag,
			PredictionGap: pg,
			ActTop: ab,
			PredictionTop: pb
		);

		// Reset counters for next window
		_spikesInput = _spikesResExc = _spikesResInh = _spikesPred = _spikesAct = 0;

		return snapshot;
	}
}
