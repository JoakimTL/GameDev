using Engine.GameLogic.ECPS;
using StandardPackage.ECPS.Components;

namespace StandardPackage.ECPS.Processors;
public class RenderableProcessor : ProcessorBase<RenderableComponent>
{
	public RenderableProcessor() : base(typeof(RenderMeshAssetComponent), typeof(RenderMaterialAssetComponent), typeof(RenderSceneComponent), typeof(RenderInstance3DataComponent))
	{
	}
}
