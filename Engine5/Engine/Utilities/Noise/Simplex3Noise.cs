using Engine.Datatypes.Vectors;
using System.Numerics;

namespace Engine.Utilities.Noise;

public class Simplex3Noise : INoiseProvider<Vector3, float> {
	private readonly uint _seed;
	private readonly Vector3 _spread;

	public Simplex3Noise( uint seed, Vector3 spread ) {
		this._seed = seed;
		_spread = spread;
	}

	public float Sample( Vector3 input ) {
		Vector3 pointInGenSpace = input / this._spread;
		Vector3i low = Vector3i.Floor( pointInGenSpace );
		Vector3 frac = pointInGenSpace - low.AsFloat;

		Span<float> dataValues = stackalloc float[ 8 ];
		GetDataValues( low, ref dataValues );
		return GetValueInterpolated( dataValues, frac );
	}

	private float GetValueInterpolated( Span<float> dataValues, Vector3 frac ) {
		float x0 = Interpolate( dataValues[ 0 ], dataValues[ 1 ], frac.X );
		float x1 = Interpolate( dataValues[ 2 ], dataValues[ 3 ], frac.X );
		float x2 = Interpolate( dataValues[ 4 ], dataValues[ 5 ], frac.X );
		float x3 = Interpolate( dataValues[ 6 ], dataValues[ 7 ], frac.X );
		float z0 = Interpolate( x0, x1, frac.Z );
		float z1 = Interpolate( x2, x3, frac.Z );
		return Interpolate( z0, z1, frac.Y );
	}

	private float Interpolate( float a, float b, float p ) => ( b * p ) + ( a * ( 1 - p ) );

	private void GetDataValues( Vector3i low, ref Span<float> dataValues ) {
		Span<Vector3i> points = stackalloc[] {
			low,
			low + new Vector3i( 1, 0, 0 ),
			low + new Vector3i( 1, 0, 0 ),
			low + new Vector3i( 0, 0, 1 ),
			low + new Vector3i( 1, 0, 1 ),
			low + new Vector3i( 0, 1, 0 ),
			low + new Vector3i( 1, 1, 0 ),
			low + new Vector3i( 0, 1, 1 ),
			low + new Vector3i( 1, 1, 1 )
		};
		for ( int i = 0; i < dataValues.Length; i++ )
			dataValues[ i ] = GetDataValue( points[ i ] );
	}

	public float GetDataValue( Vector3i p ) {
		unchecked {
			uint v = 42595009 + this._seed;
			v *= 40631027;
			v ^= (uint) ( ( p.X * 4919 ) + ( p.Y * 5879 ) + ( p.Z * 6151 ) );
			v *= 40014307;
			v ^= (uint) ( ( p.X * 5237 ) + ( p.Y * 6823 ) + ( p.Z * 4603 ) );
			return v * Constants.OneOverUintMax;
		}
	}
}
