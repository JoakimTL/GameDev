namespace Sandbox.Logic.World.Tiles.Generation;

public class Gradient2Noise {
	private readonly Vector2<int> _seed;
	private readonly Vector2<float> _spread;

	public Gradient2Noise( Vector2<int> seed, Vector2<float> spread ) {
		_seed = seed;
		_spread = spread;
	}

	public float Sample( Vector2<float> input ) {
		Vector2<float> pointInGenSpace = input.DivideEntrywise( _spread );
		Vector2<int> low = pointInGenSpace.Floor<Vector2<float>, float>().CastSaturating<float, int>();
		Vector2<float> frac = pointInGenSpace - low.CastSaturating<int, float>();

		Span<Vector2<int>> relativePoints = [
			0,
			new Vector2<int>(1, 0),
			new Vector2<int>(0, 1),
			new Vector2<int>(1, 1)
		];
		Span<Vector2<float>> dataValues = stackalloc Vector2<float>[ 4 ];
		GetDataValues( low, relativePoints, ref dataValues );
		return GetValue( relativePoints, dataValues, frac );
	}

	private static float GetValue( Span<Vector2<int>> relativePoints, Span<Vector2<float>> dataValues, Vector2<float> frac ) {
		float val = 0;
		for (int i = 0; i < relativePoints.Length; i++)
			val += (relativePoints[ i ].CastSaturating<int, float>() - frac).Dot( dataValues[ i ] );
		val /= relativePoints.Length;
		return val;
	}

	private void GetDataValues( Vector2<int> low, Span<Vector2<int>> relativePoints, ref Span<Vector2<float>> dataValues ) {
		for (int i = 0; i < dataValues.Length; i++)
			dataValues[ i ] = GetData( low + relativePoints[ i ] );
	}

	public Vector2<float> GetData( Vector2<int> p )
		=> new Vector2<float>(
				GetDataValue( p, unchecked((uint) _seed.X) ) * 2 - 1,
				GetDataValue( p, unchecked((uint) _seed.Y) ) * 2 - 1
			).Normalize<Vector2<float>, float>();

	public static float GetDataValue( Vector2<int> p, uint seed ) {
		unchecked {
			uint v = 42595009 + seed;
			v *= 40631027;
			v ^= (uint) (p.X * 4919 + p.Y * 5879);
			v *= 40014307;
			v ^= (uint) (p.X * 5237 + p.Y * 6823);
			return v * (1f / uint.MaxValue);
		}
	}
}
