using Engine.Module.Entities.Render;

namespace Engine.Standard.Render.Entities.Behaviours;
public sealed class MaterialRenderBehaviour : RenderBehaviourBase {

	private string _materialName;
	public object Material { get; private set; }

	public override void Update( double time, double deltaTime ) {

	}

	protected override bool InternalDispose() => true;
}
