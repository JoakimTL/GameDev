using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.SingleTextureRendering.Shaders;

public sealed class SingleTextureRenderingShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<SingleTextureRenderingVertexShaderProgram>();
		yield return shaderProgramService.Get<SingleTextureRenderingFragmentShaderProgram>();
	}
}
