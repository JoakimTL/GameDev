using Engine.Rendering.Objects;
using Engine.Rendering.Services;
using Engine.Rendering.Standard.Shaders.Programs;

namespace Engine.Rendering.Standard.Shaders.Pipelines;
internal class Entity3ShaderPipeline : ShaderPipelineBase {
	public override bool UsesTransparency => false;

	protected override IEnumerable<ShaderProgramBase> GetShaderPrograms( ShaderProgramService shaderProgramService ) {
		yield return shaderProgramService.Get<Entity3VertexShaderProgram>();
		yield return shaderProgramService.Get<Entity3FragmentShaderProgram>();
	}
}
