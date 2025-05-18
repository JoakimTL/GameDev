using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Shaders;

public sealed class TexturedShade2ShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => true;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<TexturedShade2VertexShaderProgram>();
		yield return shaderProgramService.Get<TexturedShadeFragmentShaderProgram>();
	}
}
