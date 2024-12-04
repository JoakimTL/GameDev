using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public readonly struct Edge2<TScalar>( Vector2<TScalar> a, Vector2<TScalar> b ) : IEqualityComparer<Edge2<TScalar>>
	where TScalar : unmanaged, INumber<TScalar> {
	public readonly Vector2<TScalar> A = a;
	public readonly Vector2<TScalar> B = b;

	public bool HasVertex( Vector2<TScalar> p ) => A == p || B == p;

	public override bool Equals( [NotNullWhen( true )] object? obj ) {
		if (obj is Edge2<TScalar> edge)
			return Equals( this, edge );
		return base.Equals( obj );
	}

	public bool Equals( Edge2<TScalar> x, Edge2<TScalar> y ) => x.HasVertex( y.A ) && x.HasVertex( y.B );

	public int GetHashCode( [DisallowNull] Edge2<TScalar> obj ) => HashCode.Combine( obj.A, obj.B );

	public override string ToString() => $"[{A} -> {B}]";

	public bool IsPointOnEdge( Vector2<TScalar> point ) {
		TScalar crossProduct = (point.Y - A.Y) * (B.X - A.X) - (point.X - A.X) * (B.Y - A.Y);
		if (crossProduct > TScalar.Zero)
			return false;

		TScalar dotProduct = (point.X - A.X) * (B.X - A.X) + (point.Y - A.Y) * (B.Y - A.Y);
		if (dotProduct < TScalar.Zero)
			return false;

		TScalar squaredLength = (B - A).MagnitudeSquared();
		return dotProduct <= squaredLength;
	}

	// The main function that returns true if line segment 'p1q1' and 'p2q2' intersect. 
	public bool Intersects( Edge2<TScalar> o ) {
		if (HasVertex(o.A) || HasVertex(o.B ))
			return false;
		// Find the four orientations needed for general and 
		// special cases 
		int o1 = Orientation( this, o.A );
		int o2 = Orientation( this, o.B );
		int o3 = Orientation( o, this.A );
		int o4 = Orientation( o, this.B );

		// General case 
		if (o1 != o2 && o1 != 0 && o2 != 0 && o3 != o4 && o3 != 0 && o4 != 0)
			return true;

		// Special Cases 
		// p1, q1 and p2 are collinear and p2 lies on segment p1q1 
		if (o1 == 0 && OnSegment( A, o.A, B ))
			return false;

		// p1, q1 and q2 are collinear and q2 lies on segment p1q1 
		if (o2 == 0 && OnSegment( A, o.B, B ))
			return false;

		// p2, q2 and p1 are collinear and p1 lies on segment p2q2 
		if (o3 == 0 && OnSegment( o.A, A, o.B ))
			return false;

		// p2, q2 and q1 are collinear and q1 lies on segment p2q2 
		if (o4 == 0 && OnSegment( o.A, B, o.B ))
			return false;

		return false; // Doesn't fall in any of the above cases 
	}

	/// <returns>0 if collinear, -1 for counterclockwise and 1 for clockwise</returns>
	private int Orientation( Edge2<TScalar> e, Vector2<TScalar> p ) => Orientation( e.A, e.B, p );

	/// <returns>0 if collinear, -1 for counterclockwise and 1 for clockwise</returns>
	/// // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
	private int Orientation( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> p ) {
		var aa = (b.Y - a.Y) * (p.X - b.X);
		var bb = (b.X - a.X) * (p.Y - b.Y);
		var c = aa - bb;
		return TScalar.Sign( ((b.Y - a.Y) * (p.X - b.X)) - ((b.X - a.X) * (p.Y - b.Y)) );
	}

	private bool OnSegment( Edge2<TScalar> e, Vector2<TScalar> p )
		=> OnSegment( e.A, e.B, p );

	private bool OnSegment( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> p )
		=> b.X <= TScalar.Max( a.X, p.X ) && b.X >= TScalar.Min( a.X, p.X ) && b.Y <= TScalar.Max( a.Y, p.Y ) && b.Y >= TScalar.Min( a.Y, p.Y );
}
