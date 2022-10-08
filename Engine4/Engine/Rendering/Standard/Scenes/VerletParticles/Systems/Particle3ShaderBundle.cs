using Engine.Rendering.Standard.Scenes.VerletParticles.Systems.Shaders;

namespace Engine.Rendering.Standard.Scenes.VerletParticles.Systems;

[Identification( "b6c2a0a7-7631-4ec1-99c4-fda818af6525" )]
public class VerletParticle3ShaderBundle : ShaderBundle {
	public VerletParticle3ShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.GetOrAdd<VerletParticle3Shader>()) ) { }

	public override bool UsesTransparency => true;
}
