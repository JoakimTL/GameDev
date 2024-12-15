namespace Sandbox.Logic.World;

public class Gradient3Noise( Vector3<int> seed, Vector3<float> spread ) {
	private readonly Vector3<int> _seed = seed;
	private readonly Vector3<float> _spread = spread;

	public float Sample( Vector3<float> input ) {
		Vector3<float> pointInGenSpace = input.DivideEntrywise( _spread );
		Vector3<int> low = pointInGenSpace.Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> frac = pointInGenSpace - low.CastTruncating<int, float>();

		Span<Vector3<int>> relativePoints = [
			0,
			(0, 0, 1),
			(0, 1, 0),
			(0, 1, 1),
			(1, 0, 0),
			(1, 0, 1),
			(1, 1, 0),
			(1, 1, 1)
		];
		Span<Vector3<float>> dataValues = stackalloc Vector3<float>[ 8 ];
		GetDataValues( low, relativePoints, dataValues );
		return GetValue( relativePoints, dataValues, frac );
	}

	private static float GetValue( Span<Vector3<int>> relativePoints, Span<Vector3<float>> dataValues, Vector3<float> frac ) {
		float val = 0;
		for (int i = 0; i < relativePoints.Length; i++)
			val += (relativePoints[ i ].CastTruncating<int, float>() - frac).Dot( dataValues[ i ] );
		val /= relativePoints.Length;
		return val;
	}

	private void GetDataValues( Vector3<int> low, Span<Vector3<int>> relativePoints, Span<Vector3<float>> dataValues ) {
		for (int i = 0; i < dataValues.Length; i++)
			dataValues[ i ] = GetData( low + relativePoints[ i ] );
	}

	public Vector3<float> GetData( Vector3<int> p )
		=> new Vector3<float>(
				(GetDataValue( p, unchecked((uint) _seed.X) ) * 2) - 1,
				(GetDataValue( p, unchecked((uint) _seed.Y) ) * 2) - 1,
				(GetDataValue( p, unchecked((uint) _seed.Z) ) * 2) - 1
			).Normalize<Vector3<float>, float>();

	public static float GetDataValue( Vector3<int> p, uint seed ) {
		unchecked {
			uint v = 42595009 + seed;
			v *= 40631027;
			v ^= (uint) ((p.X * 4919) + (p.Y * 5879) + (p.Z * 6151));
			v *= 40014307;
			v ^= (uint) ((p.X * 5237) + (p.Y * 6823) + (p.Z * 4603));
			return v * (1f / uint.MaxValue);
		}
	}
}
