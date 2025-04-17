using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class RenderCluster {
	private readonly List<Tile> _tilesInCluster;
	private readonly List<Edge> _edgesInCluster;

	public IReadOnlyList<Tile> Tiles => _tilesInCluster;
	public IReadOnlyList<Edge> Edges => _edgesInCluster;

	public readonly AABB<Vector3<float>> ClusterBounds;

	public bool IsVisible { get; private set; }

	public event Action? VisibilityChanged;

	public RenderCluster( IEnumerable<Tile> tilesInCluster, IEnumerable<Edge> edgesInCluster ) {
		this._tilesInCluster = tilesInCluster.ToList();
		this._edgesInCluster = edgesInCluster.ToList();
		this.ClusterBounds = AABB.Create<Vector3<float>>( _tilesInCluster.Select( t => t.Bounds ).ToArray() );
	}

	public void SetVisibility( bool visible ) {
		if (IsVisible == visible)
			return;
		IsVisible = visible;
		VisibilityChanged?.Invoke();
	}

	public void CheckVisibilityAgainstCameraTranslation( Vector3<float> normalizedTranslation ) {
		bool shouldBeVisible = false;

		Vector3<float> min = ClusterBounds.Minima;
		Vector3<float> max = ClusterBounds.Maxima;

		Span<Vector3<float>> boundsCorners =
		[
			(min.X, min.Y, min.Z),
			(min.X, min.Y, max.Z),
			(min.X, max.Y, max.Z),
			(min.X, max.Y, min.Z),
			(max.X, min.Y, min.Z),
			(max.X, min.Y, max.Z),
			(max.X, max.Y, max.Z),
			(max.X, max.Y, min.Z)
		];

		for (int i = 0; i < boundsCorners.Length; i++) 
			if (normalizedTranslation.Dot( boundsCorners[ i ] ) >= 0) {
				shouldBeVisible = true;
				break;
			}

		SetVisibility( shouldBeVisible );
	}
}
