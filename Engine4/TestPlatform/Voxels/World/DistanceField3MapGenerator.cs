using System.Numerics;
using Engine.Data.Datatypes;

namespace TestPlatform.Voxels.World;
public class DistanceField3MapGenerator {

	public const float OneOverUintMax = 1f / uint.MaxValue;

	public readonly uint Seed;
	private readonly Vector3 _spread;

	public DistanceField3MapGenerator( uint seed, Vector3 spread ) {
		this.Seed = seed;
		this._spread = spread;
	}

	public float GetValue( Vector3 point ) {
		Vector3 pointInGenSpace = point / this._spread;
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

	private float Interpolate( float a, float b, float p ) => (b * p) + (a * ( 1 - p ));

	private void GetDataValues( Vector3i low, ref Span<float> dataValues ) {
		Span<Vector3i> points = stackalloc Vector3i[ 8 ];
		points[ 0 ] = low;
		points[ 1 ] = low + new Vector3i( 1, 0, 0 );
		points[ 2 ] = low + new Vector3i( 0, 0, 1 );
		points[ 3 ] = low + new Vector3i( 1, 0, 1 );
		points[ 4 ] = low + new Vector3i( 0, 1, 0 );
		points[ 5 ] = low + new Vector3i( 1, 1, 0 );
		points[ 6 ] = low + new Vector3i( 0, 1, 1 );
		points[ 7 ] = low + new Vector3i( 1, 1, 1 );
		SetDataValues( points, dataValues );
	}

	private void SetDataValues( Span<Vector3i> points, Span<float> dataValues ) {
		for ( int i = 0; i < dataValues.Length; i++ ) {
			dataValues[ i ] = GetDataValue( points[ i ] );
		}
	}

	public float GetDataValue( Vector3i p ) {
		unchecked {
			uint v = 42595009 + this.Seed;
			v *= 40631027;
			v ^= (uint) ( ( p.X * 4919 ) + ( p.Y * 5879 ) + ( p.Z * 6151 ) );
			v *= 40014307;
			v ^= (uint) ( ( p.X * 5237 ) + ( p.Y * 6823 ) + ( p.Z * 4603 ) );
			return v * OneOverUintMax;
		}
	}
}
