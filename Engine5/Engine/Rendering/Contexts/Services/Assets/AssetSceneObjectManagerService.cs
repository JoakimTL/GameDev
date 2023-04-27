using Engine.GlobalServices;
using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Objects.AssetManagerment;
using Engine.Rendering.Contexts.Objects.Meshes;
using Engine.Rendering.Contexts.Objects.Scenes;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Services.Assets;

public sealed class AssetSceneObjectManagerService : Identifiable, IContextService, IUpdateable
{
    private readonly RenderableContainerService _renderableContainerService;
    private readonly SceneService _sceneService;
    private readonly AssetMaterialService _assetMaterialService;
    private readonly AssetMeshDataService _assetMeshDataService;
    private readonly VertexArrayLayoutService _vertexArrayLayoutService;
    private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService;
    private readonly RenderBufferObjectService _renderBufferObjectService;
    private readonly HashSet<IRenderable> _doubleUpdatePreventionSet;
    private readonly ConcurrentQueue<IRenderable> _incomingRenderables;
    private readonly ConcurrentQueue<IRenderable> _outgoingRenderables;
    private readonly Dictionary<IRenderable, AssetSceneObject> _sceneObjects;
    private readonly ConcurrentQueue<IRenderable> _updatedRenderablesData;
    private readonly ConcurrentQueue<IRenderable> _updatedRenderablesScene;
    private readonly Dictionary<IRenderable, Scene> _renderableScene;

    public AssetSceneObjectManagerService(RenderableContainerService renderableContainerService, SceneService sceneService, AssetMaterialService assetMaterialService, AssetMeshDataService assetMeshDataService, VertexArrayLayoutService vertexArrayLayoutService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, RenderBufferObjectService renderBufferObjectService)
    {
        _renderableContainerService = renderableContainerService;
        _sceneService = sceneService;
        _assetMaterialService = assetMaterialService;
        _assetMeshDataService = assetMeshDataService;
        _vertexArrayLayoutService = vertexArrayLayoutService;
        _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
        _renderBufferObjectService = renderBufferObjectService;

        _doubleUpdatePreventionSet = new();
        _incomingRenderables = new();
        _outgoingRenderables = new();
        _sceneObjects = new();
        _updatedRenderablesData = new();
        _updatedRenderablesScene = new();
        _renderableScene = new();

        _renderableContainerService.AddedRenderable += RenderableAdded;
        _renderableContainerService.RemovedRenderable += RenderableRemoved;

        foreach (var renderable in _renderableContainerService.Renderables)
            _incomingRenderables.Enqueue(renderable);
    }

    private void RenderableAdded(IRenderable obj) => _incomingRenderables.Enqueue(obj);
    private void RenderableRemoved(IRenderable obj) => _outgoingRenderables.Enqueue(obj);
    private void RenderableDataChanged(IRenderable obj) => _updatedRenderablesData.Enqueue(obj);
    private void RenderableSceneChanged(IRenderable obj) => _updatedRenderablesScene.Enqueue(obj);

    private void SetRenderableScene(IRenderable renderable, ISceneObject so)
    {
        if (renderable.SceneName is null)
            return;
        var scene = _sceneService.Get(renderable.SceneName);
        if (scene is null)
            return;
        scene.Add(so);
        _renderableScene[renderable] = scene;
    }

    public void Update(float time, float deltaTime)
    {
        _doubleUpdatePreventionSet.Clear();
        while (_outgoingRenderables.TryDequeue(out IRenderable? renderable))
        {
            if (!_doubleUpdatePreventionSet.Add(renderable))
                continue;
            renderable.RenderableDataChanged -= RenderableDataChanged;
            renderable.RenderableSceneChanged -= RenderableSceneChanged;
            if (_sceneObjects.TryGetValue(renderable, out AssetSceneObject? so))
                so.Dispose();
            _sceneObjects.Remove(renderable);
            _renderableScene.Remove(renderable);
        }

        _doubleUpdatePreventionSet.Clear();
        while (_incomingRenderables.TryDequeue(out IRenderable? renderable))
        {
            if (!_doubleUpdatePreventionSet.Add(renderable))
                continue;
            if (_sceneObjects.ContainsKey(renderable))
                continue;
            renderable.RenderableDataChanged += RenderableDataChanged;
            renderable.RenderableSceneChanged += RenderableSceneChanged;
            AssetSceneObject so = new(renderable);
            _sceneObjects.Add(renderable, so);
            SetRenderableScene(renderable, so);
            _updatedRenderablesData.Enqueue(renderable);
        }

        _doubleUpdatePreventionSet.Clear();
        while (_updatedRenderablesScene.TryDequeue(out IRenderable? renderable))
            if (_doubleUpdatePreventionSet.Add(renderable) && _sceneObjects.TryGetValue(renderable, out var so))
                SetRenderableScene(renderable, so);

        _doubleUpdatePreventionSet.Clear();
        while (_updatedRenderablesData.TryDequeue(out IRenderable? renderable))
        {
            if (!_doubleUpdatePreventionSet.Add(renderable))
                continue;
            if (!_sceneObjects.TryGetValue(renderable, out var so))
                continue;
            MaterialAsset? newMaterial = so.CurrentMaterial;
            var newMaterialAssetPath = _assetMaterialService.GetProperAssetPath(renderable.MaterialAssetName);
            if (so.CurrentMaterial?.MaterialAssetName != newMaterialAssetPath)
            {
                if (so.CurrentMaterial?.MaterialAssetName is not null)
                    _assetMaterialService.Discarded(so.CurrentMaterial.MaterialAssetName);
                newMaterial = newMaterialAssetPath is not null ? _assetMaterialService.Request(newMaterialAssetPath) : null;
            }
            LoadedAssetMesh? newMesh = so.CurrentMesh;
            var newMeshAssetPath = _assetMeshDataService.GetProperAssetPath(renderable.MeshDataAssetName);
            if (so.CurrentMesh?.MeshDataAssetName != newMeshAssetPath)
            {
                if (so.CurrentMesh?.MeshDataAssetName is not null)
                    _assetMeshDataService.Discarded(so.CurrentMesh.MeshDataAssetName);
                newMesh = newMeshAssetPath is not null ? _assetMeshDataService.Request(newMeshAssetPath) : null;
            }
            var newVao = GetVao(newMesh?.VertexType, renderable.InstanceData?.InstanceDataType);
            so.Set(newMaterial, newMesh, newVao);

            if (renderable.InstanceData?.InstanceDataType is not null)
            {
                var instanceDataType = renderable.InstanceData.InstanceDataType;
                var instanceLayout = _vertexArrayLayoutService.Get(instanceDataType);
                var instanceData = renderable.InstanceData.GetData();
                if (instanceLayout is not null && instanceData is not null)
                {
                    uint incompleteInstanceSizeBytes = (uint)Marshal.SizeOf(instanceDataType);
                    uint instanceSizeWithTexturesBytes = (uint)instanceLayout.StrideBytes;
                    uint numInstances = (uint)(instanceData.Length / incompleteInstanceSizeBytes);

                    if (so.InstanceData is null)
                        so.SetSceneInstanceData(new(_renderBufferObjectService.Get(instanceDataType).AllocateSegment(numInstances * instanceSizeWithTexturesBytes).NotNull(), instanceSizeWithTexturesBytes, true), instanceDataType);
                    else if (so.InstanceDataType != instanceDataType || so.InstanceData.MaxInstances < numInstances)
                        so.SetSceneInstanceDataSegment(_renderBufferObjectService.Get(instanceDataType).AllocateSegment(numInstances * instanceSizeWithTexturesBytes).NotNull(), instanceSizeWithTexturesBytes, true, instanceDataType);
                    so.UpdateInstanceData(instanceSizeWithTexturesBytes, incompleteInstanceSizeBytes, numInstances);
                }
            }
        }
    }

    private VertexArrayObjectBase? GetVao(Type? meshDataType, Type? instanceDataType)
        => meshDataType is null || instanceDataType is null ? null : _compositeVertexArrayObjectService.Get(new[] { meshDataType, instanceDataType });
}