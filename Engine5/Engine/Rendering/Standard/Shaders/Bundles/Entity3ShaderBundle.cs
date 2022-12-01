using Engine.Rendering.Objects;
using Engine.Rendering.Services;
using Engine.Rendering.Standard.Shaders.Pipelines;

namespace Engine.Rendering.Standard.Shaders.Bundles;
internal class Entity3ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) {
		AddPipeline( "default", pipelineService.Get<Entity3ShaderPipeline>() );
	}
}
