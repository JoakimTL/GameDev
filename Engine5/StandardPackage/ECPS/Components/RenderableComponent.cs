using Engine.GameLogic.ECPS;
using Engine.GlobalServices;
using Engine.Rendering;

namespace StandardPackage.ECPS.Components;

public sealed class RenderableComponent : ComponentBase, IRenderable
{
	private readonly RenderMaterialAssetComponent _materialComponent;
	private readonly RenderMeshAssetComponent _meshComponent;
	private readonly RenderSceneComponent _sceneComponent;
	private readonly RenderInstance3DataComponent _instanceDataComponent;

	public string? MaterialAssetName => this._materialComponent.AssetName;
	public string? MeshDataAssetName => this._meshComponent.AssetName;
	public string? SceneName => this._sceneComponent.SceneIdentity;
	public IRenderableInstanceData? InstanceData => this._instanceDataComponent;

	public event Action<IRenderable>? RenderableSceneChanged;
	public event Action<IRenderable>? RenderableDataChanged;

	public RenderableComponent(RenderMaterialAssetComponent materialComponent, RenderMeshAssetComponent meshComponent, RenderSceneComponent sceneComponent, RenderInstance3DataComponent instanceDataComponent)
	{
		this._materialComponent = materialComponent;
		this._meshComponent = meshComponent;
		this._sceneComponent = sceneComponent;
		this._instanceDataComponent = instanceDataComponent;
		this._sceneComponent.ComponentChanged += OnSceneComponentChanged;
		this._materialComponent.ComponentChanged += OnRenderDataComponentChanged;
		this._meshComponent.ComponentChanged += OnRenderDataComponentChanged;
		this._instanceDataComponent.ComponentChanged += OnRenderDataComponentChanged;
	}

	private void OnSceneComponentChanged(ComponentBase component) => RenderableSceneChanged?.Invoke(this);
	private void OnRenderDataComponentChanged(ComponentBase component) => RenderableDataChanged?.Invoke(this);

	protected override void OnOwnerChanged() => Engine.Global.Get<RenderableContainerService>().Add(this);
	protected override void OnDispose() => Engine.Global.Get<RenderableContainerService>().Remove(this);

}
