namespace Engine.Rendering.Standard.Scenes.Particles.Systems.Shaders;

public class Particle3ProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/particle3.vert" ] );
}
