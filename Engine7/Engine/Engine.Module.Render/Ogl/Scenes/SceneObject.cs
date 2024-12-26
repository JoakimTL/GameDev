using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Instances here might not have a mesh, but they do have matching a bind index and renderlayer.
/// </summary>
public sealed class SceneObject : DisposableIdentifiable, IComparable<SceneObject> {

	/// <summary>
	/// The number of instances that are expected to be in each collection if there are only one collection. If another collection is added, this number is multiplied by the number of collections + 1.
	/// </summary>
	private readonly uint _baseInstanceCount;
	public OglVertexArrayObjectBase VertexArrayObject { get; }
	public ShaderBundleBase ShaderBundle { get; }
	public ulong BindIndex { get; }
	public uint RenderLayer { get; }
	private readonly HashSet<SceneInstanceBase> _unmeshedSceneInstances;
	private readonly Dictionary<(IMesh, Type), List<SceneObjectSceneInstanceCollection>> _sceneInstanceCollectionsByMeshByInstanceDataType;
	private readonly BufferService _bufferService;

	public event Action<SceneInstanceBase>? OnInstanceRemoved;
	public event Action? OnChanged;

	public SceneObject( uint layer, BufferService bufferService, OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle, uint baseInstanceCount ) {
		this._sceneInstanceCollectionsByMeshByInstanceDataType = [];
		this._unmeshedSceneInstances = [];
		this.VertexArrayObject = vertexArrayObject;
		this.ShaderBundle = shaderBundle;
		this._baseInstanceCount = baseInstanceCount;
		this.BindIndex = this.VertexArrayObject.GetBindIndexWith( this.ShaderBundle ) ?? throw new ArgumentException( "Unable to establish bind index" );
		this.RenderLayer = layer;
		this._bufferService = bufferService;
	}

	public void AddIndirectCommands( List<IndirectCommand> commandList ) {
		foreach (KeyValuePair<(IMesh, Type), List<SceneObjectSceneInstanceCollection>> kvp in this._sceneInstanceCollectionsByMeshByInstanceDataType) {
			if (kvp.Value.Count == 0)
				continue;
			IMesh mesh = kvp.Key.Item1;
			foreach (SceneObjectSceneInstanceCollection collection in kvp.Value)
				if (collection.InstanceCount > 0) 
					commandList.Add( new( mesh.ElementCount, collection.InstanceCount, mesh.ElementOffset, mesh.VertexOffset, collection.BaseInstance ) );
		}
	}

	public int CompareTo( SceneObject? other ) {
		if (other is null)
			return 1;
		return this.BindIndex.CompareTo( other.BindIndex );
	}

	public void AddSceneInstance( SceneInstanceBase sceneInstance ) {
		if (sceneInstance.Removed)
			throw new ArgumentException( "SceneInstance is disposed" );
		if (sceneInstance.BindIndex != this.BindIndex)
			throw new ArgumentException( "SceneInstance is not compatible with this SceneObject" );
		if (sceneInstance.RenderLayer != this.RenderLayer)
			throw new ArgumentException( "SceneInstance is not compatible with this SceneObject" );
		if (sceneInstance.Mesh is not null) {
			AddSceneInstanceIntoCollection( sceneInstance );
			return;
		}

		if (!this._unmeshedSceneInstances.Add( sceneInstance ))
			return;

		sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
		sceneInstance.OnLayerChanged += OnLayerChanged;
		sceneInstance.OnMeshChanged += OnMeshChanged;
		sceneInstance.OnRemoved += OnInstanceRemoval;
	}

	private void AddSceneInstanceIntoCollection( SceneInstanceBase sceneInstance ) {
		IMesh mesh = sceneInstance.Mesh ?? throw new InvalidOperationException( "Instance mesh is null" );

		if (!this._sceneInstanceCollectionsByMeshByInstanceDataType.TryGetValue( (mesh, sceneInstance.InstanceDataType), out List<SceneObjectSceneInstanceCollection>? collectionList ))
			this._sceneInstanceCollectionsByMeshByInstanceDataType.Add( (mesh, sceneInstance.InstanceDataType), collectionList = [] );

		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnMeshChanged -= OnMeshChanged;
		sceneInstance.OnRemoved -= OnInstanceRemoval;

		if (TryAddingToCollection( collectionList, sceneInstance )) {
			sceneInstance.OnMeshOffsetChanged += OnMeshOffsetChanged;
			return;
		}

		uint sizePerInstanceBytes = (uint) (TypeManager.SizeOf( sceneInstance.InstanceDataType ) ?? throw new ArgumentException( "Instance data type must be unmanaged" ));
		if (!this._bufferService.Get( sceneInstance.InstanceDataType ).TryAllocate( sizePerInstanceBytes * this._baseInstanceCount * (uint) (collectionList.Count + 1), out BufferSegment? segment ))
			throw new InvalidOperationException( "Failed to allocate instance data segment" );
		SceneObjectSceneInstanceCollection newCollection = new( segment, sizePerInstanceBytes );
		newCollection.OnChanged += OnSceneCollectionChanged;

		if (!newCollection.TryAdd( sceneInstance )) {
			newCollection.Dispose();
			throw new InvalidOperationException( "Failed to add instance to collection" );
		}
		newCollection.OnInstanceRemoved += OnInstanceRemovedFromCollection;
		collectionList.Add( newCollection );
		sceneInstance.OnMeshOffsetChanged += OnMeshOffsetChanged;
	}

	private void OnMeshOffsetChanged( SceneInstanceBase changedInstance ) {
		OnChanged?.Invoke();
	}

	private bool TryAddingToCollection( List<SceneObjectSceneInstanceCollection> collectionList, SceneInstanceBase instance ) {
		foreach (SceneObjectSceneInstanceCollection collection in collectionList)
			if (collection.TryAdd( instance ))
				return true;
		return false;
	}

	//private void RemoveSceneInstanceFromCollection( SceneInstanceBase<TInstanceData> instance, IMesh oldMesh ) {
	//	if (!_sceneInstanceCollections.TryGetValue( oldMesh, out List<SceneInstanceCollection<TInstanceData>>? collectionList ))
	//		return;

	//	SceneInstanceCollection<TInstanceData>? collection = null;
	//	for (int i = 0; i < collectionList.Count; i++) {
	//		collection = collectionList[ i ];
	//		if (collection.Remove( instance ))
	//			break;
	//	}
	//	if (collection is null)
	//		return;
	//	instance.OnBindIndexChanged += OnBindIndexChanged;
	//	if (collection.InstanceCount == 0) {
	//		_sceneInstanceCollections.Remove( oldMesh );
	//		collection.Dispose();
	//	}
	//}

	private void RemoveInstance( SceneInstanceBase sceneInstance ) {
		//We're only here because the scene instance is not meshed, but has changed other properties that does not match the scene object.
		this._unmeshedSceneInstances.Remove( sceneInstance );
		sceneInstance.OnBindIndexChanged -= OnBindIndexChanged;
		sceneInstance.OnLayerChanged -= OnLayerChanged;
		sceneInstance.OnMeshChanged -= OnMeshChanged;
		sceneInstance.OnRemoved -= OnInstanceRemoval;
		sceneInstance.OnMeshOffsetChanged -= OnMeshOffsetChanged;
		OnInstanceRemoved?.Invoke( sceneInstance );
	}

	private void OnSceneCollectionChanged() => OnChanged?.Invoke();

	private void OnMeshChanged( SceneInstanceBase changedInstance, IMesh? oldMesh ) {
		if (changedInstance.Mesh is null)
			throw new InvalidOperationException( "Instance mesh is null when it shouldn't be. We're listening to events we shouldn't." );
		if (this._unmeshedSceneInstances.Remove( changedInstance ))
			AddSceneInstanceIntoCollection( changedInstance );
	}

	private void OnInstanceRemovedFromCollection( SceneInstanceBase sceneInstance ) {
		if (sceneInstance.Removed || sceneInstance.BindIndex != this.BindIndex || sceneInstance.RenderLayer != this.RenderLayer) {
			//Make sure the container is not empty, if it is we should dispose it.
			//PruneEmptyCollections();
			OnInstanceRemoved?.Invoke( sceneInstance );
			return;
		}
		//The only reason we're here is if the mesh has changed.
		if (sceneInstance.Mesh is null) {
			if (!this._unmeshedSceneInstances.Add( sceneInstance ))
				throw new InvalidOperationException( "Failed to add instance back to unmeshed collection" );
			sceneInstance.OnBindIndexChanged += OnBindIndexChanged;
			sceneInstance.OnLayerChanged += OnLayerChanged;
			sceneInstance.OnMeshChanged += OnMeshChanged;
			sceneInstance.OnRemoved += OnInstanceRemoval;
			sceneInstance.OnMeshOffsetChanged -= OnMeshOffsetChanged;
			return;
		}
		//Now we know the mesh is not null. We need to add it to the correct collection.
		AddSceneInstanceIntoCollection( sceneInstance );
	}

	private void PruneEmptyCollections() {
		foreach (KeyValuePair<(IMesh, Type), List<SceneObjectSceneInstanceCollection>> kvp in this._sceneInstanceCollectionsByMeshByInstanceDataType)
			for (int i = kvp.Value.Count - 1; i >= 0; i--)
				if (kvp.Value[ i ].InstanceCount == 0) {
					kvp.Value[ i ].Dispose();
					kvp.Value.RemoveAt( i );
				}
	}

	private void OnInstanceRemoval( IRemovable removable ) => RemoveInstance( (SceneInstanceBase) removable );
	/// <summary>
	/// Only used if the instance is not meshed. If the layer changes the instance is guaranteed to not be compatible.
	/// </summary>
	private void OnLayerChanged( SceneInstanceBase changedInstance, uint oldValue ) => RemoveInstance( changedInstance );
	/// <summary>
	/// Only used if the instance is not meshed. If the bindindex changes the instance is guaranteed to not be compatible.
	/// </summary>
	private void OnBindIndexChanged( SceneInstanceBase changedInstance, ulong? oldBindIndex ) => RemoveInstance( changedInstance );

	protected override bool InternalDispose() {
		foreach (KeyValuePair<(IMesh, Type), List<SceneObjectSceneInstanceCollection>> kvp in this._sceneInstanceCollectionsByMeshByInstanceDataType)
			foreach (SceneObjectSceneInstanceCollection collection in kvp.Value)
				collection.Dispose();
		return true;
	}
}
