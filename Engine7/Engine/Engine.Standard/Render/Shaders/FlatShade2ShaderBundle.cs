using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Shaders;

[Identity( nameof( FlatShade2ShaderBundle ) )]
public sealed class FlatShade2ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<FlatShade2ShaderPipeline>() );
}
