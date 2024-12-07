using System.Numerics;

namespace Engine.Shapes;

public static class PolygonExtensions {

	public static TScalar GetSignedArea<TScalar>( this Span<Vector2<TScalar>> orderedPolygonPoints ) where TScalar : unmanaged, INumber<TScalar> {
		TScalar sum = TScalar.Zero;
		Vector2<TScalar> p1 = orderedPolygonPoints[ ^1 ];
		for (int i = 0; i < orderedPolygonPoints.Length; i++) {
			Vector2<TScalar> p2 = orderedPolygonPoints[ i ];
			//sum += (p2.X - p1.X) * (p2.Y + p1.Y);
			sum += p1.X * p2.Y - p1.Y * p2.X;
			p1 = p2;
		}
		return sum / TScalar.CreateSaturating( 2 );
	}

	public static bool PointInPolygon<TScalar>( this Vector2<TScalar> point, Span<Vector2<TScalar>> orderedPolygonPoints ) where TScalar : unmanaged, INumber<TScalar> {
		Vector2<TScalar> rayDirection = new( TScalar.CreateSaturating( 1 ), TScalar.CreateSaturating( 0 ) );
		int intersections = 0;

		Vector2<TScalar> p2 = orderedPolygonPoints[ ^1 ];
		for (int i = 0; i < orderedPolygonPoints.Length; i++) {
			Vector2<TScalar> p1 = orderedPolygonPoints[ i ];
			Edge2<TScalar> edge = new( p1, p2 );
			IntersectionResult intersectionResult = edge.IntersectsRay( point, rayDirection );
			if (intersectionResult == IntersectionResult.OnVertex) {
				intersections++;
			} else if (intersectionResult == IntersectionResult.Intersection) {
				intersections += 2;
			}
			p2 = p1;
		}

		return (intersections / 2) % 2 == 1;
	}
}