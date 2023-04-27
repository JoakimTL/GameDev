//using Engine.GlobalServices;
//using Engine.Rendering.Contexts.Objects.AssetManagerment;
//using Engine.Rendering.Contexts.Objects.Meshes;

//namespace Engine.Rendering.Contexts.Services;

//public sealed class MeshDataAssetService : LoadedAssetService<MeshDataAsset>
//{
//	private readonly MeshLoadingService _meshLoadingService;

//	public MeshDataAssetService(MeshLoadingService meshLoadingService, ReferenceCountingService referenceCountingService) : base("assets/meshes/", referenceCountingService)
//	{
//		this._meshLoadingService = meshLoadingService;
//	}

//	protected override void LoadAsset(MeshDataAsset asset)
//	{
//		LoadedAssetMesh? mesh = _meshLoadingService.Load(asset.Path);
//		if (mesh is null)
//		{
//			this.LogWarning($"Unable to load mesh for {asset}!");
//			return;
//		}
//		asset.SetMesh(mesh);
//	}

//	protected override MeshDataAsset CreateAsset(string path) => new(path);
//}
