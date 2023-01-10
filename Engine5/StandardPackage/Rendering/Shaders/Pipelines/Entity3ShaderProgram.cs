using Engine.Rendering.Objects;
using Engine.Rendering.Services;
using StandardPackage.Rendering.Shaders.Programs;

namespace StandardPackage.Rendering.Shaders.Pipelines;
public sealed class Entity3ShaderPipeline : ShaderPipelineBase
{
    public override bool UsesTransparency => false;

    protected override IEnumerable<ShaderProgramBase> GetShaderPrograms(ShaderProgramService shaderProgramService)
    {
        yield return shaderProgramService.Get<Entity3VertexShaderProgram>();
        yield return shaderProgramService.Get<Entity3FragmentShaderProgram>();
    }
}
