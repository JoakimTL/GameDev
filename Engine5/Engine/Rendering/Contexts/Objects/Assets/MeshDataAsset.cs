using Engine.Rendering.Contexts.Objects.Meshes;

namespace Engine.Rendering.Contexts.Objects.Assets;

public sealed class MeshDataAsset : LoadedAssetBase
{

    private LoadedAssetMesh? _mesh;

    internal MeshDataAsset(string path) : base(path)
    {
        _mesh = null;
    }

    internal void SetMesh(LoadedAssetMesh newMesh)
    {
        _mesh = newMesh;
    }

    protected override void OnDispose()
    {
        _mesh?.Dispose();
        _mesh = null;
    }
}
