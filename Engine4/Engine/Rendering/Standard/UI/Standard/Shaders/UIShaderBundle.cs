namespace Engine.Rendering.Standard.UI.Standard.Shaders;

[Identification( "a6bc9616-05c6-4f5a-b568-caa1cd89fa82" )]
public class UIShaderBundle : ShaderBundle {
	public UIShaderBundle() : base( (0, Resources.Render.Shader.Pipelines.GetOrAdd<UIShaderPipeline>()) ) { }

	public override bool UsesTransparency => true;
}
