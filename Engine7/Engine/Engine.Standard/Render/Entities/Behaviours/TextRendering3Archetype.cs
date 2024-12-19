using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;
using Engine.Standard.Entities.Components.Rendering;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class TextRendering3Archetype : ArchetypeBase {
	public RenderedTextComponent RenderedTextComponent { get; set; } = null!;
	public Transform3Component Transform2Component { get; set; } = null!;
}
