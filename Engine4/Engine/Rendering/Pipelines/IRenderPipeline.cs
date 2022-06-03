namespace Engine.Rendering.Pipelines;

public interface IRenderPipeline {
	void RenderFrame();
	void DrawToScreen();
}
