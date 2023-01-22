using System.Diagnostics.CodeAnalysis;

namespace Engine.Datatypes.Vectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct AABB2i {
	[System.Runtime.InteropServices.FieldOffset( 0 )]
	private Vector2i _min;
	[System.Runtime.InteropServices.FieldOffset( 8 )]
	private Vector2i _max;

	public uint AreaExclusive => (uint) ( ( _max.X - _min.X ) * ( _max.Y - _min.Y ) );
	public uint AreaInclusive => (uint) ( ( _max.X - _min.X + 1 ) * ( _max.Y - _min.Y + 1 ) );

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

	public static bool Intersects( ref AABB2i a, ref AABB2i b ) =>
		a._min.X <= b._max.X && a._max.X >= b._min.X &&
		a._min.Y <= b._max.Y && a._max.Y >= b._min.Y;

	public static bool Inside( ref AABB2i a, ref Vector2i b ) =>
		a._min.X <= b.X && a._max.X >= b.X &&
		a._min.Y <= b.Y && a._max.Y >= b.Y;

	public static AABB2i GetLargestArea( AABB2i a, AABB2i b ) => new( Vector2i.Min( a.Min, b.Min ), Vector2i.Max( a.Max, b.Max ) );
	public static AABB2i GetSmallestArea( AABB2i a, AABB2i b ) => new( Vector2i.Max( a.Min, b.Min ), Vector2i.Min( a.Max, b.Max ) );

	public IEnumerable<Vector2i> GetPointsInAreaExclusive() {
		for ( int y = _min.Y; y < _max.Y; y++ )
			for ( int x = _min.X; x < _max.X; x++ )
				yield return new( x, y );
	}

	public IEnumerable<Vector2i> GetPointsInAreaInclusive() {
		for ( int y = _min.Y; y <= _max.Y; y++ )
			for ( int x = _min.X; x <= _max.X; x++ )
				yield return new( x, y );
	}

	public IEnumerable<Vector2i> ExceptInclusive( AABB2i area ) {
		for ( int y = _min.Y; y <= _max.Y; y++ )
			for ( int x = _min.X; x <= _max.X; x++ ) {
				var v = new Vector2i( x, y );
				if ( !Inside( ref area, ref v ) ) {
					yield return v;
				} else {
					x += area._max.X - x;
				}
			}
	}

	public IEnumerable<Vector2i> ExceptExclusive( AABB2i area ) {
		for ( int y = _min.Y; y < _max.Y; y++ )
			for ( int x = _min.X; x < _max.X; x++ ) {
				var v = new Vector2i( x, y );
				if ( !Inside( ref area, ref v ) ) {
					yield return v;
				} else {
					x += area._max.X - x;
				}
			}
	}

	public Vector2i Min => _min;
	public Vector2i Max => _max;

	public override string ToString()
		=> $"{Min} -> {Max}";

	public override bool Equals( [NotNullWhen( true )] object? obj )
		=> obj is AABB2i aabb && Equals( aabb );

	public override int GetHashCode()
		=> HashCode.Combine( _min, _max );

	public bool Equals( AABB2i other )
		=> other.Min == Min && other.Max == Max;

	public static bool operator ==( AABB2i left, AABB2i right )
		=> left.Equals( right );

	public static bool operator !=( AABB2i left, AABB2i right )
		=> !left.Equals( right );
}

