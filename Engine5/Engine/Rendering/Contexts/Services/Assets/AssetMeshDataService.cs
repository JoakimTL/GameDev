using Engine.Rendering.Contexts.Objects.Meshes;

namespace Engine.Rendering.Contexts.Services.Assets;

public sealed class AssetMeshDataService : ExternalAssetReferenceService<string, LoadedAssetMesh>, IDisposable
{
    private readonly MeshLoadingService _meshLoadingService;
    private readonly Dictionary<string, LoadedAssetMesh> _meshes;

    public AssetMeshDataService(MeshLoadingService meshLoadingService) : base("assets\\meshes\\", ".obj")
    {
        this._meshLoadingService = meshLoadingService;
        _meshes = new();
    }

    /// <summary>
    /// A requested texture should also be <see cref="Discarded(string)"/> when no longer in use.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    protected override LoadedAssetMesh? OnRequest(string filePath)
    {
        if (_meshes.TryGetValue(filePath, out LoadedAssetMesh? mesh))
        {
            AddReference(filePath);
            return mesh;
        }
        mesh = _meshLoadingService.Load(filePath);
        if (mesh is null)
            return this.LogWarningThenReturnDefault<LoadedAssetMesh>($"Unable to locate mesh data file {filePath}!");
        _meshes.Add(filePath, mesh);
        AddReference(filePath);
        return mesh;
    }

    protected override void OnDiscarded(string filePath)
    {
        if (!RemoveReference(filePath))
            return;
        if (_meshes.Remove(filePath, out LoadedAssetMesh? mesh))
            mesh.Dispose();
    }

    public void Dispose()
    {
        foreach (var mesh in _meshes.Values)
            mesh.Dispose();
        GC.SuppressFinalize(this);
    }
}
