using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;
using System.Runtime.CompilerServices;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Instances here might not have a mesh or bind index, but they do have matching a renderlayer.
/// </summary>
public sealed class SceneLayer : DisposableIdentifiable, IComparable<SceneLayer> {
	public uint RenderLayer { get; }

	private readonly HashSet<SceneInstanceBase> _unboundSceneInstances = [];
	private readonly Dictionary<ulong, SceneObject> _sceneObjectsByBindIndex = [];
	private readonly Structures.SimpleSortedList<IndirectCommandProviderBase> _indirectCommandProviders = new();
	private readonly BufferService _bufferService;
	public IReadOnlyList<IndirectCommandProviderBase> CommandProviders => this._indirectCommandProviders.AsReadOnly();

	public event Action<SceneInstanceBase>? OnInstanceRemoved;
	public event Action? OnChanged;

	public SceneLayer( uint renderLayer, BufferService bufferService ) {
		this.RenderLayer = renderLayer;
		this._bufferService = bufferService;
	}

	internal SceneObjectFixedCollection<TVertexData, TInstanceData> CreateFixedCollection<TVertexData, TInstanceData>( OglVertexArrayObjectBase vao, ShaderBundleBase shaderBundle, IMesh mesh, uint count )
		where TVertexData : unmanaged
		where TInstanceData : unmanaged {
		uint instanceSize = (uint) Unsafe.SizeOf<TInstanceData>();
		if (!this._bufferService.Get( typeof( TInstanceData ) ).TryAllocate( count * instanceSize, out BufferSegment? bufferSegment ))
			throw new InvalidOperationException( "Failed to allocate buffer segment" );
		SceneObjectFixedCollection<TVertexData, TInstanceData> collection = new( this.RenderLayer, vao, shaderBundle, mesh, bufferSegment );
		this._indirectCommandProviders.Add( collection );
		collection.OnChanged += OnProviderChanged;
		collection.OnRemoved += OnProviderRemoval;
		return collection;
	}

	public void AddSceneInstance( SceneInstanceBase sceneInstance ) {
		if (sceneInstance.RenderLayer != this.RenderLayer)
			throw new ArgumentException( "SceneInstance is not compatible with this SceneLayer" );
		if (sceneInstance.BindIndex.HasValue) {
			//This instance already has a bind index! We should add it to the appropriate SceneObject.
			AddSceneInstanceToSceneObject( sceneInstance, sceneInstance.BindIndex.Value );
			return;
		}

		if (!this._unboundSceneInstances.Add( sceneInstance ))
			throw new ArgumentException( "SceneInstance is already in the SceneLayer" );

		sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
		sceneInstance.OnLayerChanged += OnLayerChanged;
		sceneInstance.OnRemoved += OnInstanceRemoval;
	}

	private void OnBindIndexChanged( SceneInstanceBase sceneInstance, ulong? oldBindIndex ) {
		//The only reason we're here is because the scene instance finally got a bind index.
		if (!sceneInstance.BindIndex.HasValue)
			throw new ArgumentException( "SceneInstance does not have a bind index" );
		if (this._unboundSceneInstances.Remove( sceneInstance ))
			AddSceneInstanceToSceneObject( sceneInstance, sceneInstance.BindIndex.Value );
	}

	private void AddSceneInstanceToSceneObject( SceneInstanceBase sceneInstance, ulong bindIndex ) {
		OglVertexArrayObjectBase? vertexArrayObject = sceneInstance.VertexArrayObject ?? throw new ArgumentException( "SceneInstance does not have a VertexArrayObject" );
		ShaderBundleBase? shaderBundle = sceneInstance.ShaderBundle ?? throw new ArgumentException( "SceneInstance does not have a ShaderBundle" );

		if (!this._sceneObjectsByBindIndex.TryGetValue( bindIndex, out SceneObject? sceneObject )) {
			this._sceneObjectsByBindIndex.Add( bindIndex, sceneObject = new( this.RenderLayer, vertexArrayObject, shaderBundle, this._bufferService, 8 ) );
			this._indirectCommandProviders.Add( sceneObject );
			sceneObject.OnChanged += OnProviderChanged;
		}
		sceneObject.OnInstanceRemoved += OnInstanceRemovedFromSceneObject;
		sceneObject.AddSceneInstance( sceneInstance );
		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnRemoved -= OnInstanceRemoval;
	}

	private void OnInstanceRemovedFromSceneObject( SceneInstanceBase sceneInstance ) {
		if (sceneInstance.Removed || sceneInstance.RenderLayer != this.RenderLayer) {
			//Make sure the container is not empty, if it is we should dispose it.
			OnInstanceRemoved?.Invoke( sceneInstance );
			return;
		}
		//The only reason we're here is if the bind index has changed.
		if (!sceneInstance.BindIndex.HasValue) {
			if (!this._unboundSceneInstances.Add( sceneInstance ))
				throw new InvalidOperationException( "Failed to add instance back to unbound collection" );
			sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
			sceneInstance.OnLayerChanged += OnLayerChanged;
			sceneInstance.OnRemoved += OnInstanceRemoval;
			return;
		}
		//Now we know the mesh is not null. We need to add it to the correct collection.
		AddSceneInstanceToSceneObject( sceneInstance, sceneInstance.BindIndex.Value );
	}

	private void RemoveInstance( SceneInstanceBase sceneInstance ) {
		//We're only here because the scene instance is not meshed, but has changed other properties that does not match the scene object.
		this._unboundSceneInstances.Remove( sceneInstance );
		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnRemoved -= OnInstanceRemoval;
		OnInstanceRemoved?.Invoke( sceneInstance );
	}

	private void OnProviderChanged() => OnChanged?.Invoke();

	private void OnProviderRemoval( IRemovable removable ) {
		if (removable is not IndirectCommandProviderBase indirectCommandProvider)
			throw new ArgumentException( "Removable is not an IndirectCommandProviderBase" );
		this._indirectCommandProviders.Remove( indirectCommandProvider );
		OnChanged?.Invoke();
	}
	
	private void OnInstanceRemoval( IRemovable removable ) => RemoveInstance( (SceneInstanceBase) removable );

	//If the layer of a scene instance changes, we know the scene instance is no longer compatible.
	private void OnLayerChanged( SceneInstanceBase changedInstance, uint oldValue ) => RemoveInstance( changedInstance );

	public int CompareTo( SceneLayer? other ) {
		if (other is null)
			return 1;
		return this.RenderLayer.CompareTo( other.RenderLayer );
	}

	protected override bool InternalDispose() {
		foreach (SceneObject sceneObject in this._sceneObjectsByBindIndex.Values)
			sceneObject.Dispose();
		var removableIndirectCommandProviders = this._indirectCommandProviders.AsReadOnly().OfType<IRemovable>().ToArray();
		foreach (IRemovable removable in removableIndirectCommandProviders)
			removable.Remove();
		this._indirectCommandProviders.Clear();
		this._sceneObjectsByBindIndex.Clear();
		this._unboundSceneInstances.Clear();
		return true;
	}
}
