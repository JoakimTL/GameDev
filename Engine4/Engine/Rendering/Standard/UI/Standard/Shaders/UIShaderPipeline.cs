namespace Engine.Rendering.Standard.UI.Standard.Shaders;

public class UIShaderPipeline : ShaderPipeline {
	public UIShaderPipeline() : base( typeof( UIShaderProgramVertex ), typeof( UIShaderProgramFragment ) ) { }
}
