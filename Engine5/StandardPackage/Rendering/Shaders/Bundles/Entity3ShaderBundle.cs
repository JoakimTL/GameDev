using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Services;
using Engine.Structure.Attributes;
using StandardPackage.Rendering.Shaders.Pipelines;

namespace StandardPackage.Rendering.Shaders.Bundles;

[Identity(nameof(Entity3ShaderBundle))]
public sealed class Entity3ShaderBundle : ShaderBundleBase
{
    protected override void AddPipelines(ShaderPipelineService pipelineService)
    {
        AddPipeline("default", pipelineService.Get<Entity3ShaderPipeline>());
    }
}
