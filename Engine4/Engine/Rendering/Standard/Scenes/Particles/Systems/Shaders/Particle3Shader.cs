namespace Engine.Rendering.Standard.Scenes.Particles.Systems.Shaders;

public class Particle3Shader : ShaderPipeline {
	public Particle3Shader() : base( typeof( Particle3ProgramVertex ), typeof( Particle3ProgramFragment ) ) { }
}
