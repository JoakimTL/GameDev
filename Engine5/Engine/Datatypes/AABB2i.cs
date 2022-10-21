namespace Engine.Datatypes;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct AABB2i {
	[System.Runtime.InteropServices.FieldOffset( 0 )]
	private Vector2i _min;
	[System.Runtime.InteropServices.FieldOffset( 8 )]
	private Vector2i _max;

	public AABB2i( Vector2i a, Vector2i b ) {
		_min = Vector2i.Min( a, b );
		_max = Vector2i.Max( a, b );
	}

	public AABB2i( Vector2i[] vecs ) {
		if ( vecs.Length == 0 )
			throw new ArgumentException( $"Length of {nameof( vecs )} must be greater than zero!" );
		_min = _max = vecs[ 0 ];
		for ( int i = 1; i < vecs.Length; i++ )
			Add( vecs[ i ] );
	}

	/// <summary>
	/// Extends the AABB
	/// </summary>
	public void Add( Vector2i v ) {
		_min = Vector2i.Min( _min, v );
		_max = Vector2i.Max( _max, v );
	}

	/// <summary>
	/// Resets the AABB
	/// </summary>
	public void Set( Vector2i a, Vector2i b ) {
		_min = Vector2i.Min( a, b );
		_max = Vector2i.Max( a, b );
	}

	public Vector2i Min => _min;
	public Vector2i Max => _max;
}

