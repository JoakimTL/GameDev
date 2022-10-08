namespace Engine.Rendering.Standard.UI.Standard.Shaders;

public class UIShaderProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "ui/uiShader.frag" ] );
}