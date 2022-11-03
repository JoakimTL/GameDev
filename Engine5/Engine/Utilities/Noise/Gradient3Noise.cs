using Engine.Datatypes;
using System.Numerics;

namespace Engine.Utilities.Noise;

public class Gradient3Noise : INoiseProvider<Vector3, float> {
	private readonly Vector3i _seed;
	private readonly Vector3 _spread;

	public Gradient3Noise( Vector3i seed, Vector3 spread ) {
		this._seed = seed;
		_spread = spread;
	}

	public float Sample( Vector3 input ) {
		Vector3 pointInGenSpace = input / this._spread;
		Vector3i low = Vector3i.Floor( pointInGenSpace );
		Vector3 frac = pointInGenSpace - low.AsFloat;

		Span<Vector3i> relativePoints = stackalloc[] {
			0,
			new Vector3i( 1, 0, 0 ),
			new Vector3i( 1, 0, 0 ),
			new Vector3i( 0, 0, 1 ),
			new Vector3i( 1, 0, 1 ),
			new Vector3i( 0, 1, 0 ),
			new Vector3i( 1, 1, 0 ),
			new Vector3i( 0, 1, 1 ),
			new Vector3i( 1, 1, 1 )
		};
		Span<Vector3> dataValues = stackalloc Vector3[ 8 ];
		GetDataValues( low, relativePoints, ref dataValues );
		return GetValue( relativePoints, dataValues, frac );
	}

	private float GetValue( Span<Vector3i> relativePoints, Span<Vector3> dataValues, Vector3 frac ) {
		float val = 0;
		for ( int i = 0; i < relativePoints.Length; i++ )
			val += Vector3.Dot( relativePoints[ i ].AsFloat - frac, dataValues[ i ] );
		val /= relativePoints.Length;
		return val;
	}

	private void GetDataValues( Vector3i low, Span<Vector3i> relativePoints, ref Span<Vector3> dataValues ) {
		for ( int i = 0; i < dataValues.Length; i++ )
			dataValues[ i ] = GetData( low + relativePoints[ i ] );
	}

	public Vector3 GetData( Vector3i p )
		=> Vector3.Normalize(
			new(
				GetDataValue( p, unchecked((uint) _seed.X) ) * 2 - 1,
				GetDataValue( p, unchecked((uint) _seed.Y) ) * 2 - 1,
				GetDataValue( p, unchecked((uint) _seed.Z) ) * 2 - 1
			)
		);

	public float GetDataValue( Vector3i p, uint seed ) {
		unchecked {
			uint v = 42595009 + seed;
			v *= 40631027;
			v ^= (uint) ( ( p.X * 4919 ) + ( p.Y * 5879 ) + ( p.Z * 6151 ) );
			v *= 40014307;
			v ^= (uint) ( ( p.X * 5237 ) + ( p.Y * 6823 ) + ( p.Z * 4603 ) );
			return v * Constants.OneOverUintMax;
		}
	}
}
