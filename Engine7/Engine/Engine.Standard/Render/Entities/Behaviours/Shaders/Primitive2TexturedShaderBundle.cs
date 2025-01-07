using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Entities.Behaviours.Shaders;

[Identity( nameof( Primitive2TexturedShaderBundle ) )]
public sealed class Primitive2TexturedShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<Primitive2TexturedShaderPipeline>() );
}
