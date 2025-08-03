namespace Neural.Network;

// ---------------------------------------------------------
// 4) Output & Action gates (continuous WTA, bound-crossing)
// ---------------------------------------------------------
public sealed class Gate {
	private readonly float[] _evidence;
	private readonly float[] _bias;       // same length; decays separately if you implemented that
	private readonly Random _rng = new Random( 1337 );

	private readonly float _tau;          // evidence time constant (s)
	public float Bound { get; private set; }
	public float Margin { get; private set; }
	public float DwellSec { get; private set; }

	// Common-mode suppression and tiny symmetry breaking
	public float NormLambda { get; set; } = 0.90f;   // fraction of mean to subtract (0..1)
	public float NoiseAmp { get; set; } = 0.0015f; // small jitter

	private float _lastDecisionSec = -1f;

	public Gate( int nGroups, float tau, float bound, float margin, float dwellSec ) {
		_evidence = new float[ nGroups ];
		_bias = new float[ nGroups ];
		_tau = tau;
		Bound = bound;
		Margin = margin;
		DwellSec = dwellSec;
	}

	public void AddBias( int i, float b ) => _bias[ i ] += b;
	public void AddEvidence( int i, float x ) => _evidence[ i ] += x;

	public void Decay( float dt ) {
		float k = MathF.Exp( -dt / _tau );
		for (int i = 0; i < _evidence.Length; i++)
			_evidence[ i ] *= k;
		// If you also decay bias, do it here with a slower tauBias
	}

	// <- THIS is the right place
	private void NormalizeAndJitter() {
		if (NormLambda > 0f) {
			// Use the mean of EVIDENCE ONLY; do NOT include bias here.
			float mean = 0f;
			for (int i = 0; i < _evidence.Length; i++)
				mean += _evidence[ i ];
			mean /= Math.Max( 1, _evidence.Length );

			float sub = NormLambda * mean;
			if (sub != 0f) {
				for (int i = 0; i < _evidence.Length; i++) {
					float v = _evidence[ i ] - sub;
					_evidence[ i ] = v > 0f ? v : 0f; // clamp at 0 to keep bound logic intuitive
				}
			}
		}

		if (NoiseAmp > 0f) {
			for (int i = 0; i < _evidence.Length; i++) {
				float u = (float) _rng.NextDouble() * 2f - 1f; // [-1, +1]
				_evidence[ i ] += u * NoiseAmp;
			}
		}
	}

	public (int index, float best, float second) PeekTop() {
		int bestIdx = 0;
		float best = _evidence[ 0 ] + _bias[ 0 ];
		float second = float.NegativeInfinity;
		for (int i = 1; i < _evidence.Length; i++) {
			float v = _evidence[ i ] + _bias[ i ];
			if (v > best) { second = best; best = v; bestIdx = i; } else if (v > second) { second = v; }
		}
		return (bestIdx, best, second);
	}

	public bool TryDecide( float timeSec, float dt, out int winner, out float value ) {
		// Assume caller already added this tick’s drive and called Decay(dt).
		NormalizeAndJitter();  // <-- always run right before decision

		// Evaluate winner after bias (bias is not normalized out)
		int bi = 0;
		float best = _evidence[ 0 ] + _bias[ 0 ];
		float second = float.NegativeInfinity;
		for (int i = 1; i < _evidence.Length; i++) {
			float v = _evidence[ i ] + _bias[ i ];
			if (v > best) { second = best; best = v; bi = i; } else if (v > second) { second = v; }
		}

		bool overBound = best >= Bound;
		bool gapOk = (best - second) >= Margin;
		bool dwellOk = (timeSec - _lastDecisionSec) >= DwellSec;

		if (overBound && gapOk && dwellOk) {
			winner = bi;
			value = best;
			_lastDecisionSec = timeSec;
			return true;
		}
		winner = -1;
		value = 0f;
		return false;
	}
}
