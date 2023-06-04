using Engine.Rendering;
using Engine.Rendering.Contexts.Services;
using StandardPackage.Rendering.Pipelines;

namespace TestPlatformClient;

public class TestGameRenderModule : RenderModule {
	protected override void OnInitialize( Window window ) {
		window.Context.Service<RenderPipelineService>().Add<Default3Pipeline>();
	}
}