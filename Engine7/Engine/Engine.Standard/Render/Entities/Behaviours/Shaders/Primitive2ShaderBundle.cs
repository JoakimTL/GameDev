using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Standard.Render.Entities.Behaviours.Shaders;
[Identity( nameof( Primitive2ShaderBundle ) )]
public sealed class Primitive2ShaderBundle : ShaderBundleBase {
	protected override void AddPipelines( ShaderPipelineService pipelineService ) => AddPipeline( "default", pipelineService.Get<Primitive2ShaderPipeline>() );
}

public sealed class Primitive2ShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<Primitive2VertexShaderProgram>();
		yield return shaderProgramService.Get<Primitive2FragmentShaderProgram>();
	}
}

public sealed class Primitive2VertexShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "primitive2.vert" ) );
}
public sealed class Primitive2FragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "primitive2.frag" ) );
}
