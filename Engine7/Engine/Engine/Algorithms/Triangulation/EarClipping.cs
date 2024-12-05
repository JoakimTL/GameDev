using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public static class EarClipping {
	public static List<Triangle2<TScalar>> TriangulatePolygon<TScalar, TFloatingScalar>( List<Vector2<TScalar>> points )
	where TScalar : unmanaged, INumber<TScalar>
	where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
		if (points == null || points.Count < 3)
			throw new ArgumentException( "Polygon must have at least 3 points." );

		List<Triangle2<TScalar>> triangles = [];
		List<Vector2<TScalar>> remainingVertices = new( points );

		while (remainingVertices.Count > 3) {
			bool earFound = false;

			for (int i = 0; i < remainingVertices.Count; i++) {
				int prevIndex = (i - 1 + remainingVertices.Count) % remainingVertices.Count;
				int nextIndex = (i + 1) % remainingVertices.Count;

				Vector2<TScalar> prev = remainingVertices[ prevIndex ];
				Vector2<TScalar> current = remainingVertices[ i ];
				Vector2<TScalar> next = remainingVertices[ nextIndex ];

				if (IsConvex<TScalar, TFloatingScalar>( prev, current, next )) {
					// Check if no other vertex is inside the triangle
					bool isEar = true;
					for (int j = 0; j < remainingVertices.Count; j++) {
						if (j == prevIndex || j == i || j == nextIndex)
							continue;

						if (PointInTriangle<TScalar, TFloatingScalar>( remainingVertices[ j ], prev, current, next )) {
							isEar = false;
							break;
						}
					}

					if (isEar) {
						triangles.Add( new Triangle2<TScalar>( prev, current, next ) );
						remainingVertices.RemoveAt( i );
						earFound = true;
						break;
					}
				}
			}

			if (!earFound)
				return triangles;
		}

		// Add the final triangle
		if (remainingVertices.Count == 3) {
			triangles.Add( new Triangle2<TScalar>(
				remainingVertices[ 0 ],
				remainingVertices[ 1 ],
				remainingVertices[ 2 ]
			) );
		}

		return triangles;
	}

	private static bool IsConvex<TScalar, TFloatingScalar>( Vector2<TScalar> prev, Vector2<TScalar> current, Vector2<TScalar> next )
		where TScalar : unmanaged, INumber<TScalar>
		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
		return CrossProduct( prev.CastSaturating<TScalar, TFloatingScalar>(), current.CastSaturating<TScalar, TFloatingScalar>(), next.CastSaturating<TScalar, TFloatingScalar>() ) > TFloatingScalar.Zero;
	}

	private static TFloatingScalar CrossProduct<TFloatingScalar>( Vector2<TFloatingScalar> a, Vector2<TFloatingScalar> b, Vector2<TFloatingScalar> c )
		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
		return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
	}

	private static bool PointInTriangle<TScalar, TFloatingScalar>( Vector2<TScalar> point, Vector2<TScalar> a, Vector2<TScalar> b, Vector2<TScalar> c )
		where TScalar : unmanaged, INumber<TScalar>
		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
		TFloatingScalar area = TFloatingScalar.Abs( CrossProduct( a.CastSaturating<TScalar, TFloatingScalar>(), b.CastSaturating<TScalar, TFloatingScalar>(), c.CastSaturating<TScalar, TFloatingScalar>() ) );
		TFloatingScalar area1 = TFloatingScalar.Abs( CrossProduct( point.CastSaturating<TScalar, TFloatingScalar>(), a.CastSaturating<TScalar, TFloatingScalar>(), b.CastSaturating<TScalar, TFloatingScalar>() ) );
		TFloatingScalar area2 = TFloatingScalar.Abs( CrossProduct( point.CastSaturating<TScalar, TFloatingScalar>(), b.CastSaturating<TScalar, TFloatingScalar>(), c.CastSaturating<TScalar, TFloatingScalar>() ) );
		TFloatingScalar area3 = TFloatingScalar.Abs( CrossProduct( point.CastSaturating<TScalar, TFloatingScalar>(), c.CastSaturating<TScalar, TFloatingScalar>(), a.CastSaturating<TScalar, TFloatingScalar>() ) );

		return TFloatingScalar.Abs( area - (area1 + area2 + area3) ) < TFloatingScalar.CreateSaturating(0.0001);
	}
}