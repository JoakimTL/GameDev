using Engine.Module.Render.Domain;

namespace Engine.Standard.Render;
internal class Class1 {
}

public sealed class RenderSceneManager {

}

/// <summary>
/// Does some actual rendering during the update loop.
/// </summary>
public sealed class RenderPipelineService : IRenderPipeline {
	private readonly RenderSceneManager _renderSceneManager;

	public RenderPipelineService( RenderSceneManager renderSceneManager ) {
		this._renderSceneManager = renderSceneManager;
	}

	public void PrepareRendering( double time, double deltaTime ) {
		throw new NotImplementedException();
	}

	public void DrawToScreen() {
		throw new NotImplementedException();
	}
}