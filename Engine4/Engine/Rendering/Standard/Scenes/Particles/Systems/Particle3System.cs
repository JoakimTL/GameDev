using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.Scenes.Particles.Systems;

public class Particle3System : ParticleSystem<Vertex2, Particle3Data> {
	public Particle3System( uint maxParticles ) : base( Resources.Render.Shader.Bundles.Get<Particle3ShaderBundle>(), Resources.Render.Mesh2.Square, maxParticles ) {
	}
}
