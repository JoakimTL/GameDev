using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.SingleTextureRendering.Shaders;

public sealed class SingleTextureRenderingFragmentShaderProgram : OglShaderProgramBase {
	protected override void AttachShaders( ShaderSourceService shaderSourceService ) => this.AttachShader( shaderSourceService.GetOrThrow( "singleTextureRendering.frag" ) );
}