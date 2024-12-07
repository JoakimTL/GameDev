using Engine.Shapes;
using System.Collections.Generic;
using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public sealed class TriangulationConstrainer<TScalar, TFloatingScalar>
	where TScalar : unmanaged, INumber<TScalar>
	where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar> {

	private readonly List<Triangle2<TScalar>> _triangles;
	private readonly List<Edge2<TScalar>> _constrainingEdges;
	private readonly Queue<Edge2<TScalar>> _constrainingEdgesQueue;
	private readonly List<Triangle2<TScalar>> _intersectingTriangles;
	private readonly HashSet<Vector2<TScalar>> _newPoints;
	private bool _isFinished;

	public IReadOnlyList<Triangle2<TScalar>> Triangles => this._triangles;
	public bool Finished => this._isFinished;

	public TriangulationConstrainer( Span<Triangle2<TScalar>> triangles, Span<Edge2<TScalar>> constrainingEdges ) {
		this._triangles = [];
		this._triangles.AddRange( triangles );
		this._constrainingEdges = [];
		this._constrainingEdgesQueue = [];
		this._intersectingTriangles = [];
		this._newPoints = [];

		foreach (Edge2<TScalar> edge in constrainingEdges) {
			if (this._constrainingEdges.Contains( edge ))
				throw new InvalidOperationException( "Duplicate constraining edge." );
			this._constrainingEdges.Add( edge );
			this._constrainingEdgesQueue.Enqueue( edge );
		}
	}

	public TriangulationConstrainer( Delaunator<TScalar, TFloatingScalar> delaunator, Span<Edge2<TScalar>> constrainingEdges ) : this( [], constrainingEdges ) {
		if (!delaunator.Finished)
			throw new InvalidOperationException( "Delaunator must be finished before constraining." );
		this._triangles.AddRange( delaunator.Triangles );
	}

	/// <returns>True if the constriction is finished.</returns>
	public bool Process() {
		if (this._isFinished)
			return true;
		if (!this._constrainingEdgesQueue.TryDequeue( out Edge2<TScalar> constraintEdge )) {
			this._isFinished = true;
			return true;
		}
		Span<Edge2<TScalar>> edges = stackalloc Edge2<TScalar>[ 3 ];
		Span<Vector2<TScalar>> vertices = stackalloc Vector2<TScalar>[ 3 ];

		this._intersectingTriangles.Clear();
		foreach (Triangle2<TScalar> triangle in this._triangles) {
			triangle.FillWithEdges( edges );
			foreach (Edge2<TScalar> edge in edges) {
				if (HasIntersectionAndNoCommonVertices( edge, constraintEdge )) {
					this._intersectingTriangles.Add( triangle );
					break;
				}
			}
		}

		if (this._intersectingTriangles.Count == 0)
			return false;

		// Step 1: Remove intersecting triangles.
		foreach (Triangle2<TScalar> triangle in this._intersectingTriangles)
			this._triangles.Remove( triangle );

		this._newPoints.Clear();
		this._newPoints.Add( constraintEdge.A );
		this._newPoints.Add( constraintEdge.B );

		foreach (Triangle2<TScalar> triangle in this._intersectingTriangles) {
			triangle.FillWithVertices( vertices );
			foreach (Vector2<TScalar> vertex in vertices) {
				if (!this._newPoints.Contains( vertex )) {
					this._newPoints.Add( vertex );
				}
			}
		}

		Retriangulate( constraintEdge );

		return false;
	}

	private void Retriangulate( Edge2<TScalar> constraintEdge ) {
		List<Vector2<TScalar>> sortedPoints = [ .. this._newPoints.OrderBy( p => GetEdgeParameter( constraintEdge, p ) ) ];

		for (int i = 0; i < sortedPoints.Count; i++) {
			foreach (Vector2<TScalar> otherPoint in this._newPoints) {
				if (!constraintEdge.HasVertex( otherPoint )) {
					Triangle2<TScalar> newTriangle = new( constraintEdge.A, constraintEdge.B, otherPoint );

					// Ensure the new triangle is valid.
					if (IsValidTriangle( newTriangle, constraintEdge ))
						this._triangles.Add( newTriangle );
				}
			}
		}

		for (int i = 0; i < sortedPoints.Count; i++)
			for (int j = i + 1; j < sortedPoints.Count; j++) {
				Edge2<TScalar> edge = new( sortedPoints[ i ], sortedPoints[ j ] );
				foreach (Vector2<TScalar> otherPoint in this._newPoints) {
					if (!edge.HasVertex( otherPoint )) {
						Triangle2<TScalar> newTriangle = new( edge.A, edge.B, otherPoint );

						// Ensure the new triangle is valid.
						if (IsValidTriangle( newTriangle, constraintEdge ))
							this._triangles.Add( newTriangle );
					}
				}
			}
	}

	private bool IsValidTriangle( Triangle2<TScalar> triangle, Edge2<TScalar> constraintEdge ) {
		Span<Edge2<TScalar>> edges = stackalloc Edge2<TScalar>[ 3 ];
		Span<Edge2<TScalar>> edgesOther = stackalloc Edge2<TScalar>[ 3 ];
		triangle.FillWithEdges( edges );
		foreach (Edge2<TScalar> edge in edges)
			if (HasIntersectionAndNoCommonVertices( edge, constraintEdge ))
				return false;
		foreach (Triangle2<TScalar> otherTriangle in this._triangles) {
			if (otherTriangle.Equals( triangle ))
				return false;
			otherTriangle.FillWithEdges( edgesOther );
			foreach (Edge2<TScalar> edgeA in edges)
				foreach (Edge2<TScalar> edgeB in edgesOther)
					if (HasIntersectionAndNoCommonVertices( edgeA, edgeB ))
						return false;
		}
		return true;
	}

	private TFloatingScalar GetEdgeParameter( Edge2<TScalar> edge, Vector2<TScalar> point ) {
		if (edge.A.X == edge.B.X) {
			// Edge is vertical; use the y-coordinate for parameterization.
			if (edge.A.Y == edge.B.Y)
				throw new InvalidOperationException( "Degenerate edge with zero length." );

			return TFloatingScalar.CreateSaturating( point.Y - edge.A.Y ) / TFloatingScalar.CreateSaturating( edge.B.Y - edge.A.Y );
		} else {
			// Edge is not vertical; use the x-coordinate for parameterization.
			return TFloatingScalar.CreateSaturating( point.X - edge.A.X ) / TFloatingScalar.CreateSaturating( edge.B.X - edge.A.X );
		}
	}

	private bool HasIntersectionAndNoCommonVertices( Edge2<TScalar> a, Edge2<TScalar> b ) => !a.Equals( b ) && !(a.HasVertex( b.A ) || a.HasVertex( b.B )) && a.Intersects( b );
}
