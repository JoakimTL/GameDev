﻿using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Instances here might not have a mesh or bind index, but they do have matching a renderlayer.
/// </summary>
public sealed class SceneLayer : DisposableIdentifiable, IComparable<SceneLayer> {
	public uint RenderLayer { get; }

	private readonly HashSet<SceneInstanceBase> _unboundSceneInstances = [];
	private readonly Dictionary<ulong, SceneObject> _sceneObjectsByBindIndex = [];
	private readonly Structures.SimpleSortedList<SceneObject> _sceneObjects = new();
	private readonly BufferService _bufferService;
	public IReadOnlyList<SceneObject> SceneObjects => _sceneObjects.AsReadOnly();

	public event Action<SceneInstanceBase>? OnInstanceRemoved;
	public event Action? OnChanged;

	public SceneLayer( uint renderLayer, BufferService bufferService ) {
		RenderLayer = renderLayer;
		this._bufferService = bufferService;
	}

	public void AddSceneInstance( SceneInstanceBase sceneInstance ) {
		if (sceneInstance.RenderLayer != RenderLayer)
			throw new ArgumentException( "SceneInstance is not compatible with this SceneLayer" );
		if (sceneInstance.BindIndex.HasValue) {
			//This instance already has a bind index! We should add it to the appropriate SceneObject.
			AddSceneInstanceToSceneObject( sceneInstance, sceneInstance.BindIndex.Value );
			return;
		}

		if (!_unboundSceneInstances.Add( sceneInstance ))
			throw new ArgumentException( "SceneInstance is already in the SceneLayer" );

		sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
		sceneInstance.OnLayerChanged += OnLayerChanged;
		sceneInstance.OnDisposed += OnInstanceDisposed;
	}

	private void OnBindIndexChanged( SceneInstanceBase sceneInstance, ulong? oldBindIndex ) {
		//The only reason we're here is because the scene instance finally got a bind index.
		if (!sceneInstance.BindIndex.HasValue)
			throw new ArgumentException( "SceneInstance does not have a bind index" );
		if (_unboundSceneInstances.Remove( sceneInstance ))
			AddSceneInstanceToSceneObject( sceneInstance, sceneInstance.BindIndex.Value );
	}

	private void AddSceneInstanceToSceneObject( SceneInstanceBase sceneInstance, ulong bindIndex ) {
		OglVertexArrayObjectBase? vertexArrayObject = sceneInstance.VertexArrayObject ?? throw new ArgumentException( "SceneInstance does not have a VertexArrayObject" );
		ShaderBundleBase? shaderBundle = sceneInstance.ShaderBundle ?? throw new ArgumentException( "SceneInstance does not have a ShaderBundle" );

		if (!_sceneObjectsByBindIndex.TryGetValue( bindIndex, out SceneObject? sceneObject )) {
			_sceneObjectsByBindIndex.Add( bindIndex, sceneObject = new( RenderLayer, _bufferService, vertexArrayObject, shaderBundle, 2048 ) );
			_sceneObjects.Add( sceneObject );
			sceneObject.OnChanged += OnSceneObjectChanged;
		}
		sceneObject.OnInstanceRemoved += OnInstanceRemovedFromSceneObject;
		sceneObject.AddSceneInstance( sceneInstance );
		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnDisposed -= OnInstanceDisposed;
	}

	private void OnInstanceRemovedFromSceneObject( SceneInstanceBase sceneInstance ) {
		if (sceneInstance.Disposed || sceneInstance.RenderLayer != RenderLayer) {
			//Make sure the container is not empty, if it is we should dispose it.
			OnInstanceRemoved?.Invoke( sceneInstance );
			return;
		}
		//The only reason we're here is if the bind index has changed.
		if (!sceneInstance.BindIndex.HasValue) {
			if (!_unboundSceneInstances.Add( sceneInstance ))
				throw new InvalidOperationException( "Failed to add instance back to unbound collection" );
			sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
			sceneInstance.OnLayerChanged += OnLayerChanged;
			sceneInstance.OnDisposed += OnInstanceDisposed;
			return;
		}
		//Now we know the mesh is not null. We need to add it to the correct collection.
		AddSceneInstanceToSceneObject( sceneInstance, sceneInstance.BindIndex.Value );
	}

	private void RemoveInstance( SceneInstanceBase sceneInstance ) {
		//We're only here because the scene instance is not meshed, but has changed other properties that does not match the scene object.
		_unboundSceneInstances.Remove( sceneInstance );
		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnDisposed -= OnInstanceDisposed;
		OnInstanceRemoved?.Invoke( sceneInstance );
	}

	private void OnSceneObjectChanged() => OnChanged?.Invoke();

	private void OnInstanceDisposed( IListenableDisposable disposable ) => RemoveInstance( (SceneInstanceBase) disposable );

	//If the layer of a scene instance changes, we know the scene instance is no longer compatible.
	private void OnLayerChanged( SceneInstanceBase changedInstance, uint oldValue ) => RemoveInstance( changedInstance );

	public int CompareTo( SceneLayer? other ) {
		if (other is null)
			return 1;
		return RenderLayer.CompareTo( other.RenderLayer );
	}

	protected override bool InternalDispose() {
		foreach (SceneObject sceneObject in _sceneObjectsByBindIndex.Values)
			sceneObject.Dispose();
		_sceneObjects.Clear();
		_sceneObjectsByBindIndex.Clear();
		_unboundSceneInstances.Clear();
		return true;
	}
}