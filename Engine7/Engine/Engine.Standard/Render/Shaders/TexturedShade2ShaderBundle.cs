using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Shaders;

[Identity( nameof( TexturedShade2ShaderBundle ) )]
public sealed class TexturedShade2ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<TexturedShade2ShaderPipeline>() );
}
