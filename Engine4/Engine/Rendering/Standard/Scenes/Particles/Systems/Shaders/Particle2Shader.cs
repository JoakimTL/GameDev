namespace Engine.Rendering.Standard.Scenes.Particles.Systems.Shaders;

public class Particle2Shader : ShaderPipeline {
	public Particle2Shader() : base( typeof( Particle2ProgramVertex ), typeof( Particle2ProgramFragment ) ) { }
}
