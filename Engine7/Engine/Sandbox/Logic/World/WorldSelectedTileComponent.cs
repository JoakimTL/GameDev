using Engine.Module.Entities.Container;

namespace Sandbox.Logic.World;

public sealed class WorldSelectedTileComponent : ComponentBase {
	public Tile HoveringTile { get; private set; } = null!;
	public Tile SelectedTile { get; private set; } = null!;
}
