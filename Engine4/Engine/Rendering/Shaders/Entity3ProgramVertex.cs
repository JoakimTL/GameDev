namespace Engine.Rendering.Shaders;

public class Entity3ProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/geometry3.vert" ] );
}
