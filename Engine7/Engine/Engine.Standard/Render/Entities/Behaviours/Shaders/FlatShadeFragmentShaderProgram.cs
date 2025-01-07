using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Entities.Behaviours.Shaders;

public sealed class FlatShadeFragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => AttachShader( shaderSourceService.GetOrThrow( "flatShade.frag" ) );
}
