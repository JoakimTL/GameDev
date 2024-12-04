using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public struct Triangle2<TScalar>( Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> c ) : IEqualityComparer<Triangle2<TScalar>>
	where TScalar : unmanaged, INumber<TScalar> {

	public Vector2<TScalar> A = a;
	public Vector2<TScalar> B = b;
	public Vector2<TScalar> C = c;

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

	public Vector3<TScalar> GetBarycentric( Vector2<TScalar> p ) {
		var ab = B - A;
		var ac = C - A;
		var bc = C - B;

		var cp = p - C;
		var ap = p - A;

		var det = Matrix.Create2x2.TransposedBasis( ac, bc ).GetDeterminant();
		var v = Matrix.Create2x2.TransposedBasis( bc, cp ).GetDeterminant() / det;
		//var vT = ((p.X - C.X) * (B.Y - C.Y) - (B.X - C.X) * (p.Y - C.Y)) / det;
		var w = Matrix.Create2x2.TransposedBasis( ap, ac ).GetDeterminant() / det;
		//var wT = ((p.X - A.X) * (C.Y - A.Y) - (C.X - A.X) * (p.Y - A.Y)) / det;
		var u = TScalar.One - v - w;
		return new Vector3<TScalar>( u, v, w );
	}

	public bool Inside( Vector2<TScalar> p ) => GetBarycentric( p ) >= Vector3<TScalar>.Zero;

	public bool PointInCircumcircle<TFloatingScalar>( Vector2<TScalar> p )
		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
		var fA = A.CastSaturating<TScalar, TFloatingScalar>();
		var fB = B.CastSaturating<TScalar, TFloatingScalar>();
		var fC = C.CastSaturating<TScalar, TFloatingScalar>();
		var fp = p.CastSaturating<TScalar, TFloatingScalar>();

		var pa = fA - fp;
		var pb = fB - fp;
		var pc = fC - fp;

		var paSq = pa.MagnitudeSquared();
		var pbSq = pb.MagnitudeSquared();
		var pcSq = pc.MagnitudeSquared();

		var det = Matrix.Create3x3.TransposedBasis(
			new Vector3<TFloatingScalar>( pa.X, pa.Y, paSq ),
			new Vector3<TFloatingScalar>( pb.X, pb.Y, pbSq ),
			new Vector3<TFloatingScalar>( pc.X, pc.Y, pcSq ) )
			.GetDeterminant();// * (WindsClockwise() ? TFloatingScalar.NegativeOne : TFloatingScalar.One);

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

	public bool HasVertex( Vector2<TScalar> p ) => A == p || B == p || C == p;

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
			return Equals( this, triangle );
		return base.Equals( obj );
	}

	public bool Equals( Triangle2<TScalar> x, Triangle2<TScalar> y ) => x.HasVertex( y.A ) && x.HasVertex( y.B ) && x.HasVertex( y.C );

	public override string ToString() => $"[{A} -> {B} -> {C}]";

	public int GetHashCode( [DisallowNull] Triangle2<TScalar> obj ) => HashCode.Combine( obj.A, obj.B, obj.C );

	public bool AllPointsIn( Span<Vector2<TScalar>> points ) {
		bool hasA = false;
		bool hasB = false;
		bool hasC = false;
		for (int i = 0; i < points.Length; i++) {
			if (A == points[ i ])
				hasA = true;
			if (B == points[ i ])
				hasB = true;
			if (C == points[ i ])
				hasC = true;
		}
		return hasA && hasB && hasC;
	}

	public bool WindsClockwise() {
		var v0 = B - A;
		var v1 = C - A;
		return (v0.X * v1.Y) - (v0.Y * v1.X) < TScalar.Zero;
	}

	internal void FillWithEdges( Span<Edge2<TScalar>> edges ) {
		edges[ 0 ] = new( A, B );
		edges[ 1 ] = new( B, C );
		edges[ 2 ] = new( C, A );
	}

	internal void FillWithVerticies( Span<Vector2<TScalar>> vertices ) {
		vertices[ 0 ] = A;
		vertices[ 1 ] = B;
		vertices[ 2 ] = C;
	}
}
