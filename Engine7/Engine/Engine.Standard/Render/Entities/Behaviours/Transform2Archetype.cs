using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class Transform2Archetype : ArchetypeBase {
	public Transform2Component Transform2Component { get; set; } = null!;
}
