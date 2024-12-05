//using Microsoft.VisualBasic;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Drawing;
//using System.Formats.Tar;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;

//namespace Engine.Algorithms.Triangulation;
//public static class Delaunay {
//	public static IEnumerable<Triangle2<TScalar>> ConstrainedTriangulate<TScalar, TFloatingScalar>( Span<Vector2<TScalar>> points, List<Edge2<TScalar>> enforcedEdges )
//		where TScalar : unmanaged, INumber<TScalar>
//		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
//		List<Triangle2<TScalar>> triangles = Triangulate<TScalar, TFloatingScalar>( points ).ToList();

//		foreach (Edge2<TScalar> edge in enforcedEdges)
//			EnforceConstraint( triangles, edge, enforcedEdges );
//		//https://github.com/Habrador/Computational-geometry/blob/master/Assets/_Habrador%20Computational%20Geometry%20Library/4.%20Triangulation/Delaunay/ConstrainedDelaunaySloan.cs

//		return triangles;
//	}

//	private static void EnforceConstraint<TScalar>( List<Triangle2<TScalar>> triangles, Edge2<TScalar> constraintEdge, List<Edge2<TScalar>> enforcedEdges ) where TScalar : unmanaged, INumber<TScalar> {
//		Span<Edge2<TScalar>> edges = stackalloc Edge2<TScalar>[ 3 ];
//		Span<Vector2<TScalar>> vertices = stackalloc Vector2<TScalar>[ 3 ];

//		List<Triangle2<TScalar>> intersectingTriangles = new List<Triangle2<TScalar>>();
//		foreach (Triangle2<TScalar> triangle in triangles) {
//			triangle.FillWithEdges( edges );
//			foreach (Edge2<TScalar> edge in edges) {
//				if (!constraintEdge.Equals( edge ) && constraintEdge.Intersects( edge )) {
//					intersectingTriangles.Add( triangle );
//					break;
//				}
//			}
//		}

//		if (intersectingTriangles.Count == 0)
//			return;

//		// Step 1: Remove intersecting triangles.
//		foreach (Triangle2<TScalar> triangle in intersectingTriangles) {
//			triangles.Remove( triangle );
//		}

//		HashSet<Vector2<TScalar>> newPoints = [ constraintEdge.A, constraintEdge.B ];

//		foreach (Triangle2<TScalar> triangle in intersectingTriangles) {
//			triangle.FillWithVerticies( vertices );
//			foreach (Vector2<TScalar> vertex in vertices) {
//				if (!newPoints.Contains( vertex )) {
//					newPoints.Add( vertex );
//				}
//			}
//		}

//		Retriangulate( constraintEdge, newPoints, triangles );
//	}

//	private static void Retriangulate<TScalar>( Edge2<TScalar> constraintEdge, HashSet<Vector2<TScalar>> points, List<Triangle2<TScalar>> triangles ) where TScalar : unmanaged, INumber<TScalar> {
//		List<Vector2<TScalar>> sortedPoints = points.OrderBy( p => GetEdgeParameter( constraintEdge, p ) ).ToList();

//		for (int i = 0; i < sortedPoints.Count - 1; i++) {
//			Edge2<TScalar> edge = new Edge2<TScalar>( sortedPoints[ i ], sortedPoints[ i + 1 ] );
//			foreach (Vector2<TScalar> otherPoint in points) {
//				if (!edge.HasVertex( otherPoint )) {
//					Triangle2<TScalar> newTriangle = new Triangle2<TScalar>( edge.A, edge.B, otherPoint );

//					// Ensure the new triangle is valid.
//					if (IsValidTriangle( newTriangle, constraintEdge, triangles )) {
//						triangles.Add( newTriangle );
//					}
//				}
//			}
//		}
//	}
//	private static void CreateTrianglesForEdge<TScalar>( Edge2<TScalar> edge, IEnumerable<Triangle2<TScalar>> intersectingTriangles, List<Triangle2<TScalar>> triangles, Edge2<TScalar> constraintEdge ) where TScalar : unmanaged, INumber<TScalar> {
//		Span<Vector2<TScalar>> currentTriangleVertices = stackalloc Vector2<TScalar>[ 3 ];
//		HashSet<Vector2<TScalar>> vertices = [];
//		foreach (Triangle2<TScalar> triangle in intersectingTriangles) {
//			triangle.FillWithVerticies( currentTriangleVertices );
//			foreach (Vector2<TScalar> vertex in currentTriangleVertices) {
//				if (!edge.HasVertex( vertex )) {
//					vertices.Add( vertex );
//				}
//			}
//		}

//		foreach (Vector2<TScalar> vertex in vertices) {
//			Triangle2<TScalar> newTriangle = new Triangle2<TScalar>( edge.A, edge.B, vertex );
//			if (IsValidTriangle( newTriangle, constraintEdge, triangles )) {
//				triangles.Add( newTriangle );
//			}
//		}
//	}
//	private static bool IsValidTriangle<TScalar>( Triangle2<TScalar> triangle, Edge2<TScalar> constraintEdge, List<Triangle2<TScalar>> triangles ) where TScalar : unmanaged, INumber<TScalar> {
//		Span<Edge2<TScalar>> edges = stackalloc Edge2<TScalar>[ 3 ];
//		Span<Edge2<TScalar>> edgesOther = stackalloc Edge2<TScalar>[ 3 ];
//		triangle.FillWithEdges( edges );
//		foreach (Edge2<TScalar> edge in edges)
//			if (!edge.Equals( constraintEdge ) && constraintEdge.Intersects<( edge ))
//				return false;
//		foreach (Triangle2<TScalar> otherTriangle in triangles) {
//			otherTriangle.FillWithEdges( edgesOther );
//			foreach (Edge2<TScalar> edgeA in edges)
//				foreach (Edge2<TScalar> edgeB in edgesOther)
//					if (!edgeA.Equals( edgeB ) && edgeA.Intersects( edgeB ))
//						return false;
//		}
//		return true;
//	}

//	private static double GetEdgeParameter<TScalar>( Edge2<TScalar> edge, Vector2<TScalar> point ) where TScalar : unmanaged, INumber<TScalar> {
//		if (edge.A.X == edge.B.X) {
//			// Edge is vertical; use the y-coordinate for parameterization.
//			if (edge.A.Y == edge.B.Y)
//				throw new InvalidOperationException( "Degenerate edge with zero length." );

//			return double.CreateSaturating( point.Y - edge.A.Y ) / double.CreateSaturating( edge.B.Y - edge.A.Y );
//		} else {
//			// Edge is not vertical; use the x-coordinate for parameterization.
//			return double.CreateSaturating( point.X - edge.A.X ) / double.CreateSaturating( edge.B.X - edge.A.X );
//		}
//	}

//	public static IEnumerable<Triangle2<TScalar>> Triangulate<TScalar, TFloatingScalar>( Span<Vector2<TScalar>> points )
//		where TScalar : unmanaged, INumber<TScalar>
//		where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {
//		if (points.Length < 3)
//			return Enumerable.Empty<Triangle2<TScalar>>();

//		AABB<Vector2<TScalar>> bounds = AABB.Create( points );
//		Vector2<TScalar> span = bounds.Maxima - bounds.Minima;

//		List<Triangle2<TScalar>> triangles = [];
//		List<Triangle2<TScalar>> badTriangles = [];
//		List<Edge2<TScalar>> polygonEdge = [];

//		Triangle2<TScalar> superTriangle = new( bounds.Minima - (span * TScalar.CreateSaturating( 2 )),
//			bounds.Maxima + (new Vector2<TScalar>( span.X, -span.Y ) * TScalar.CreateSaturating( 2 )), //Should really be sqrt 2, but pi works too.
//			bounds.Maxima + (new Vector2<TScalar>( -span.X, span.Y ) * TScalar.CreateSaturating( 2 )) );
//		triangles.Add( superTriangle );

//		for (int i = 0; i < points.Length; i++) {
//			badTriangles.Clear();
//			foreach (Triangle2<TScalar> triangle in triangles)
//				if (triangle.PointInCircumcircle<TFloatingScalar>( points[ i ] ))
//					badTriangles.Add( triangle );
//			polygonEdge.Clear();
//			foreach (Triangle2<TScalar> triangle in badTriangles) {
//				Edge2<TScalar> edgeAB = new( triangle.A, triangle.B );
//				Edge2<TScalar> edgeBC = new( triangle.B, triangle.C );
//				Edge2<TScalar> edgeCA = new( triangle.C, triangle.A );

//				if (!polygonEdge.Contains( edgeAB ))
//					polygonEdge.Add( edgeAB );
//				else
//					polygonEdge.Remove( edgeAB );

//				if (!polygonEdge.Contains( edgeBC ))
//					polygonEdge.Add( edgeBC );
//				else
//					polygonEdge.Remove( edgeBC );

//				if (!polygonEdge.Contains( edgeCA ))
//					polygonEdge.Add( edgeCA );
//				else
//					polygonEdge.Remove( edgeCA );
//			}

//			triangles.RemoveAll( badTriangles.Contains );

//			foreach (Edge2<TScalar> edge in polygonEdge)
//				triangles.Add( new Triangle2<TScalar>( edge.A, edge.B, points[ i ] ) );

//		}

//		triangles.RemoveAll( t => t.HasVertex( superTriangle.A ) || t.HasVertex( superTriangle.B ) || t.HasVertex( superTriangle.C ) );

//		return triangles;
//	}

//}
