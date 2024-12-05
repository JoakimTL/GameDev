using Engine.Logging;
using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public sealed class Delaunator<TScalar, TFloatingScalar> : Identifiable
	where TScalar : unmanaged, INumber<TScalar>
	where TFloatingScalar : unmanaged, IFloatingPointIeee754<TFloatingScalar>{

	private readonly List<Vector2<TScalar>> _points;
	private readonly Queue<Vector2<TScalar>> _processQueue;
	private readonly List<Triangle2<TScalar>> _triangles;
	private readonly List<Triangle2<TScalar>> _badTriangles;
	private readonly Triangle2<TScalar> _superTriangle;
	private readonly List<Edge2<TScalar>> _polygonEdge;
	private bool _isFinished;

	public IReadOnlyList<Triangle2<TScalar>> Triangles => _triangles;
	public bool Finished => _isFinished;

	public Delaunator( Span<Vector2<TScalar>> points, bool allowDuplicates, string? debugName = null ) {
		if (debugName is not null)
			this.Nickname = debugName;
		_points = [];
		_processQueue = [];
		_triangles = [];
		_badTriangles = [];
		_polygonEdge = [];

		foreach (Vector2<TScalar> point in points) {
			if (_points.Contains( point ) && !allowDuplicates) {
				this.LogWarning( "Duplicate point. Skipping." );
				continue;
			}
			_points.Add( point );
			_processQueue.Enqueue( point );
		}

		AABB<Vector2<TScalar>> bounds = AABB.Create( points );
		Vector2<TScalar> span = bounds.Maxima - bounds.Minima;
		_superTriangle = new( bounds.Minima - (span * TScalar.CreateSaturating( 2 )),
			bounds.Maxima + (new Vector2<TScalar>( span.X, -span.Y ) * TScalar.CreateSaturating( 2 )), //Should really be sqrt 2, but pi works too.
			bounds.Maxima + (new Vector2<TScalar>( -span.X, span.Y ) * TScalar.CreateSaturating( 2 )) );
		_triangles.Add( _superTriangle );
	}

	public TriangulationConstrainer<TScalar, TFloatingScalar> CreateConstrainer(Span<Edge2<TScalar>> edges) => new( this, edges );

	/// <returns>True if there are no more points to process.</returns>
	public bool Process() {
		if (_isFinished)
			return true;
		if (!_processQueue.TryDequeue( out Vector2<TScalar> point )){
			_isFinished = true;
			_triangles.RemoveAll( t => t.HasVertex( _superTriangle.A ) || t.HasVertex( _superTriangle.B ) || t.HasVertex( _superTriangle.C ) );
			return true;
		}

		_badTriangles.Clear();
		foreach (Triangle2<TScalar> triangle in _triangles)
			if (triangle.PointInCircumcircle<TFloatingScalar>( point ))
				_badTriangles.Add( triangle );

		_polygonEdge.Clear();
		foreach (Triangle2<TScalar> triangle in _badTriangles) {
			Edge2<TScalar> edgeAB = new( triangle.A, triangle.B );
			Edge2<TScalar> edgeBC = new( triangle.B, triangle.C );
			Edge2<TScalar> edgeCA = new( triangle.C, triangle.A );

			if (!_polygonEdge.Contains( edgeAB ))
				_polygonEdge.Add( edgeAB );
			else
				_polygonEdge.Remove( edgeAB );

			if (!_polygonEdge.Contains( edgeBC ))
				_polygonEdge.Add( edgeBC );
			else
				_polygonEdge.Remove( edgeBC );

			if (!_polygonEdge.Contains( edgeCA ))
				_polygonEdge.Add( edgeCA );
			else
				_polygonEdge.Remove( edgeCA );
		}

		_triangles.RemoveAll( _badTriangles.Contains );

		foreach (Edge2<TScalar> edge in _polygonEdge)
			_triangles.Add( new Triangle2<TScalar>( edge.A, edge.B, point ) );

		return false;
	}
}
