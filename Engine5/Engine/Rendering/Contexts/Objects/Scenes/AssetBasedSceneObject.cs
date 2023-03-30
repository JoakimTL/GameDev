using Engine.Rendering.Contexts.Services;
using Engine.Structure.Interfaces;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public sealed class AssetBasedSceneObject : SceneObjectBase
{
	private IRenderable _renderable;
	private string? _currentMeshAsset;
	private string? _currentMaterialAsset;
	public event Action<AssetBasedSceneObject>? RequireUpdate;

	public AssetBasedSceneObject(IRenderable renderable)
	{
		_renderable = renderable;
		_renderable.RenderableDataChanged += OnChanged;
	}

	private void OnChanged(IRenderable obj) => RequireUpdate?.Invoke(this);

	public void Update(MeshDataAssetService meshDataAssetService, MaterialAssetService materialAssetService, )
	{
		SetMesh(_renderable.?.Mesh);
		SetShaders(_renderable.MaterialAsset?.Shader?.ShaderBundle);
		SetVertexArrayObject(_renderable.MaterialAsset?.Shader?.ShaderBundle);
	}

	public override void Bind()
	{

	}

}