using Engine.Module.Entities.Container;

namespace Sandbox.Logic.Old.OldWorld;

public sealed class WorldCameraArchetype : ArchetypeBase {
	public WorldComponent WorldComponent { get; set; } = null!;
	public WorldCameraComponent WorldCameraComponent { get; set; } = null!;
}
