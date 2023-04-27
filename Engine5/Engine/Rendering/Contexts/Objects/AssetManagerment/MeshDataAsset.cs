//using Engine.Rendering.Contexts.Objects.Assets;
//using Engine.Rendering.Contexts.Objects.Meshes;

//namespace Engine.Rendering.Contexts.Objects.AssetManagerment;

//public sealed class MeshDataAsset : LoadedAssetBase
//{

//    private LoadedAssetMesh _mesh;
//    public Type? VertexType => _mesh.VertexType;
//    public IMesh? Mesh => _mesh;

//    internal MeshDataAsset(string path, LoadedAssetMesh newMesh) : base(path)
//    {
//        _mesh = newMesh;
//    }

//    protected override void OnDispose()
//    {
//        _mesh.Dispose();
//    }
//}
