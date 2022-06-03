using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.Scenes.Particles.Systems;
public class Particle2System : ParticleSystem<Vertex2, Particle2Data> {
	public Particle2System( uint maxParticles ) : base( Resources.Render.Shader.Bundles.Get<Particle2ShaderBundle>(), Resources.Render.Mesh2.Square, maxParticles ) {
	}
}
