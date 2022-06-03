namespace Engine.Rendering.Shaders;

public class Entity3ProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/geometry3.frag" ] );
}

public class Entity3DirectionalProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/geometry3directional.frag" ] );
}
