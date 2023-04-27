//using Engine.GlobalServices;
//using Engine.Rendering.Contexts.Objects.Scenes;
//using Engine.Structure.Interfaces;
//using System.Collections.Concurrent;

//namespace Engine.Rendering.Contexts.Services;


//public sealed class RenderableLoaderService : Identifiable, IContextService, IUpdateable
//{
//    private readonly RenderableContainerService _renderableContainerService;
//    private readonly SceneService _sceneService;
//    private readonly ConcurrentQueue<IRenderable> _incomingRenderables;
//    private readonly ConcurrentQueue<IRenderable> _sceneSwitchingRenderables;
//    private readonly ConcurrentQueue<AssetBasedSceneObject> _updatingSceneObjects;
//    private readonly ConcurrentQueue<IRenderable> _outgoingRenderables;
//    private readonly Dictionary<IRenderable, AssetBasedSceneObject> _sceneObjectByRenderable;
//    private readonly Dictionary<AssetBasedSceneObject, Scene> _currentSceneBySceneObject;

//    public RenderableLoaderService(RenderableContainerService renderableContainerService, SceneService sceneService)
//    {
//        _incomingRenderables = new();
//        _sceneSwitchingRenderables = new();
//        _updatingSceneObjects = new();
//        _outgoingRenderables = new();
//        _sceneObjectByRenderable = new();
//        _currentSceneBySceneObject = new();
//        _renderableContainerService = renderableContainerService;
//        _sceneService = sceneService;
//        _renderableContainerService.AddedRenderable += OnAddedRenderable;
//        _renderableContainerService.RemovedRenderable += OnRemovedRenderable;
//        foreach (var r in _renderableContainerService.Renderables)
//            _incomingRenderables.Enqueue(r);
//    }


//    private void OnAddedRenderable(IRenderable obj) => _incomingRenderables.Enqueue(obj);
//    private void OnSceneChanged(IRenderable obj) => _sceneSwitchingRenderables.Enqueue(obj);
//    private void OnSceneObjectRequiringUpdates(AssetBasedSceneObject obj) => _updatingSceneObjects.Enqueue(obj);
//    private void OnRemovedRenderable(IRenderable obj) => _outgoingRenderables.Enqueue(obj);

//    public void Update(float time, float deltaTime)
//    {
//        while (_outgoingRenderables.TryDequeue(out var renderable))
//            if (_sceneObjectByRenderable.TryGetValue(renderable, out var so))
//            {
//                _sceneObjectByRenderable.Remove(renderable);
//                renderable.RenderableSceneChanged -= OnSceneChanged;
//                so.RequireUpdate -= OnSceneObjectRequiringUpdates;
//                if (_currentSceneBySceneObject.Remove(so, out var scene))
//                    scene.Remove(so);
//            }

//        while (_incomingRenderables.TryDequeue(out var renderable))
//            if (!_sceneObjectByRenderable.ContainsKey(renderable))
//            {
//                var so = new AssetBasedSceneObject(renderable);
//                _sceneObjectByRenderable.Add(renderable, new AssetBasedSceneObject(renderable));
//                so.RequireUpdate += OnSceneObjectRequiringUpdates;
//                renderable.RenderableSceneChanged += OnSceneChanged;
//                OnSceneChanged(renderable);
//            }

//        while (_sceneSwitchingRenderables.TryDequeue(out var renderable))
//        {
//            if (!_sceneObjectByRenderable.TryGetValue(renderable, out var so))
//                continue;
//            if (_currentSceneBySceneObject.Remove(so, out var scene))
//                scene.Remove(so);
//            if (renderable.SceneName is null)
//                continue;
//            scene = _sceneService.Get(renderable.SceneName);
//            if (scene is null)
//                continue;
//            scene.Add(so);
//            _currentSceneBySceneObject.Add(so, scene);
//        }

//        while (_updatingSceneObjects.TryDequeue(out var so))
//            so.Update(time, deltaTime);
//    }
//}
