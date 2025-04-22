using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Civs.Render.World.Lines;

public sealed class Line3ShaderPipeline : OglShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<OglShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<Line3VertexShaderProgram>();
		yield return shaderProgramService.Get<LineFragmentShaderProgram>();
	}
}
