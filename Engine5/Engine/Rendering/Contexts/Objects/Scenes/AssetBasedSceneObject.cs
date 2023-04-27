//using Engine.Rendering.Contexts.Objects.AssetManagerment;
//using Engine.Rendering.Contexts.Objects.Assets;
//using Engine.Rendering.Contexts.Services;
//using System.Runtime.InteropServices;

//namespace Engine.Rendering.Contexts.Objects.Scenes;

//public sealed class AssetBasedSceneObject : SceneObjectBase
//{
//	private IRenderable _renderable;
//	private string? _currentMeshAsset;
//	private string? _currentMaterialAsset;
//	private Type? _meshDataType;
//	private Type? _instanceDataType;
//	private SceneInstanceData? _instanceData;
//	private ReferenceContainer<MeshDataAsset>? _currentMesh;
//	private MaterialAsset? _currentMaterial;
//	public event Action<AssetBasedSceneObject>? RequireUpdate;

//	public AssetBasedSceneObject(IRenderable renderable)
//	{
//		_renderable = renderable;
//		_renderable.RenderableDataChanged += OnChanged;
//	}

//	private void OnChanged(IRenderable obj) => RequireUpdate?.Invoke(this);

//	public void Update(MeshDataAssetService meshDataAssetService, MaterialAssetService materialAssetService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, VertexArrayLayoutService vertexArrayLayoutService, RenderBufferObjectService renderBufferObjectService)
//	{
//		var earlierMeshDataType = _meshDataType;
//		var earlierInstanceDataType = _instanceDataType;
//		uint numTextures = 0;
//		if (_currentMaterialAsset != _renderable.MaterialAssetName)
//		{
//			_currentMaterialAsset = _renderable.MaterialAssetName;
//			_currentMaterial = _currentMaterialAsset is not null ? materialAssetService.Get(_currentMaterialAsset) : null;
//			SetShaders(_currentMaterial?.Shader?.ShaderBundle);
//			if (_currentMaterialAsset is not null)
//			{
//				_currentMaterial = materialAssetService.Get(_currentMaterialAsset);
//					SetShaders(_currentMaterial?.Shader?.ShaderBundle);
//			}
//		}

//		if (_currentMeshAsset != _renderable.MeshDataAssetName)
//		{
//			_currentMeshAsset = _renderable.MeshDataAssetName;
//			_currentMesh = _currentMeshAsset is not null ? meshDataAssetService.Get(_currentMeshAsset) : null;
//			SetMesh(_currentMesh?.Value.Mesh);
//			_meshDataType = _currentMesh?.Value.VertexType;
//		}

//		_instanceDataType = _renderable.InstanceData?.InstanceDataType;
//		if ((earlierMeshDataType != _meshDataType || earlierInstanceDataType != _instanceDataType) && _meshDataType is not null && _instanceDataType is not null)
//			SetVertexArrayObject(compositeVertexArrayObjectService.Get(new[] { _meshDataType, _instanceDataType }));

//		if (_instanceDataType is not null)
//		{
//			var instanceLayout = vertexArrayLayoutService.Get(_instanceDataType);
//			var instanceData = _renderable.InstanceData?.GetData();
//			if (instanceLayout is not null && instanceData is not null)
//			{
//				uint unmanagedInstanceDataSizeBytes = (uint)Marshal.SizeOf(_instanceDataType);
//				uint instanceSizeBytes = (uint)instanceLayout.StrideBytes;
//				uint numInstances = (uint)(instanceData.Length / unmanagedInstanceDataSizeBytes);
//				if (AdjustInstanceDataSegment(_instanceDataType, instanceSizeBytes, numInstances * instanceSizeBytes, renderBufferObjectService) && _instanceData is not null)
//				{
//					unsafe
//					{
//						byte* currentInstanceData = stackalloc byte[(int)instanceSizeBytes];
//						Span<byte> span = new Span<byte>(currentInstanceData, (int)instanceSizeBytes);
//						fixed (byte* srcPtr = instanceData)
//						{
//							for (uint i = 0; i < numInstances; i++)
//							{
//								Buffer.MemoryCopy(srcPtr + i * unmanagedInstanceDataSizeBytes, currentInstanceData, instanceSizeBytes, unmanagedInstanceDataSizeBytes);
//								for (int t = 0; t < numTextures; t++)
//								{

//								}
//								_instanceData.SetInstances(i * instanceSizeBytes, span);
//							}
//						}
//					}
//					_instanceData?.SetInstances(0, instanceData);
//				}
//			}
//		}
//	}

//	private bool AdjustInstanceDataSegment(Type instanceDataType, uint instanceSizeBytes, uint instanceDataLengthBytes, RenderBufferObjectService renderBufferObjectService)
//	{
//		if (_instanceData is null)
//		{
//			var segment = renderBufferObjectService.Get(instanceDataType).AllocateSegment(instanceDataLengthBytes);
//			if (segment is null)
//				return false;
//			_instanceData = new(segment, instanceSizeBytes);
//		}
//		else if (_instanceData.MaxInstances < instanceDataLengthBytes / instanceSizeBytes)
//		{
//			var segment = renderBufferObjectService.Get(instanceDataType).AllocateSegment(instanceDataLengthBytes);
//			if (segment is null)
//				return false;
//			_instanceData.SetSegment(segment, instanceSizeBytes);
//		}
//		return true;
//	}

//	public override void Bind()
//	{

//	}

//}