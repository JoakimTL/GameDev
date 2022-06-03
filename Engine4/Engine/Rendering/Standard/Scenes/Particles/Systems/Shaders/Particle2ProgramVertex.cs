namespace Engine.Rendering.Standard.Scenes.Particles.Systems.Shaders;

public class Particle2ProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/particle2.vert" ] );
}
