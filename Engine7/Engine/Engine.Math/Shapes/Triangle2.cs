using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Shapes;

public readonly struct Triangle2<TScalar>( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> c ) : IEqualityComparer<Triangle2<TScalar>>
	where TScalar : unmanaged, INumber<TScalar> {

	public readonly Vector2<TScalar> A = a;
	public readonly Vector2<TScalar> B = b;
	public readonly Vector2<TScalar> C = c;

	//public Circle<TScalar>? GetCircumcircle() {
	//	var two = TScalar.One + TScalar.One;
	//	var midAB = (A + B).ScalarDivide( two );
	//	var midAC = (A + C).ScalarDivide( two );

	//	var perpAB = (B - A) * -Bivector2<TScalar>.One;
	//	var perpAC = (C - A) * -Bivector2<TScalar>.One;

	//	if (!TryFindIntersection( midAB, perpAB, midAC, perpAC, out var center ))
	//		return null;

	//	var radius = (center - A).Magnitude<Vector2<TScalar>, TScalar>();
	//	return new Circle<TScalar>( center, radius );
	//}

	public readonly Vector3<TScalar> GetBarycentric( Vector2<TScalar> p ) {
		Vector2<TScalar> ab = this.B - this.A;
		Vector2<TScalar> ac = this.C - this.A;
		Vector2<TScalar> bc = this.C - this.B;

		Vector2<TScalar> cp = p - this.C;
		Vector2<TScalar> ap = p - this.A;

		TScalar det = Matrix.Create2x2.TransposedBasis( ac, bc ).GetDeterminant();
		TScalar v = Matrix.Create2x2.TransposedBasis( bc, cp ).GetDeterminant() / det;
		//var vT = ((p.X - C.X) * (B.Y - C.Y) - (B.X - C.X) * (p.Y - C.Y)) / det;
		TScalar w = Matrix.Create2x2.TransposedBasis( ap, ac ).GetDeterminant() / det;
		//var wT = ((p.X - A.X) * (C.Y - A.Y) - (C.X - A.X) * (p.Y - A.Y)) / det;
		TScalar u = TScalar.One - v - w;
		return new Vector3<TScalar>( u, v, w );
	}

	public bool Inside( Vector2<TScalar> p ) => GetBarycentric( p ) >= Vector3<TScalar>.Zero;

	public bool PointInCircumcircle<TFloatingScalar>( Vector2<TScalar> p )
		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
		Vector2<TFloatingScalar> fA = this.A.CastSaturating<TScalar, TFloatingScalar>();
		Vector2<TFloatingScalar> fB = this.B.CastSaturating<TScalar, TFloatingScalar>();
		Vector2<TFloatingScalar> fC = this.C.CastSaturating<TScalar, TFloatingScalar>();
		Vector2<TFloatingScalar> fp = p.CastSaturating<TScalar, TFloatingScalar>();

		Vector2<TFloatingScalar> pa = fA - fp;
		Vector2<TFloatingScalar> pb = fB - fp;
		Vector2<TFloatingScalar> pc = fC - fp;

		TFloatingScalar paSq = pa.MagnitudeSquared();
		TFloatingScalar pbSq = pb.MagnitudeSquared();
		TFloatingScalar pcSq = pc.MagnitudeSquared();

		TFloatingScalar det = Matrix.Create3x3.TransposedBasis(
			new Vector3<TFloatingScalar>( pa.X, pa.Y, paSq ),
			new Vector3<TFloatingScalar>( pb.X, pb.Y, pbSq ),
			new Vector3<TFloatingScalar>( pc.X, pc.Y, pcSq ) )
			.GetDeterminant() * (WindsClockwise() ? TFloatingScalar.NegativeOne : TFloatingScalar.One);

		//bool inProperCircle = false;
		//var circumcircle = GetCircumcircle();
		//if (circumcircle.HasValue) {
		//	var circle = circumcircle.Value;
		//	inProperCircle = (circle.Center - p).Magnitude<Vector2<TScalar>, TScalar>() < circle.Radius;

		//	if (inProperCircle != det > TScalar.CreateSaturating( 0.00007 )) {
		//		Console.WriteLine( "a" );
		//		return inProperCircle;
		//	}
		//}

		return det > TFloatingScalar.Zero;
	}

	public readonly bool HasVertex( Vector2<TScalar> p ) => this.A == p || this.B == p || this.C == p;

	//private static bool TryFindIntersection( Vector2<TScalar> midAB, Vector2<TScalar> dirAB, Vector2<TScalar> midAC, Vector2<TScalar> dirAC, out Vector2<TScalar> intersection ) {
	//	intersection = default;
	//	TScalar det = Matrix.Create2x2.TransposedBasis( dirAB, dirAC ).GetDeterminant();

	//	if (TScalar.Abs( det ) < TScalar.Epsilon * TScalar.CreateChecked( 10000 ))
	//		return false;

	//	TScalar t = ((midAC.X - midAB.X) * dirAC.Y - (midAC.Y - midAB.Y) * dirAC.X) / det;
	//	intersection = midAB + t * dirAB;
	//	return true;
	//}

	public override bool Equals( [NotNullWhen( true )] object? obj ) {
		if (obj is Triangle2<TScalar> triangle)
			return HasVertex( triangle.A ) && HasVertex( triangle.B ) && HasVertex( triangle.C );
		return base.Equals( obj );
	}

	public override readonly int GetHashCode() => HashCode.Combine( this.A, this.B, this.C );

	public readonly bool Equals( Triangle2<TScalar> x, Triangle2<TScalar> y ) => x.Equals( y );

	public readonly int GetHashCode( [DisallowNull] Triangle2<TScalar> obj ) => obj.GetHashCode();

	public override readonly string ToString() => $"[{this.A} -> {this.B} -> {this.C}]";

	public readonly bool AllPointsIn( Span<Vector2<TScalar>> points ) {
		bool hasA = false;
		bool hasB = false;
		bool hasC = false;
		for (int i = 0; i < points.Length; i++) {
			if (this.A == points[ i ])
				hasA = true;
			if (this.B == points[ i ])
				hasB = true;
			if (this.C == points[ i ])
				hasC = true;
		}
		return hasA && hasB && hasC;
	}

	public readonly bool WindsClockwise() {
		Vector2<TScalar> v0 = this.B - this.A;
		Vector2<TScalar> v1 = this.C - this.A;
		return v0.X * v1.Y - v0.Y * v1.X < TScalar.Zero;
	}

	public readonly void FillWithEdges( Span<Edge2<TScalar>> edges ) {
		edges[ 0 ] = new( this.A, this.B );
		edges[ 1 ] = new( this.B, this.C );
		edges[ 2 ] = new( this.C, this.A );
	}

	public readonly void FillWithVertices( Span<Vector2<TScalar>> vertices ) {
		vertices[ 0 ] = this.A;
		vertices[ 1 ] = this.B;
		vertices[ 2 ] = this.C;
	}

	public static bool operator ==( Triangle2<TScalar> left, Triangle2<TScalar> right ) => left.Equals( right );

	public static bool operator !=( Triangle2<TScalar> left, Triangle2<TScalar> right ) => !(left == right);
}
