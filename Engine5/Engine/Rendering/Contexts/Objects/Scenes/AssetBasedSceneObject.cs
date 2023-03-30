using Engine.Structure.Interfaces;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public sealed class AssetBasedSceneObject : SceneObjectBase, IUpdateable
{
	private IRenderable _renderable;
	public event Action<AssetBasedSceneObject>? RequireUpdate;

	public AssetBasedSceneObject(IRenderable renderable)
	{
		_renderable = renderable;
		_renderable.RenderableChanged += OnChanged;
	}

	private void OnChanged(IRenderable obj)
	{
		RequireUpdate?.Invoke(this);
	}

	public void Update(float time, float deltaTime)
	{
		SetMesh(_renderable.MeshDataAsset?.Mesh);
		SetShaders(_renderable.MaterialAsset?.Shader?.ShaderBundle);
		SetVertexArrayObject(_renderable.MaterialAsset?.Shader?.ShaderBundle);
	}

	public override void Bind()
	{

	}

}