namespace Neural.Network;

public sealed class Synapse : IDisposable {
	// ── Learning constants (tune later) ───────────────────────────────────────
	public const float Eta = 1.0f;      // scale for bare STDP (if enabled)
	public const float TauPre = 0.020f;    // 20 ms
	public const float TauPost = 0.020f;    // 20 ms
	public const float A_plus = 0.0002f;   // tiny unsupervised LTP (safe)
	public const float A_minus = -0.0002f;  // tiny unsupervised LTD (safe)

	public const float W_min = 0.02f;     // non-zero floor so dead synapses can recover
	public const float W_max = 10.0f;

	public const float TauEligibility = 0.7f;  // long enough to bridge Commit
	public const float kEligibility = 0.05f; // tag size
	public const float EtaR = 0.02f; // reward-modulated learning rate

	public static bool UseBareSTDP = false; // <─ turn it off (or leave tiny A± above)

	// ── State ────────────────────────────────────────────────────────────────
	private float _preTrace, _postTrace, _eligibility;
	private bool _preSpikedThisTick = false, _postSpikedThisTick = false;

	public Neuron Target { get; private set; }
	public float Weight { get; set; }

	public Synapse( Neuron target, float weight ) {
		if (weight < 0)
			throw new ArgumentException( $"{nameof( weight )} cannot be negative" );
		Target = target;
		Weight = MathF.Max( W_min, weight );
		Target.AddIncomingSynapse();
		target.Activated += OnPostSpike;
	}

	public void TriggerConnection( Neuron source ) {
		Target.Excite( Weight * (int) source.NeuronType );
	}

	// SOURCE spike (pre)
	public void OnPreSpike() {
		if (UseBareSTDP) {
			// multiplicative LTD; vanishes as Weight→W_min
			Weight += Eta * A_minus * _postTrace * MathF.Max( 0f, (Weight - W_min) );
			Weight = float.Clamp( Weight, W_min, W_max );
		}
		_preTrace += 1f;
		_preSpikedThisTick = true;
	}

	// TARGET spike (post)
	public void OnPostSpike() {
		if (UseBareSTDP) {
			// multiplicative LTP; vanishes as Weight→W_max
			Weight += Eta * A_plus * _preTrace * MathF.Max( 0f, (W_max - Weight) );
			Weight = float.Clamp( Weight, W_min, W_max );
		}
		_postTrace += 1f;
		_postSpikedThisTick = true;
	}

	// call each ms at start of tick
	public void Tick( float dt ) {
		_preSpikedThisTick = false;
		_postSpikedThisTick = false;
		_preTrace *= MathF.Exp( -dt / TauPre );
		_postTrace *= MathF.Exp( -dt / TauPost );
		_eligibility *= MathF.Exp( -dt / TauEligibility );
	}

	// call once per ms AFTER spikes/axon triggering; delta may be 0 most of the time
	public void Consolidate( float dt, float delta ) {
		if (_preSpikedThisTick)
			_eligibility += kEligibility * _postTrace; // post→pre coincidence
		if (_postSpikedThisTick)
			_eligibility += kEligibility * _preTrace;  // pre→post coincidence

		if (delta != 0f) {
			// weight-dependent 3rd factor; stronger where it helps, mild near bounds
			float span = delta > 0f ? (W_max - Weight) : (Weight - W_min);
			if (span > 0f) {
				Weight += EtaR * delta * _eligibility * span;
				Weight = float.Clamp( Weight, W_min, W_max );
			}
		}
	}

	public void Dispose() {
		Target.RemoveIncomingSynapse();
		Target.Activated -= OnPostSpike;
		Target = null!;
		GC.SuppressFinalize( this );
	}
}