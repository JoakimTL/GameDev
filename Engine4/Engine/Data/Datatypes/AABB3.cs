using System.Numerics;

namespace Engine.Data.Datatypes;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct AABB3 {
	[System.Runtime.InteropServices.FieldOffset( 0 )]
	private Vector3 _min;
	[System.Runtime.InteropServices.FieldOffset( 12 )]
	private Vector3 _max;

	public AABB3( Vector3 a, Vector3 b ) {
		this._min = Vector3.Min( a, b );
		this._max = Vector3.Max( a, b );
	}

	public AABB3( Vector3[] vecs ) {
		if ( vecs.Length == 0 )
			throw new ArgumentException( $"Length of {nameof( vecs )} must be greater than zero!" );
		this._min = this._max = vecs[ 0 ];
		for ( int i = 1; i < vecs.Length; i++ ) {
			Add( vecs[ i ] );
		}
	}

	/// <summary>
	/// Extends the AABB
	/// </summary>
	public void Add( Vector3 v ) {
		this._min = Vector3.Min( this._min, v );
		this._max = Vector3.Max( this._max, v );
	}

	/// <summary>
	/// Resets the AABB
	/// </summary>
	public void Set( Vector3 a, Vector3 b ) {
		this._min = Vector3.Min( a, b );
		this._max = Vector3.Max( a, b );
	}

	public Vector3 Min => this._min;
	public Vector3 Max => this._max;
}

