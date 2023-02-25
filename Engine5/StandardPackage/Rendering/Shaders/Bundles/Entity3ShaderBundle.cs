using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Services;
using StandardPackage.Rendering.Shaders.Pipelines;

namespace StandardPackage.Rendering.Shaders.Bundles;
public sealed class Entity3ShaderBundle : ShaderBundleBase
{
    protected override void AddPipelines(ShaderPipelineService pipelineService)
    {
        AddPipeline("default", pipelineService.Get<Entity3ShaderPipeline>());
    }
}
