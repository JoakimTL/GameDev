using Engine.Rendering.Objects;
using Engine.Rendering.Services;
using StandardPackage.Shaders.Pipelines;

namespace StandardPackage.Shaders.Bundles;
public sealed class Entity3ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) {
		AddPipeline( "default", pipelineService.Get<Entity3ShaderPipeline>() );
	}
}
