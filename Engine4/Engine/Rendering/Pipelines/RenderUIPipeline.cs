using Engine.Structure;

namespace Engine.Rendering.Pipelines;

[ProcessAfter( typeof( Render2Pipeline ), typeof( IRenderPipeline ) )]
public class RenderUIPipeline : DisposableIdentifiable, IRenderPipeline {
	public RenderUIPipeline() : base() {

	}

	public void RenderFrame() => throw new NotImplementedException();
	public void DrawToScreen() => throw new NotImplementedException();

	protected override bool OnDispose() {
		return true;
	}
}