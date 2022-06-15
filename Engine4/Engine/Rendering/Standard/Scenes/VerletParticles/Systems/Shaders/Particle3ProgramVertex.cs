namespace Engine.Rendering.Standard.Scenes.VerletParticles.Systems.Shaders;

public class VerletParticle3ProgramVertex : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/particle3verlet.vert" ] );
}
