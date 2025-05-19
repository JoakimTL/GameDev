using Engine;

namespace Civlike.World.Generation;
public class Noise3 {

	public float Scale { get; set; } = 1.0f;
	public uint Seed { get; set; } = 0;

	public Noise3( int seed, float scale ) {
		this.Seed = unchecked((uint) seed);
		this.Scale = scale;
	}

	public Noise3( uint seed, float scale ) {
		this.Seed = seed;
		this.Scale = scale;
	}

	public float Noise( Vector3<float> xyz ) => Noise( Seed, Scale, xyz );

	public static float Noise( uint seed, float scale, Vector3<float> xyz ) {
		Vector3<float> scaledXyz = xyz * scale;
		Vector3<int> low = scaledXyz.Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> frac = scaledXyz - low.CastSaturating<int, float>();
		Span<Vector3<int>> hashPoints = [
			low + (0, 0, 0), low + (0, 0, 1), low + (0, 1, 0), low + (0, 1, 1),
			low + (1, 0, 0), low + (1, 0, 1), low + (1, 1, 0), low + (1, 1, 1)
		];

		Span<float> dataValues = stackalloc float[ 8 ];
		for (int i = 0; i < 8; i++) {
			Vector3<int> point = hashPoints[ i ];
			dataValues[ i ] = Hash4( point.X, point.Y, point.Z, seed );
		}

		float xInterp = float.Cos( frac.X * MathF.PI ) * 0.5f + 0.5f;
		float yInterp = float.Cos( frac.Y * MathF.PI ) * 0.5f + 0.5f;
		float zInterp = float.Cos( frac.Z * MathF.PI ) * 0.5f + 0.5f;

		Span<float> sumsZ = stackalloc float[ 4 ];
		for (int i = 0; i < sumsZ.Length; i++)
			sumsZ[ i ] = zInterp * dataValues[ i * 2 ] + (1 - zInterp) * dataValues[ i * 2 + 1 ];

		Span<float> sumsY = stackalloc float[ 2 ];
		for (int i = 0; i < sumsY.Length; i++)
			sumsY[ i ] = yInterp * sumsZ[ i * 2 ] + (1 - yInterp) * sumsZ[ i * 2 + 1 ];

		return xInterp * sumsY[ 0 ] + (1 - xInterp) * sumsY[ 1 ];
	}

	public static float Hash4( int x, int y, int z, uint seed ) {
		return Hash( unchecked(seed + (uint) x * 19 + (uint) y * 47 + (uint) z * 101) ) / (float) uint.MaxValue;
	}

	public static uint Hash( uint input ) {
		unchecked {
			uint x = input;
			x ^= x >> 16;
			x *= 0x85ebca6bu;
			x ^= x << 16;
			x *= 0xc2b2ae35u;
			return x;
		}
	}

}
