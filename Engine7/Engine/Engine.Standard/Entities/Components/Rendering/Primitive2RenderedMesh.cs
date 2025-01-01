using Engine.Standard.Render.Meshing;

namespace Engine.Standard.Entities.Components.Rendering;

public sealed class Primitive2RenderedMesh( Primitive2 primitive ) : IRenderedMesh {
	public Primitive2 Primitive { get; } = primitive;
}
