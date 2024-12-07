using Engine.Logging;
using Engine.Shapes;
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

	public IReadOnlyList<Triangle2<TScalar>> Triangles => this._triangles;
	public bool Finished => this._isFinished;

	public Delaunator( Span<Vector2<TScalar>> points, bool allowDuplicates, string? debugName = null ) {
		if (debugName is not null)
			this.Nickname = debugName;
		this._points = [];
		this._processQueue = [];
		this._triangles = [];
		this._badTriangles = [];
		this._polygonEdge = [];

		foreach (Vector2<TScalar> point in points) {
			if (this._points.Contains( point ) && !allowDuplicates) {
				this.LogWarning( "Duplicate point. Skipping." );
				continue;
			}
			this._points.Add( point );
			this._processQueue.Enqueue( point );
		}

		AABB<Vector2<TScalar>> bounds = AABB.Create( points );
		Vector2<TScalar> span = bounds.Maxima - bounds.Minima;
		this._superTriangle = new( bounds.Minima - (span * TScalar.CreateSaturating( 2 )),
			bounds.Maxima + (new Vector2<TScalar>( span.X, -span.Y ) * TScalar.CreateSaturating( 2 )), //Should really be sqrt 2, but pi works too.
			bounds.Maxima + (new Vector2<TScalar>( -span.X, span.Y ) * TScalar.CreateSaturating( 2 )) );
		this._triangles.Add( this._superTriangle );
	}

	public TriangulationConstrainer<TScalar, TFloatingScalar> CreateConstrainer(Span<Edge2<TScalar>> edges) => new( this, edges );

	/// <returns>True if there are no more points to process.</returns>
	public bool Process() {
		if (this._isFinished)
			return true;
		if (!this._processQueue.TryDequeue( out Vector2<TScalar> point )){
			this._isFinished = true;
			this._triangles.RemoveAll( t => t.HasVertex( this._superTriangle.A ) || t.HasVertex( this._superTriangle.B ) || t.HasVertex( this._superTriangle.C ) );
			return true;
		}

		this._badTriangles.Clear();
		foreach (Triangle2<TScalar> triangle in this._triangles)
			if (triangle.PointInCircumcircle<TFloatingScalar>( point ))
				this._badTriangles.Add( triangle );

		this._polygonEdge.Clear();
		foreach (Triangle2<TScalar> triangle in this._badTriangles) {
			Edge2<TScalar> edgeAB = new( triangle.A, triangle.B );
			Edge2<TScalar> edgeBC = new( triangle.B, triangle.C );
			Edge2<TScalar> edgeCA = new( triangle.C, triangle.A );

			if (!this._polygonEdge.Contains( edgeAB ))
				this._polygonEdge.Add( edgeAB );
			else
				this._polygonEdge.Remove( edgeAB );

			if (!this._polygonEdge.Contains( edgeBC ))
				this._polygonEdge.Add( edgeBC );
			else
				this._polygonEdge.Remove( edgeBC );

			if (!this._polygonEdge.Contains( edgeCA ))
				this._polygonEdge.Add( edgeCA );
			else
				this._polygonEdge.Remove( edgeCA );
		}

		this._triangles.RemoveAll( this._badTriangles.Contains );

		foreach (Edge2<TScalar> edge in this._polygonEdge)
			this._triangles.Add( new Triangle2<TScalar>( edge.A, edge.B, point ) );

		return false;
	}
}
