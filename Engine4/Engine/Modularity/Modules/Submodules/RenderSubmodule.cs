using Engine.Rendering.Pipelines;
using Engine.Structure;

namespace Engine.Modularity.Modules.Submodules;

[ProcessAfter( typeof( WindowCreationSubmodule ), typeof( IUpdateable ) )]
[ProcessAfter( typeof( ContextUpdateSubmodule ), typeof( IUpdateable ) )]
public class RenderSubmodule : Submodule {

	public RenderSubmodule() : base( true ) {
		OnInitialization += Initialized;
		OnUpdate += Updated;
	}

	private void Updated( float time, float deltaTime ) => Resources.Render.PipelineManager.Render();

	private void Initialized() {
		Resources.Render.Window.WindowEvents.Closing += WindowClosing;
		Resources.Render.PipelineManager.AddPipeline<Render3Pipeline>();
	}

	private void WindowClosing() => Remove();

	protected override bool OnDispose() => true;

}
