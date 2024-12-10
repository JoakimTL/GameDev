using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class TextRendering2Archetype : ArchetypeBase {
	public RenderedTextComponent RenderedTextComponent { get; set; } = null!;
	public Transform2Component Transform2Component { get; set; } = null!;
}
