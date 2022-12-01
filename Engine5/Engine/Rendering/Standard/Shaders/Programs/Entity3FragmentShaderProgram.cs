using Engine.Rendering.Objects;
using Engine.Rendering.Services;

namespace Engine.Rendering.Standard.Shaders.Programs;

public class Entity3FragmentShaderProgram : ShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) {
		ShaderSource? source = shaderSourceService.Get( "assets/shaders/geometry3.frag" );
		if ( source is null )
			return;
		AttachShader( source );
	}
}
