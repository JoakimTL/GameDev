using System.Numerics;

namespace Engine.Data.Bounds;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct AABB2 {
	[System.Runtime.InteropServices.FieldOffset( 0 )]
	private Vector2 _min;
	[System.Runtime.InteropServices.FieldOffset( 8 )]
	private Vector2 _max;

	public AABB2( Vector2 a, Vector2 b ) {
		this._min = Vector2.Min( a, b );
		this._max = Vector2.Max( a, b );
	}

	public AABB2( Vector2[] vecs ) {
		if ( vecs.Length == 0 )
			throw new ArgumentException( $"Length of {nameof( vecs )} must be greater than zero!" );
		this._min = this._max = vecs[ 0 ];
		for ( int i = 1; i < vecs.Length; i++ )
			Add( vecs[ i ] );
	}

	/// <summary>
	/// Extends the AABB
	/// </summary>
	public void Add( Vector2 v ) {
		this._min = Vector2.Min( this._min, v );
		this._max = Vector2.Max( this._max, v );
	}

	/// <summary>
	/// Resets the AABB
	/// </summary>
	public void Set( Vector2 a, Vector2 b ) {
		this._min = Vector2.Min( a, b );
		this._max = Vector2.Max( a, b );
	}

	public Vector2 Min => this._min;
	public Vector2 Max => this._max;
}

