using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;
using Engine;

namespace Civlike.Client.Render.World.Lines;

[Identity( nameof( Line3ShaderBundle ) )]
public sealed class Line3ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<Line3ShaderPipeline>() );
}
