using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Text.Fonts.Shaders;

[Identity( nameof( GlyphShaderBundle ) )]
public sealed class GlyphShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<GlyphShaderPipeline>() );
}
