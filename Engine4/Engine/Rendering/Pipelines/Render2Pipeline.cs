using Engine.Structure;

namespace Engine.Rendering.Pipelines;

[ProcessAfter( typeof( Render3Pipeline ), typeof( IRenderPipeline ) )]
public class Render2Pipeline : DisposableIdentifiable, IRenderPipeline {
	public Render2Pipeline( ) {

	}

	public void RenderFrame() => throw new NotImplementedException();
	public void DrawToScreen() => throw new NotImplementedException();

	protected override bool OnDispose() {
		return true;
	}
}
