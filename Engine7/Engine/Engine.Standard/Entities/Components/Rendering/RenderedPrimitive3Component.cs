//using Engine.Module.Entities.Container;
//using Engine.Standard.Render.Meshing;

//namespace Engine.Standard.Entities.Components.Rendering;

//public sealed class RenderedPrimitive3Component : ComponentBase {
//	private Primitive3 _primitive = Primitive3.Cube;
//	public Primitive3 Primitive { get => this._primitive; set => SetPrimitive( value ); }

//	private void SetPrimitive( Primitive3 primitive ) {
//		if (this._primitive == primitive)
//			return;
//		this._primitive = primitive;
//		InvokeComponentChanged();
//	}
//}
