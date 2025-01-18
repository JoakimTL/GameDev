namespace Sandbox.Logic.OldWorld.Tiles.Generation;

public class Gradient3Noise( Vector3<int> seed, Vector3<float> phase, Vector3<float> spread ) {
	private readonly Vector3<int> _seed = seed;
	private readonly Vector3<float> _phase = phase;
	private readonly Vector3<float> _spread = spread;

	public float Sample( Vector3<float> input ) {
		Vector3<float> pointInGenSpace = (input + _phase).DivideEntrywise( _spread ) - _phase;
		Vector3<int> low = pointInGenSpace.Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> frac = pointInGenSpace - low.CastSaturating<int, float>();

		Span<Vector3<int>> relativePoints = [
			(0, 0, 0), (0, 0, 1), (0, 1, 0), (0, 1, 1),
			(1, 0, 0), (1, 0, 1), (1, 1, 0), (1, 1, 1)
		];

		Span<float> dataValues = stackalloc float[ 8 ];
		GetDataValues( low, relativePoints, dataValues );
		return GetValue( relativePoints, dataValues, frac );
	}
	// returns 3D value noise
	//float noise( in vec3 x ) {
	//	// grid
	//	vec3 p = floor( x );
	//	vec3 w = fract( x );

	//	// quintic interpolant
	//	vec3 u = w * w * w * (w * (w * 6.0 - 15.0) + 10.0);


	//	// gradients
	//	vec3 ga = hash( p + vec3( 0.0, 0.0, 0.0 ) );
	//	vec3 gb = hash( p + vec3( 1.0, 0.0, 0.0 ) );
	//	vec3 gc = hash( p + vec3( 0.0, 1.0, 0.0 ) );
	//	vec3 gd = hash( p + vec3( 1.0, 1.0, 0.0 ) );
	//	vec3 ge = hash( p + vec3( 0.0, 0.0, 1.0 ) );
	//	vec3 gf = hash( p + vec3( 1.0, 0.0, 1.0 ) );
	//	vec3 gg = hash( p + vec3( 0.0, 1.0, 1.0 ) );
	//	vec3 gh = hash( p + vec3( 1.0, 1.0, 1.0 ) );

	//	// projections
	//	float va = dot( ga, w - vec3( 0.0, 0.0, 0.0 ) );
	//	float vb = dot( gb, w - vec3( 1.0, 0.0, 0.0 ) );
	//	float vc = dot( gc, w - vec3( 0.0, 1.0, 0.0 ) );
	//	float vd = dot( gd, w - vec3( 1.0, 1.0, 0.0 ) );
	//	float ve = dot( ge, w - vec3( 0.0, 0.0, 1.0 ) );
	//	float vf = dot( gf, w - vec3( 1.0, 0.0, 1.0 ) );
	//	float vg = dot( gg, w - vec3( 0.0, 1.0, 1.0 ) );
	//	float vh = dot( gh, w - vec3( 1.0, 1.0, 1.0 ) );

	//	// interpolation
	//	return va +
	//		   u.x * (vb - va) +
	//		   u.y * (vc - va) +
	//		   u.z * (ve - va) +
	//		   u.x * u.y * (va - vb - vc + vd) +
	//		   u.y * u.z * (va - vc - ve + vg) +
	//		   u.z * u.x * (va - vb - ve + vf) +
	//		   u.x * u.y * u.z * (-va + vb + vc - vd + ve - vf - vg + vh);
	//}
	private static float GetValue( Span<Vector3<int>> relativePoints, Span<float> dataValues, Vector3<float> frac ) {
		//(0, 0, 0), (0, 0, 1), (0, 1, 0), (0, 1, 1),
		//(1, 0, 0), (1, 0, 1), (1, 1, 0), (1, 1, 1)
		float sum = 0;
		float weightTotal = 0;
		for (int i = 0; i < relativePoints.Length; i++) {
			float dist = float.Min( (frac - relativePoints[ i ].CastSaturating<int, float>()).MagnitudeSquared(), 1 );
			float bias = 1 - dist;
			sum += bias * dataValues[ i ];
			weightTotal += bias;
		}
		return sum / weightTotal;
	}

	private void GetDataValues( Vector3<int> low, Span<Vector3<int>> relativePoints, Span<float> dataValues ) {
		for (int i = 0; i < dataValues.Length; i++)
			dataValues[ i ] = GetData( low + relativePoints[ i ] );
	}

	public float GetData( Vector3<int> p )
		=> (
			GetDataValue( p.X + p.Y * p.Z, _seed.X ) +
			GetDataValue( p.Y + p.Z * p.X, _seed.Y ) +
			GetDataValue( p.Z + p.X * p.Y, _seed.Z )
		) / 3;

	public static float GetDataValue( int v, int seed ) {
		unchecked {
			uint x = Hash( (uint) v ) + Hash( (uint) seed );
			x += x << 10;
			x ^= x >> 6;
			x += x << 3;
			x ^= x >> 11;
			x += x << 15;
			return (int) x * (1f / int.MaxValue);

			//x ^= x >> 17;
			//x *= 0xed5ad4bbU;
			//x ^= x >> 11;
			//x *= 0xac4c1b51U;
			//x ^= x >> 15;
			//x *= 0x31848babU;
			//x ^= x >> 14;
			//return (int) x * (1f / int.MaxValue);

			//uint state = ((uint) p + seed) * 747796405u + 2891336453u;
			//uint word = ((state >> (int) ((state >> 28) + 4u)) ^ state) * 277803737u;
			//return ((word >> 22) ^ word) * (1f / uint.MaxValue);

			//uint v = 42595009 + seed;
			//v *= 40631027;
			//v ^= (uint) (p * 4919);
			//v *= 40014307;
			//v ^= (uint) (p * 5237);
			//return v * (1f / uint.MaxValue);
		}
	}

	public static uint Hash( uint input ) {
		unchecked {
			uint x = input;
			x += x << 10;
			x ^= x >> 6;
			x += x << 3;
			x ^= x >> 11;
			x += x << 15;
			return x;
		}
	}
}