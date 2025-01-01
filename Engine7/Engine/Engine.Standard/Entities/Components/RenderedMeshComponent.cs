using Engine.Module.Entities.Container;

namespace Engine.Standard.Entities.Components;

public sealed class RenderedMeshComponent : ComponentBase {
	public IRenderedMesh? RenderedMesh { get; private set; } = null;
	public Vector4<double> Color { get; private set; }

	public void SetRenderedMesh( IRenderedMesh? renderedMesh ) {
		if (this.RenderedMesh == renderedMesh)
			return;
		this.RenderedMesh = renderedMesh;
		InvokeComponentChanged();
	}

	public void SetColor( Vector4<double> color ) {
		if (this.Color == color)
			return;
		this.Color = color;
		InvokeComponentChanged();
	}
}

