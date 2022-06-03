namespace Engine.Rendering.Shaders;
public class PfxShaderProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "pfx.vert" ] );
}
