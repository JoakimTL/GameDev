using Engine.Module.Entities.Container;

namespace Engine.Standard.Entities.Components.Rendering;

public sealed class RenderedPrimitive2Component : ComponentBase {
	private Primitive2 _primitive = Primitive2.Rectangle;
	public Primitive2 Primitive { get => this._primitive; set => SetPrimitive( value ); }

	private void SetPrimitive( Primitive2 primitive ) {
		if (this._primitive == primitive)
			return;
		this._primitive = primitive;
		InvokeComponentChanged();
	}

	public enum Primitive2 {
		Rectangle,
		Circle,
		EquilateralTriangle,
		RightSidedTriangle
	}
}
