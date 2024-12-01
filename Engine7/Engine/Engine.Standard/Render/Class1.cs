using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render;
internal class Class1 {
}

public sealed class RenderSceneManager {

}

/// <summary>
/// Does some actual rendering during the update loop.
/// </summary>
public sealed class RenderPipelineService : IRenderPipeline {

	public RenderPipelineService( SceneService sceneService ) {
	}

	public void PrepareRendering( double time, double deltaTime ) {
		throw new NotImplementedException();
	}

	public void DrawToScreen() {
		throw new NotImplementedException();
	}
}
