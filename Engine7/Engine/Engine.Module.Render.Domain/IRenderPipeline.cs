namespace Engine.Module.Render.Domain;

public interface IRenderPipeline {
	void PrepareRendering( double time, double deltaTime );
	void DrawToScreen();
}