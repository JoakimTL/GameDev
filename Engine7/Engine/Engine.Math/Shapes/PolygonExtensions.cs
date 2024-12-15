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

	public static bool PointInPolygon<TScalar>( this Vector2<TScalar> point, Span<Vector2<TScalar>> orderedPolygonPoints, Vector2<TScalar> rayDirection ) where TScalar : unmanaged, INumber<TScalar> {
		//https://stackoverflow.com/questions/47004208/should-point-on-the-edge-of-polygon-be-inside-polygon
		int intersections = 0;
		int skippedEdges = 0;

		Span<Vector2<TScalar>> postDegeneratePoints = stackalloc Vector2<TScalar>[ orderedPolygonPoints.Length ];
		Edge2<TScalar> conceptualRay = new( point, point + rayDirection );
		for (int i = 0; i < postDegeneratePoints.Length; i++) {
			if (Orientation( conceptualRay, orderedPolygonPoints[ i ] ) == 0) {
				postDegeneratePoints[ i ] = (orderedPolygonPoints[ i ] * TScalar.CreateSaturating( 98 ) + orderedPolygonPoints[ (i + 1) % orderedPolygonPoints.Length ] + orderedPolygonPoints[ (i - 1 + orderedPolygonPoints.Length) % orderedPolygonPoints.Length ]) / TScalar.CreateSaturating( 100 );
			} else {
				postDegeneratePoints[ i ] = orderedPolygonPoints[ i ];
			}
		}

		Vector2<TScalar> p2 = postDegeneratePoints[ ^1 ];
		for (int i = 0; i < postDegeneratePoints.Length; i++) {
			Vector2<TScalar> p1 = postDegeneratePoints[ i ];
			if (p1 == p2) {
				skippedEdges++;
				continue;
			}
			Edge2<TScalar> edge = new( p1, p2 );
			IntersectionResult intersectionResult = edge.IntersectsRay( point, rayDirection );
			if (intersectionResult == IntersectionResult.OnVertex) {
				intersections++;
			} else if (intersectionResult == IntersectionResult.Intersection) {
				intersections += 2;
			}
			p2 = p1;
		}

		if (skippedEdges > 0)
			Console.WriteLine( $"Skipped {skippedEdges} edges." );

		return (intersections / 2) % 2 == 1;
	}

	/// <summary>
	/// Checks the orientation of the point relative to the edge. Returns -1 if the point is to the left of the edge, 0 if the point is on the edge, and 1 if the point is to the right of the edge.
	/// </summary>
	public static int Orientation<TScalar>( this Edge2<TScalar> edge, Vector2<TScalar> p ) where TScalar : unmanaged, INumber<TScalar> {
		Vector2<TScalar> perp = (edge.B - edge.A) * -Bivector2<TScalar>.One;
		return TScalar.Sign( perp.Dot( p - edge.A ) );
	}
}