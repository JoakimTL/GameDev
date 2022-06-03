using System.Numerics;
using Engine.Data.Datatypes;

namespace TestPlatform.Voxels.World;

public class DistanceField2MapGenerator {

	public const float OneOverUintMax = 1f / uint.MaxValue;

	public readonly uint Seed;
	private readonly Vector2 _spread;

	public DistanceField2MapGenerator( uint seed, Vector2 spread ) {
		this.Seed = seed;
		this._spread = spread;
	}

	public float GetValue( Vector2 point ) {
		Vector2 pointInGenSpace = point / this._spread;
		Vector2i low = Vector2i.Floor( pointInGenSpace );
		Vector2 frac = pointInGenSpace - low.AsFloat;

		Span<float> dataValues = stackalloc float[ 4 ];
		GetDataValues( low, ref dataValues );
		return GetValueInterpolated( dataValues, frac );
	}

	private float GetValueInterpolated( Span<float> dataValues, Vector2 frac ) {
		float x0 = Interpolate( dataValues[ 0 ], dataValues[ 1 ], frac.X );
		float x1 = Interpolate( dataValues[ 2 ], dataValues[ 3 ], frac.X );
		return Interpolate( x0, x1, frac.Y );
	}

	private float Interpolate( float a, float b, float p ) => ( b * p ) + ( a * ( 1 - p ) );

	private void GetDataValues( Vector2i low, ref Span<float> dataValues ) {
		Span<Vector2i> points = stackalloc Vector2i[ 8 ];
		points[ 0 ] = low;
		points[ 1 ] = low + new Vector2i( 1, 0 );
		points[ 2 ] = low + new Vector2i( 0, 1 );
		points[ 3 ] = low + new Vector2i( 1, 1 );
		SetDataValues( points, dataValues );
	}

	private void SetDataValues( Span<Vector2i> points, Span<float> dataValues ) {
		for ( int i = 0; i < dataValues.Length; i++ ) {
			dataValues[ i ] = GetDataValue( points[ i ] );
		}
	}

	public float GetDataValue( Vector2i p ) {
		unchecked {
			uint v = 42595009 + this.Seed;
			v *= 40631027;
			v ^= (uint) ( ( p.X * 4919 ) + ( p.Y * 5879 ) );
			v *= 40014307;
			v ^= (uint) ( ( p.X * 5237 ) + ( p.Y * 6823 ) );
			return v * OneOverUintMax;
		}
	}
}
