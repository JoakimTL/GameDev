using Engine.Rendering.Standard;

namespace Engine.Rendering.Shaders;

[Identification( "e0d49716-c34e-4d1e-97a2-20382ab6060a" )]
public class Entity3ShaderBundle : ShaderBundle {
	public Entity3ShaderBundle() : base( "Entity3",
		(0, Resources.Render.Shader.Pipelines.Get<Entity3Shader>()),
		(1, Resources.Render.Shader.Pipelines.Get<Entity3DirectionalShader>())
	 ) { }

	public override bool UsesTransparency => false;
}
