using Engine.Module.Entities.Container;
using Sandbox.Logic.Old.OldWorld.Tiles;
using Sandbox.Render.Oldworld;

namespace Sandbox.Logic.Old.OldWorld;

public sealed class WorldSelectedTileComponent : ComponentBase, IMessageReadingComponent {
	public Tile? HoveringTile { get; private set; } = null!;
	public Tile? SelectedTile { get; private set; } = null!;

	public void ReadMessage( object message ) {
		if (message is TileHoverMessage tileHoverMessage) {
			HoveringTile = tileHoverMessage.Tile;
			InvokeComponentChanged();
		}
	}
}
