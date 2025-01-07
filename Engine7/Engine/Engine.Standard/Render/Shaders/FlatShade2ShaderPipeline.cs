using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Shaders;

public sealed class FlatShade2ShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => true;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<FlatShade2VertexShaderProgram>();
		yield return shaderProgramService.Get<FlatShadeFragmentShaderProgram>();
	}
}
