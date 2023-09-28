using System.Numerics;

namespace Engine.Data.Bounds;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct AABB4 {
	[System.Runtime.InteropServices.FieldOffset( 0 )]
	private Vector4 _min;
	[System.Runtime.InteropServices.FieldOffset( 16 )]
	private Vector4 _max;

	public AABB4( Vector4 a, Vector4 b ) {
		this._min = Vector4.Min( a, b );
		this._max = Vector4.Max( a, b );
	}

	public AABB4( Vector4[] vecs ) {
		if ( vecs.Length == 0 )
			throw new ArgumentException( $"Length of {nameof( vecs )} must be greater than zero!" );
		this._min = this._max = vecs[ 0 ];
		for ( int i = 1; i < vecs.Length; i++ )
			Add( vecs[ i ] );
	}

	/// <summary>
	/// Extends the AABB
	/// </summary>
	public void Add( Vector4 v ) {
		this._min = Vector4.Min( this._min, v );
		this._max = Vector4.Max( this._max, v );
	}

	/// <summary>
	/// Resets the AABB
	/// </summary>
	public void Set( Vector4 a, Vector4 b ) {
		this._min = Vector4.Min( a, b );
		this._max = Vector4.Max( a, b );
	}

	public Vector4 Min => this._min;
	public Vector4 Max => this._max;
}

