namespace Engine.Rendering.Standard.Scenes.VerletParticles.Systems.Shaders;

public class VerletParticle3ProgramFragment : ShaderProgram {
	protected override void AttachShaders() => AttachShader( Resources.Render.Shader.Sources[ "geom/particle3verlet.frag" ] );
}