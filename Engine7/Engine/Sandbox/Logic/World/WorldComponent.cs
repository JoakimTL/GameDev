using Engine.Module.Entities.Container;

namespace Sandbox.Logic.World;
public sealed class WorldComponent : ComponentBase {
	public double SimulatedSurfaceArea { get; set; } = 0;
}

public sealed class WorldCameraComponent : ComponentBase {

}

public sealed class WorldCameraArchetype : ArchetypeBase {
	public WorldComponent WorldComponent { get; set; } = null!;
	public WorldCameraComponent WorldCameraComponent { get; set; } = null!;
}