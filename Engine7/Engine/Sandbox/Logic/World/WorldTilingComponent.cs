using Engine.Module.Entities.Container;
using Sandbox.Logic.World.Tiles.Generation;

namespace Sandbox.Logic.World;

public sealed class WorldTilingComponent : ComponentBase {
	public WorldTiling Tiling { get; } = new();
}
