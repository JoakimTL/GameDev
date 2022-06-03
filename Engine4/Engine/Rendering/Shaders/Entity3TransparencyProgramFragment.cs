namespace Engine.Rendering.Shaders;

public class Entity3TransparencyProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/geometry3transparent.frag" ] );
}

public class Entity3DirectionalTransparencyProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/geometry3directionaltransparent.frag" ] );
}
