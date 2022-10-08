using Engine.Rendering.Standard;

namespace Engine.Rendering.Shaders;

[Identification( "e0d49716-c34e-4d1e-97a2-20382ab6060a" )]
public class Entity3ShaderBundle : ShaderBundle {
	public Entity3ShaderBundle() : base(
		(0, Resources.Render.Shader.Pipelines.GetOrAdd<Entity3Shader>()),
		(1, Resources.Render.Shader.Pipelines.GetOrAdd<Entity3DirectionalShader>())
	 ) { }

	public override bool UsesTransparency => false;
}

[Identification( "6d42491e-0f65-46e5-b340-2ebd0179d9cc" )]
public class Entity2ShaderBundle : ShaderBundle {
	public Entity2ShaderBundle() : base(
		(0, Resources.Render.Shader.Pipelines.GetOrAdd<Entity2Shader>())
	 ) { }

	public override bool UsesTransparency => false;
}
