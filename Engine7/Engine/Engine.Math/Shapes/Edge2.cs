using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Shapes;

public readonly struct Edge2<TScalar> : IEqualityComparer<Edge2<TScalar>>
	where TScalar : unmanaged, INumber<TScalar> {
	public readonly Vector2<TScalar> A;
	public readonly Vector2<TScalar> B;

	public Edge2( Vector2<TScalar> a, Vector2<TScalar> b ) {
		this.A = a;
		this.B = b;
		if (this.A == this.B)
			throw new ArgumentException( "An edge cannot have the same vertex twice." );
	}

	public bool HasVertex( Vector2<TScalar> p ) => this.A == p || this.B == p;

	public override bool Equals( [NotNullWhen( true )] object? obj ) {
		if (obj is Edge2<TScalar> edge)
			return HasVertex( edge.A ) && HasVertex( edge.B );
		return base.Equals( obj );
	}

	public override int GetHashCode() => HashCode.Combine( this.A, this.B );

	public bool Equals( Edge2<TScalar> x, Edge2<TScalar> y ) => x.Equals( y );

	public int GetHashCode( [DisallowNull] Edge2<TScalar> obj ) => obj.GetHashCode();

	public override string ToString() => $"[{this.A} -> {this.B}]";

	public bool IsPointOnEdge( Vector2<TScalar> point ) {
		TScalar crossProduct = (point.Y - this.A.Y) * (this.B.X - this.A.X) - (point.X - this.A.X) * (this.B.Y - this.A.Y);
		if (crossProduct > TScalar.Zero)
			return false;

		TScalar dotProduct = (point.X - this.A.X) * (this.B.X - this.A.X) + (point.Y - this.A.Y) * (this.B.Y - this.A.Y);
		if (dotProduct < TScalar.Zero)
			return false;

		TScalar squaredLength = (this.B - this.A).MagnitudeSquared();
		return dotProduct <= squaredLength;
	}

	// The main function that returns true if line segment 'p1q1' and 'p2q2' intersect. 
	public bool Intersects( Edge2<TScalar> o ) {
		//http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/

		// Find the four orientations needed for general and 
		// special cases 
		int o1 = Orientation( this, o.A );
		int o2 = Orientation( this, o.B );
		int o3 = Orientation( o, this.A );
		int o4 = Orientation( o, this.B );

		// General case 
		if (o1 != o2 && o3 != o4)
			return true;

		// Special Cases 
		// p1, q1 and p2 are collinear and p2 lies on segment p1q1 
		if (o1 == 0 && OnSegment( this.A, this.B, o.A ) && !HasVertex( o.A ))
			return true;

		// p1, q1 and q2 are collinear and q2 lies on segment p1q1 
		if (o2 == 0 && OnSegment( this.A, this.B, o.B ) && !HasVertex( o.B ))
			return true;

		// p2, q2 and p1 are collinear and p1 lies on segment p2q2 
		if (o3 == 0 && OnSegment( o.A, o.B, this.A ) && !o.HasVertex( this.A ))
			return true;

		// p2, q2 and q1 are collinear and q1 lies on segment p2q2 
		if (o4 == 0 && OnSegment( o.A, o.B, this.B ) && !o.HasVertex( this.B ))
			return true;

		return false; // Doesn't fall in any of the above cases 
	}


	/// <returns>0 if collinear, -1 for counterclockwise and 1 for clockwise</returns>
	private static int Orientation( Edge2<TScalar> e, Vector2<TScalar> p ) => Orientation( e.A, e.B, p );

	/// <returns>0 if collinear, -1 for counterclockwise and 1 for clockwise</returns>
	/// // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
	private static int Orientation( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> p ) {
		TScalar aa = (b.Y - a.Y) * (p.X - b.X);
		TScalar bb = (b.X - a.X) * (p.Y - b.Y);
		TScalar result = aa - bb;
		if (TScalar.Abs( result ) < TScalar.CreateSaturating( 0.000001 ))
			return 0;
		return TScalar.Sign( result );
	}

	private static bool OnSegment( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> p )
		=> p.X <= TScalar.Max( a.X, b.X ) && p.X >= TScalar.Min( a.X, b.X )
		&& p.Y <= TScalar.Max( a.Y, b.Y ) && p.Y >= TScalar.Min( a.Y, b.Y );

	//public bool IntersectsRay( Vector2<TScalar> rayOrigin, Vector2<TScalar> rayDirection ) {
	//	Vector2<TScalar> v1 = rayOrigin - A;
	//	Vector2<TScalar> v2 = B - A;
	//	Vector2<TScalar> v3 = rayDirection * -Bivector2<TScalar>.One;

	//	TScalar dot = v2.Dot( v3 );
	//	if (TScalar.Abs( dot ) < TScalar.CreateSaturating(0.000001))
	//		return false;

	//	TScalar t1 = v2.Wedge( v1 ).XY / dot;
	//	TScalar t2 = v1.Dot( v3 ) / dot;

	//	if (t1 >= TScalar.Zero && (t2 >= TScalar.Zero && t2 <= TScalar.One))
	//		return true;

	//	return false;

	//}

	public IntersectionResult IntersectsRay( Vector2<TScalar> rayOrigin, Vector2<TScalar> rayDirection ) {
		Vector2<TScalar> localA = this.A - rayOrigin;
		Vector2<TScalar> localB = this.B - rayOrigin;

		if (localA == Vector2<TScalar>.Zero || localB == Vector2<TScalar>.Zero)
			return IntersectionResult.OnVertex; // One of the points is at the ray origin

		TScalar dotA = localA.Dot( rayDirection );
		TScalar dotB = localB.Dot( rayDirection );

		if (dotA < TScalar.Zero && dotB < TScalar.Zero)
			return IntersectionResult.NoIntersection; // Both points are behind the ray

		//if (dotA == TScalar.Zero && dotB == TScalar.Zero)
		//	return ???; // This case is handled later

		if (dotA == TScalar.Zero && dotB < TScalar.Zero || dotA < TScalar.Zero && dotB == TScalar.Zero)
			return IntersectionResult.NoIntersection; //The edge starts or ends behind the ray, and ends next to it. It can't be intersecting

		// Compute orientations for general and special cases
		int o1 = Orientation( rayOrigin, rayOrigin + rayDirection, A );
		int o2 = Orientation( rayOrigin, rayOrigin + rayDirection, B );

		if (o1 == 0 || o2 == 0)
			return IntersectionResult.OnVertex; // The ray intersects a vertex

		// General case: The ray intersects the segment if orientations differ
		if (o1 != o2)
			return IntersectionResult.Intersection;

		// Special cases (collinear points)
		if (o1 == 0 && OnRay( rayOrigin, rayDirection, A ))
			return IntersectionResult.Intersection;

		if (o2 == 0 && OnRay( rayOrigin, rayDirection, B ))
			return IntersectionResult.Intersection;

		return IntersectionResult.NoIntersection; // No intersection
	}

	/// <summary>
	/// Checks if point p lies on the infinite ray starting at a, in the direction of b.
	/// </summary>
	private static bool OnRay( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> p ) {
		// Check if p is collinear with the ray
		if (Orientation( a, b, p ) != 0)
			return false;

		// Check if p lies "forward" on the ray
		return (p.X - a.X) * (b.X - a.X) >= TScalar.Zero && (p.Y - a.Y) * (b.Y - a.Y) >= TScalar.Zero;
	}

	public static bool operator ==( Edge2<TScalar> left, Edge2<TScalar> right ) => left.Equals( right );

	public static bool operator !=( Edge2<TScalar> left, Edge2<TScalar> right ) => !(left == right);
}


public enum IntersectionResult {
	//If the intersection happened on a vertex, the edge is considered to be intersecting
	OnVertex,
	Intersection,
	NoIntersection
}