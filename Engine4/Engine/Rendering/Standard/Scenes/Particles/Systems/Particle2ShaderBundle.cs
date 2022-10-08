using Engine.Rendering.Standard.Scenes.Particles.Systems.Shaders;

namespace Engine.Rendering.Standard.Scenes.Particles.Systems;

[Identification( "71db0738-9fd5-4d93-aaeb-87f8bd33454e" )]
public class Particle2ShaderBundle : ShaderBundle {
	public Particle2ShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.GetOrAdd<Particle2Shader>()) ) { }

	public override bool UsesTransparency => true;
}
