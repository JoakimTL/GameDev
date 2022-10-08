namespace Engine.Rendering.Standard.UI.Standard.Shaders;

public class UIShaderProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "ui/uiShader.vert" ] );
}
