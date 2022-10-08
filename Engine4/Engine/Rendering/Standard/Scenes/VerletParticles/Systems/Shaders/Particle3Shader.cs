namespace Engine.Rendering.Standard.Scenes.VerletParticles.Systems.Shaders;

public class VerletParticle3Shader : ShaderPipeline {
	public VerletParticle3Shader() : base( typeof( VerletParticle3ProgramVertex ), typeof( VerletParticle3ProgramFragment ) ) { }
}
