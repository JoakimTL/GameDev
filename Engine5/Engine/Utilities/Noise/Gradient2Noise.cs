using Engine.Datatypes;
using System.Numerics;

namespace Engine.Utilities.Noise;

public class Gradient2Noise : INoiseProvider<Vector2, float> {
	private readonly Vector2i _seed;
	private readonly Vector2 _spread;

	public Gradient2Noise( Vector2i seed, Vector2 spread ) {
		this._seed = seed;
		_spread = spread;
	}

	public float Sample( Vector2 input ) {
		Vector2 pointInGenSpace = input / this._spread;
		Vector2i low = Vector2i.Floor( pointInGenSpace );
		Vector2 frac = pointInGenSpace - low.AsFloat;

		Span<Vector2i> relativePoints = stackalloc[] {
			0,
			new Vector2i( 1, 0 ),
			new Vector2i( 0, 1 ),
			new Vector2i( 1, 1 )
		};
		Span<Vector2> dataValues = stackalloc Vector2[ 4 ];
		GetDataValues( low, relativePoints, ref dataValues );
		return GetValue( relativePoints, dataValues, frac );
	}

	private static float GetValue( Span<Vector2i> relativePoints, Span<Vector2> dataValues, Vector2 frac ) {
		float val = 0;
		for ( int i = 0; i < relativePoints.Length; i++ )
			val += Vector2.Dot( relativePoints[ i ].AsFloat - frac, dataValues[ i ] );
		val /= relativePoints.Length;
		return val;
	}

	private void GetDataValues( Vector2i low, Span<Vector2i> relativePoints, ref Span<Vector2> dataValues ) {
		for ( int i = 0; i < dataValues.Length; i++ )
			dataValues[ i ] = GetData( low + relativePoints[ i ] );
	}

	public Vector2 GetData( Vector2i p )
		=> Vector2.Normalize(
			new(
				GetDataValue( p, unchecked((uint) _seed.X) ) * 2 - 1,
				GetDataValue( p, unchecked((uint) _seed.Y) ) * 2 - 1
			)
		);

	public static float GetDataValue( Vector2i p, uint seed ) {
		unchecked {
			uint v = 42595009 + seed;
			v *= 40631027;
			v ^= (uint) ( ( p.X * 4919 ) + ( p.Y * 5879 ) );
			v *= 40014307;
			v ^= (uint) ( ( p.X * 5237 ) + ( p.Y * 6823 ) );
			return v * Constants.OneOverUintMax;
		}
	}
}