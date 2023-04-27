using Engine.GameLogic.ECPS;
using Engine.GlobalServices;
using Engine.Rendering;

namespace StandardPackage.ECPS.Components;

public sealed class RenderableComponent : ComponentBase, IRenderable
{
	private readonly RenderMaterialAssetComponent _materialComponent;
	private readonly RenderMeshAssetComponent _meshComponent;
	private readonly RenderSceneComponent _sceneComponent;
	private readonly RenderInstanceDataComponent _instanceDataComponent;

	public string? MaterialAssetName => _materialComponent.AssetName;
	public string? MeshDataAssetName => _meshComponent.AssetName;
	public string? SceneName => _sceneComponent.SceneIdentity;
	public IRenderableInstanceData? InstanceData => _instanceDataComponent;

	public event Action<IRenderable>? RenderableSceneChanged;
	public event Action<IRenderable>? RenderableDataChanged;

	public RenderableComponent(RenderMaterialAssetComponent materialComponent, RenderMeshAssetComponent meshComponent, RenderSceneComponent sceneComponent, RenderInstanceDataComponent instanceDataComponent)
	{
		_materialComponent = materialComponent;
		_meshComponent = meshComponent;
		_sceneComponent = sceneComponent;
		_instanceDataComponent = instanceDataComponent;
		_sceneComponent.ComponentChanged += OnSceneComponentChanged;
		_materialComponent.ComponentChanged += OnRenderDataComponentChanged;
		_meshComponent.ComponentChanged += OnRenderDataComponentChanged;
		_instanceDataComponent.ComponentChanged += OnRenderDataComponentChanged;
	}

	private void OnSceneComponentChanged(ComponentBase component) => RenderableSceneChanged?.Invoke(this);
	private void OnRenderDataComponentChanged(ComponentBase component) => RenderableDataChanged?.Invoke(this);

	protected override void OnOwnerChanged() => Engine.Global.Get<RenderableContainerService>().Add(this);
	protected override void OnDispose() => Engine.Global.Get<RenderableContainerService>().Remove(this);

}
