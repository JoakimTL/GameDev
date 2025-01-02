using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.SingleTextureRendering.Shaders;

public sealed class SingleTextureRenderingShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) {
		AddPipeline( "default",  pipelineService.Get<SingleTextureRenderingShaderPipeline>() );
	}
}
