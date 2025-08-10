using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;
using Engine;

namespace Civlike.World.Render.Shaders;

[Identity( nameof( GlobeTerrainShaderBundle ) )]
public sealed class GlobeTerrainShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<GlobeTerrainShaderPipeline>() );
}

public sealed class GlobeTerrainShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<GlobeTerrainVertexShaderProgram>();
		yield return shaderProgramService.Get<GlobeTerrainFragmentShaderProgram>();
	}
}

public sealed class GlobeTerrainVertexShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "globeTerrain.vert" ) );
}
public sealed class GlobeTerrainFragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "globeTerrain.frag" ) );
}

