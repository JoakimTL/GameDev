using Engine.Rendering.Standard.Scenes.Particles.Systems.Shaders;

namespace Engine.Rendering.Standard.Scenes.Particles.Systems;

[Identification( "e270f8fe-d874-4600-8985-6ea5cfc034fa" )]
public class Particle3ShaderBundle : ShaderBundle {
	public Particle3ShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.GetOrAdd<Particle3Shader>()) ) { }

	public override bool UsesTransparency => true;
}
