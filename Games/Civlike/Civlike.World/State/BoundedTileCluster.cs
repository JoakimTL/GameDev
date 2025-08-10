using Civlike.World.Geometry;
using Engine;

namespace Civlike.World.State;

public sealed class BoundedTileCluster : DisposableIdentifiable {
	public BoundedTileCluster( Globe globe, BoundedRenderCluster cluster, IReadOnlyList<Tile> tiles ) {
		this.Globe = globe;
		this.RenderCluster = cluster;
		this.Tiles = tiles;
		foreach (Tile tile in tiles)
			tile.StateChanged += OnStateChanged;
	}

	public event Action<BoundedTileCluster>? StateChanged;

	public Globe Globe { get; }
	public BoundedRenderCluster RenderCluster { get; }
	public IReadOnlyList<Tile> Tiles { get; }

	private void OnStateChanged( StateBase<Tile> tileState ) {
		StateChanged?.Invoke( this );
	}

	protected override bool InternalDispose() {
		foreach (Tile tile in this.Tiles)
			tile.StateChanged -= OnStateChanged;
		return true;
	}
}