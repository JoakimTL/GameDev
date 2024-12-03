using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Algorithms.Triangulation;
public static class Delaunay {

	public static IEnumerable<Triangle2<TScalar>> Triangulate<TScalar>( Span<Vector2<TScalar>> points ) where TScalar : unmanaged, INumber<TScalar> {
		if (points.Length < 3)
			return Enumerable.Empty<Triangle2<TScalar>>();

		AABB<Vector2<TScalar>> bounds = AABB.Create( points );
		var span = bounds.Maxima - bounds.Minima;

		List<Triangle2<TScalar>> triangles = [];
		List<Triangle2<TScalar>> badTriangles = [];
		List<Edge2<TScalar>> polygonEdge = [];

		Triangle2<TScalar> superTriangle = new( bounds.Minima - span * TScalar.CreateSaturating(4),
			bounds.Maxima + new Vector2<TScalar>( -span.X, span.Y ) * TScalar.CreateSaturating( 4 ), //Should really be sqrt 2, but pi works too.
			bounds.Maxima + new Vector2<TScalar>( span.X, -span.Y ) * TScalar.CreateSaturating( 4 ) );
		triangles.Add( superTriangle );

		for (int i = 0; i < points.Length; i++) {
			badTriangles.Clear();
			foreach (var triangle in triangles)
				if (triangle.PointInCircumcircle( points[ i ] ))
					badTriangles.Add( triangle );
			polygonEdge.Clear();
			foreach (var triangle in badTriangles) {
				Edge2<TScalar> edgeAB = new( triangle.A, triangle.B );
				Edge2<TScalar> edgeBC = new( triangle.B, triangle.C );
				Edge2<TScalar> edgeCA = new( triangle.C, triangle.A );

				if (!polygonEdge.Contains( edgeAB ))
					polygonEdge.Add( edgeAB );
				else
					polygonEdge.Remove( edgeAB );

				if (!polygonEdge.Contains( edgeBC ))
					polygonEdge.Add( edgeBC );
				else
					polygonEdge.Remove( edgeBC );

				if (!polygonEdge.Contains( edgeCA ))
					polygonEdge.Add( edgeCA );
				else
					polygonEdge.Remove( edgeCA );
			}

			triangles.RemoveAll( badTriangles.Contains );

			foreach (var edge in polygonEdge)
				triangles.Add( new Triangle2<TScalar>( edge.A, edge.B, points[ i ] ) );

		}

		triangles.RemoveAll( t => t.HasVertex( superTriangle.A ) || t.HasVertex( superTriangle.B ) || t.HasVertex( superTriangle.C ) );

		return triangles;
	}

}

public static class EarClipping {
	public static IEnumerable<Triangle2<TScalar>> Triangulate<TScalar>( Span<Vector2<TScalar>> points ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (points.Length < 3)
			return Enumerable.Empty<Triangle2<TScalar>>();
		List<Triangle2<TScalar>> triangles = [];
		List<Vector2<TScalar>> remainingPoints = [];
		remainingPoints.AddRange( points );
		while (remainingPoints.Count > 3) {
			for (int i = 0; i < remainingPoints.Count; i++) {
				Vector2<TScalar> a = remainingPoints[ i ];
				Vector2<TScalar> b = remainingPoints[ (i + 1) % remainingPoints.Count ];
				Vector2<TScalar> c = remainingPoints[ (i + 2) % remainingPoints.Count ];
				Triangle2<TScalar> triangle = new( a, b, c );
				bool isEar = true;
				for (int j = 0; j < remainingPoints.Count; j++) {
					if (j == i || j == (i + 1) % remainingPoints.Count || j == (i + 2) % remainingPoints.Count)
						continue;
					if (triangle.Inside( remainingPoints[ j ] )) {
						isEar = false;
						break;
					}
				}
				if (isEar) {
					triangles.Add( triangle );
					remainingPoints.RemoveAt( (i + 1) % remainingPoints.Count );
					break;
				}
			}
		}
		triangles.Add( new Triangle2<TScalar>( remainingPoints[ 0 ], remainingPoints[ 1 ], remainingPoints[ 2 ] ) );
		return triangles;
	}
}
