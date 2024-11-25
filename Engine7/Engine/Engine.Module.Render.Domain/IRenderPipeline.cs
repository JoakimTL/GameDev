namespace Engine.Module.Render.Domain;

public interface IRenderPipeline : IUpdateable {
	void DrawToScreen();
}