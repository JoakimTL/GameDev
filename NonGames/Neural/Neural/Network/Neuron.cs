namespace Neural.Network;

public sealed class Neuron {
	public int IncomingSynapses { get; private set; }
	public float K { get; private set; } = 1f;

	public float ExhaustionLevel { get; private set; } = 0f;  // seconds remaining in refractory
	public float ActivationLevel { get; private set; } = 0f;  // membrane-like state
	public float RefractoryRate { get; set; } = 1f;           // sec/sec (countdown)
	public float ActivationDamperPeriod { get; set; } = 0.015f; // ~15 ms
	public NeuronType NeuronType { get; set; }

	public float IncomingActivation { get; private set; } = 0f;
	public bool ActivatedThisTick { get; private set; } = false;
	public float EwmaFiringRateHz { get; private set; }  // smoothed rate (Hz)
	public float TauRateSec { get; set; } = 1.0f;        // EWMA time-constant (s)

	public event Action? Activated;

	private readonly Axon _axon;

	public Neuron( NeuronType type ) {
		NeuronType = type;
		_axon = new Axon( this );
	}

	public IReadOnlyCollection<Synapse> Synapses => _axon.Synapses;


	public void ConnectTo( Neuron target, float weight ) => _axon.AddSynapse( target, weight );

	public void AddIncomingSynapse() { IncomingSynapses++; UpdateK(); }
	public void RemoveIncomingSynapse() { IncomingSynapses = Math.Max( 0, IncomingSynapses - 1 ); UpdateK(); }

	private void UpdateK() {
		float eff = MathF.Max( 1f, IncomingSynapses * 0.25f ); // 4× softer
		K = 1f / MathF.Sqrt( eff );
	}

	public void Excite( float input ) {
		if (ExhaustionLevel > 0f)
			return;
		IncomingActivation += input;
	}

	public void Tick( float time, float dt ) {
		// decay refractory & membrane
		ExhaustionLevel -= dt * RefractoryRate;
		if (ExhaustionLevel < 0f)
			ExhaustionLevel = 0f;

		ActivationLevel *= float.Exp( -dt / ActivationDamperPeriod );
		if (ActivationLevel < 0f)
			ActivationLevel = 0f;

		// exponential decay of the rate estimate
		EwmaFiringRateHz *= MathF.Exp( -dt / TauRateSec );

		ActivatedThisTick = false;

		if (ExhaustionLevel > 0f)
			return;

		if (ActivationLevel >= 1f) {
			Activated?.Invoke();
			ActivatedThisTick = true;
			EwmaFiringRateHz += 1f / TauRateSec;

			// spike effects
			_axon.OnSourceSpike();             // STDP pre events
			_axon.DeliverSpike();              // postsyn excitation/inhibition

			// enter short absolute refractory and reset the state
			ExhaustionLevel += 0.003f;         // 3 ms
			ActivationLevel = 0f;
		}
	}

	public void PostTick() {
		// integrate incoming inputs once per tick (discrete Euler)
		ActivationLevel += IncomingActivation * K;
		IncomingActivation = 0f;
	}
}
