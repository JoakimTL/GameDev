using Engine.Module.Entities.Container;

namespace Sandbox.Logic.World;
public sealed class WorldComponent : ComponentBase {
	public double SimulatedSurfaceArea { get; set; } = 0;
}

public sealed class WorldTilingComponent : ComponentBase {
	public WorldTiling Tiling { get; } = new();
}
