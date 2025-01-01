using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Scenes;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Render.Entities;

public sealed class RenderEntity : DisposableIdentifiable, IUpdateable, IRemovable {
	private readonly Entity _entity;
	private readonly DisposableList _disposables;
	private readonly RemovableList _removables;
	private readonly Dictionary<Type, RenderBehaviourBase> _behaviours;
	private readonly Queue<IInitializable> _initializationQueue;

	public event Action<RenderBehaviourBase>? OnBehaviourRemoved;
	public event RemovalHandler? OnRemoved;

	public RenderEntityServiceAccess ServiceAccess { get; }

	public bool Removed { get; private set; }

	internal RenderEntity( Entity entity, RenderEntityServiceAccess serviceAccess ) {
		this._entity = entity;
		this.ServiceAccess = serviceAccess;
		this._behaviours = [];
		this._removables = new();
		this._initializationQueue = [];
		this._disposables = new();
	}

	public T RequestSceneInstance<T>( string sceneName, uint layer ) where T : SceneInstanceBase, new() {
		T instance = this.ServiceAccess.SceneInstanceProvider.RequestSceneInstance<T>( sceneName, layer );
		this._removables.Add( instance );
		return instance;
	}

	public SceneInstanceCollection<TVertexData, TInstanceData> RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( string sceneName, uint layer )
		where TVertexData : unmanaged
		where TInstanceData : unmanaged
		where TShaderBundle : ShaderBundleBase {
		SceneInstanceCollection<TVertexData, TInstanceData> collection = this.ServiceAccess.SceneInstanceProvider.RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( sceneName, layer );
		this._removables.Add( collection );
		return collection;
	}

	//public T? RequestShaderBundle<T>() where T : ShaderBundleBase
	//	=> this._serviceAccess.ShaderBundleProvider.GetShaderBundle<T>() as T;

	//public OglVertexArrayObjectBase? RequestCompositeVertexArray<TVertexData, TSceneInstanceData>() where TVertexData : unmanaged where TSceneInstanceData : unmanaged
	//	=> this._serviceAccess.CompositeVertexArrayProvider.GetVertexArray<TVertexData, TSceneInstanceData>();

	//public VertexMesh<TVertex> RequestNewEmptyMesh<TVertex>( uint vertexCount, uint elementCount ) where TVertex : unmanaged
	//	=> this._serviceAccess.MeshProvider.CreateEmptyMesh<TVertex>( vertexCount, elementCount );

	//public VertexMesh<TVertex> RequestNewMesh<TVertex>( Span<TVertex> vertices, Span<uint> elements ) where TVertex : unmanaged
	//	=> this._serviceAccess.MeshProvider.CreateMesh( vertices, elements );

	//TODO: public void ListenToEvents<T>( Action<T> action ) => this._entity.ListenToEvents( action );

	public void SendMessageToEntity( object message ) => this._entity.AddMessage( message );

	public bool AddBehaviour( RenderBehaviourBase renderBehaviour ) {
		if (!this._behaviours.TryAdd( renderBehaviour.GetType(), renderBehaviour ))
			return this.LogWarningThenReturn( $"Behaviour of type {renderBehaviour.GetType().Name} already exists.", false );
		_disposables.Add( renderBehaviour );
		if (renderBehaviour is IInitializable initializable)
			_initializationQueue.Enqueue( initializable );
		renderBehaviour.OnDisposed += OnRenderBehaviourDisposed;
		return true;
	}

	public void RemoveBehaviour( Type behaviourType ) {
		if (!this._behaviours.Remove( behaviourType, out RenderBehaviourBase? renderBehaviour )) {
			this.LogWarning( $"Couldn't find behaviour of type {behaviourType.Name}." );
			return;
		}
		renderBehaviour.OnDisposed -= OnRenderBehaviourDisposed;
		OnBehaviourRemoved?.Invoke( renderBehaviour );
		renderBehaviour.Dispose();
	}

	private void OnRenderBehaviourDisposed( IListenableDisposable disposable ) {
		if (!this._behaviours.Remove( disposable.GetType(), out RenderBehaviourBase? renderBehaviour )) {
			this.LogWarning( $"Couldn't find behaviour of type {disposable.GetType().Name}." );
			return;
		}
		renderBehaviour.OnDisposed -= OnRenderBehaviourDisposed;
		OnBehaviourRemoved?.Invoke( renderBehaviour );
	}

	public bool TryGetBehaviour<T>( [NotNullWhen( true )] out T? renderBehaviour ) where T : RenderBehaviourBase
		=> (renderBehaviour = null) is null && this._behaviours.TryGetValue( typeof( T ), out RenderBehaviourBase? baseBehaviour ) && (renderBehaviour = baseBehaviour as T) is not null;

	public void Update( double time, double deltaTime ) {
		while (_initializationQueue.TryDequeue( out IInitializable? initializable )) {
			if (!_behaviours.ContainsKey( initializable.GetType() ))
				continue;
			initializable.Initialize();
		}
		foreach (RenderBehaviourBase renderBehaviour in this._behaviours.Values)
			renderBehaviour.Update( time, deltaTime );
	}



	protected override bool InternalDispose() {
		foreach (RenderBehaviourBase renderBehaviour in this._behaviours.Values)
			renderBehaviour.Dispose();
		this._behaviours.Clear();
		_disposables.Dispose();
		return true;
	}

	public void Remove() {
		if (this.Removed)
			return;
		this.Removed = true;
		_removables.Clear( true );
		OnRemoved?.Invoke( this );
	}
}
