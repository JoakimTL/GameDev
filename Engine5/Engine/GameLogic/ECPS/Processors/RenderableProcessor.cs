using Engine.GameLogic.ECPS.Components;

namespace Engine.GameLogic.ECPS.Processors;
public class RenderableProcessor : ProcessorBase<RenderableComponent>
{
    public RenderableProcessor() : base(typeof(RenderMeshAssetComponent), typeof(RenderShaderAssetComponent))
    {
    }
}
