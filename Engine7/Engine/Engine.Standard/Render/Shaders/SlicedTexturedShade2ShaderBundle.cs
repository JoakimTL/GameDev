using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Shaders;

[Identity( nameof( SlicedTexturedShade2ShaderBundle ) )]
public sealed class SlicedTexturedShade2ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<SlicedTexturedShade2ShaderPipeline>() );
}
