using Engine.Module.Render.Domain;

namespace Engine.Module.Render.Ogl;

public sealed class RenderPipelineExecuterInstanceExtension( IInstanceProvider instanceProvider ) : InstanceProviderExtensionBase<IInstanceProvider, IRenderPipeline>( instanceProvider ), IRenderPipeline {
	public void PrepareRendering( double time, double deltaTime ) {
		foreach (IRenderPipeline updateable in this.SortedInstances)
			updateable.PrepareRendering( time, deltaTime );
	}

	public void DrawToScreen() {
		foreach (IRenderPipeline updateable in this.SortedInstances)
			updateable.DrawToScreen();
	}
}