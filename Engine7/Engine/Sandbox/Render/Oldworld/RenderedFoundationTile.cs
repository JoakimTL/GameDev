using Sandbox.Logic.OldWorld.Tiles;

namespace Sandbox.Render.Oldworld;

public sealed class RenderedFoundationTile( CompositeTile tile ) : Identifiable {
	public CompositeTile Tile { get; } = tile;
	public int LevelOfDetail { get; private set; }
	public event Action<RenderedFoundationTile>? LoDChanged;

	public void SetLevelOfDetail( int newLoD ) {
		if (LevelOfDetail == newLoD)
			return;
		LevelOfDetail = newLoD;
		LoDChanged?.Invoke( this );
	}
}
