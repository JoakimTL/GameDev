using Engine.Module.Entities.Container;
using Sandbox.Render.World;

namespace Sandbox.Logic.World;

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
