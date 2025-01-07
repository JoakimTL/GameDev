using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Entities.Behaviours.Shaders;

public sealed class Primitive2TexturedShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => true;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<Primitive2VertexShaderProgram>();
		yield return shaderProgramService.Get<TexturedShadeFragmentShaderProgram>();
	}
}
