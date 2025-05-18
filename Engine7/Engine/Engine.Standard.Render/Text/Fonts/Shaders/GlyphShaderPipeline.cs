using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Text.Fonts.Shaders;

public sealed class GlyphShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<GlyphVertexShaderProgram>();
		yield return shaderProgramService.Get<GlyphFragmentShaderProgram>();
	}
}
