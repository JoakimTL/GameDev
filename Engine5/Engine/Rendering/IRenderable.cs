namespace Engine.Rendering;

public interface IRenderable
{
    event Action<IRenderable>? RenderableSceneChanged;
    string? SceneName { get; }
    event Action<IRenderable>? RenderableDataChanged;
    string? MaterialAssetName { get; }
    string? MeshDataAssetName { get; }
    IRenderableInstanceData? InstanceData { get; }
}
