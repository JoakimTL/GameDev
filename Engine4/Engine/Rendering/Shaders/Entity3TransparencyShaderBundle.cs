using Engine.Rendering.Standard;

namespace Engine.Rendering.Shaders;

[Identification( "f99a1c86-0752-4573-8023-288342744284" )]
public class Entity3TransparencyShaderBundle : ShaderBundle {
	public Entity3TransparencyShaderBundle() : base( 
		(0, Resources.Render.Shader.Pipelines.Get<Entity3TransparencyShader>()),
		(1, Resources.Render.Shader.Pipelines.Get<Entity3DirectionalTransparencyShader>())
	) { }

	public override bool UsesTransparency => true;
}
