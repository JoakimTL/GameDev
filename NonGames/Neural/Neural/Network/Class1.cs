namespace Neural.Network;

// ---------------------
// 0) Basic neuron types
// ---------------------
public enum NeuronType { Inhibitory = -5, Excitatory = 1 }

public sealed class Axon {
	private readonly Neuron _source;
	private readonly Dictionary<Neuron, Synapse> _byTarget = new();

	public Axon( Neuron source ) { _source = source; }
	public IReadOnlyCollection<Synapse> Synapses => _byTarget.Values;

	public void AddSynapse( Neuron target, float weight ) {
		if (!_byTarget.ContainsKey( target )) {
			var s = new Synapse( target, weight );
			_byTarget.Add( target, s );
		}
	}
	public void RemoveSynapse( Neuron target ) {
		if (_byTarget.Remove( target, out var s ))
			s.Dispose();
	}

	public void OnSourceSpike() {
		foreach (var s in _byTarget.Values)
			s.OnPreSpike();
	}
	public void DeliverSpike() {
		foreach (var s in _byTarget.Values)
			s.TriggerConnection( _source );
	}
}

// --------------------------------------------------
// 1) Token encoder (deterministic k-of-N SDR per char)
// --------------------------------------------------
public static class TokenEncoder {
	public static List<Neuron> PickGroup( char token, IReadOnlyList<Neuron> pool, int k ) {
		// Deterministic PRNG per token (SplitMix-like from char code)
		ulong seed = (ulong) token * 11400714819323198485ul ^ 0x9E3779B97F4A7C15ul;
		var used = new HashSet<int>();
		var result = new List<Neuron>( k );
		while (result.Count < Math.Min( k, pool.Count )) {
			seed = seed + 0x9E3779B97F4A7C15ul;
			ulong z = (seed ^ (seed >> 30)) * 0xBF58476D1CE4E5B9ul;
			z = (z ^ (z >> 27)) * 0x94D049BB133111EBul;
			z ^= z >> 31;
			int idx = (int) (z % (ulong) pool.Count);
			if (used.Add( idx ))
				result.Add( pool[ idx ] );
		}
		return result;
	}
}

// ----------------------------------------
// 2) String buffer controlled by neurons
// ----------------------------------------
public sealed class StringBuffer {
	private readonly List<char> _buf = new();
	public int Cursor { get; private set; } = 0;
	public string Snapshot => new string( _buf.ToArray() );

	public void Append( char c ) { _buf.Insert( Cursor, c ); Cursor++; }
	public void Insert( char c ) { _buf.Insert( Cursor, c ); }
	public void Delete() { if (_buf.Count > 0 && Cursor > 0) { _buf.RemoveAt( Cursor - 1 ); Cursor--; } }
	public void MoveLeft() { if (Cursor > 0) Cursor--; }
	public void MoveRight() { if (Cursor < _buf.Count) Cursor++; }
	public void Clear() { _buf.Clear(); Cursor = 0; }
}

// ----------------------------------------
// 3) Internal critic (RPE-like dopamine)
// ----------------------------------------
public sealed class InternalCritic {
	private readonly NeuralNetwork _net;
	private float _baseline = 0f;                  // EWMA of rewards
	private readonly float _alpha = 1f / 2000f;    // ~2 s at 1 kHz

	public float LastDelta { get; private set; } = 0f;

	public InternalCritic( NeuralNetwork net ) { _net = net; }

	public void OnOutcome( float reward, float pulseSec = 0.08f ) {
		_baseline = (1 - _alpha) * _baseline + _alpha * reward;
		LastDelta = reward - _baseline;
		_net.StartDopaminePulse( LastDelta, pulseSec );
	}
}
