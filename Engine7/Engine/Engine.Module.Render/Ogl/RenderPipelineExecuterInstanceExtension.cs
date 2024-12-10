using Engine.Module.Render.Domain;

namespace Engine.Module.Render.Ogl;

public sealed class RenderPipelineExecuterInstanceExtension( IInstanceProvider instanceProvider ) : InstanceProviderExtensionBase<IRenderPipeline, IRenderPipeline>( instanceProvider ), IRenderPipeline {
	public void PrepareRendering( double time, double deltaTime ) {
		foreach (IRenderPipeline pipeline in this.SortedInstances)
			pipeline.PrepareRendering( time, deltaTime );
	}

	public void DrawToScreen() {
		foreach (IRenderPipeline pipeline in this.SortedInstances)
			pipeline.DrawToScreen();
	}
}