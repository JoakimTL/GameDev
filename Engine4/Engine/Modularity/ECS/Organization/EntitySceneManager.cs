using System.Collections.Concurrent;
using Engine.Modularity.Domains.Modules;
using Engine.Modularity.ECS.Components;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;

namespace Engine.Modularity.ECS.Organization;
public abstract class EntitySceneManager<R, T, V, SD> : Identifiable where R : RenderComponentBase where T : Component where V : unmanaged where SD : unmanaged {
	private readonly RenderModule _renderModule;
	protected readonly EntityManager _manager;
	protected readonly Scene _scene;
	private readonly BufferedMesh _defaultMesh;
	private readonly ShaderBundle _defaultShader;
	protected readonly uint _batchSize;
	private readonly List<BatchSceneObject<V, SD>> _sceneObjects;
	private readonly Dictionary<Guid, BatchSceneObjectEntityConnection> _entityToSceneObject;
	private readonly ComponentOrganizer<R> _renderComponentOrganizer;
	private readonly ComponentOrganizer<TupleComponent<R, T>> _renderDataTupleComponentOrganizer;
	private readonly ConcurrentQueue<Entity> _componentAddedQueue;
	private readonly ConcurrentQueue<Entity> _componentRemovedQueue;
	private readonly ConcurrentQueue<Entity> _componentChangeQueue;

	public EntitySceneManager( RenderModule renderModule, EntityManager manager, Scene scene, BufferedMesh defaultMesh, ShaderBundle defaultShader, uint batchSize = 512 ) {
		this._renderModule = renderModule;
		this._manager = manager;
		this._scene = scene;
		this._defaultMesh = defaultMesh;
		this._defaultShader = defaultShader;
		this._batchSize = batchSize;
		this._sceneObjects = new();
		this._entityToSceneObject = new();
		this._renderComponentOrganizer = new ComponentOrganizer<R>( manager );
		this._renderDataTupleComponentOrganizer = new ComponentOrganizer<TupleComponent<R, T>>( manager );
		this._componentAddedQueue = new();
		this._componentRemovedQueue = new();
		this._componentChangeQueue = new();
		this._renderComponentOrganizer.OnComponentAdded += ComponentAdded;
		this._renderComponentOrganizer.OnComponentRemoved += ComponentRemoved;
		this._renderDataTupleComponentOrganizer.OnComponentChanged += ComponentChanged;
	}

	private void ComponentAdded( Entity e, R c ) => this._componentAddedQueue.Enqueue( e );
	private void ComponentRemoved( Entity e, R c ) => this._componentRemovedQueue.Enqueue( e );
	private void ComponentChanged( Entity e, TupleComponent<R, T> c ) => this._componentChangeQueue.Enqueue( e );
	public void Update() {
		while ( this._componentAddedQueue.TryDequeue( out Entity? e ) )
			AddSceneObjectComponent( e );
		while ( this._componentChangeQueue.TryDequeue( out Entity? e ) )
			UpdateSceneObjectComponent( e );
		while ( this._componentRemovedQueue.TryDequeue( out Entity? e ) )
			RemoveSceneObjectComponent( e );
	}

	private void AddSceneObjectComponent( Entity e ) {
		e.AddComponent( new TupleComponent<R, T>() );
		R renderComponent = e.GetComponent<R>().NotNull();
		BufferedMesh mesh = _renderModule.BufferedMesh.Get<V>( renderComponent.Template.MeshFilename ) ?? this._defaultMesh;
		ShaderBundle bundle = _renderModule.Shader.Bundles.Get( renderComponent.Template.ShaderGuid ) ?? this._defaultShader;
		GetAvailableSceneObject( mesh, bundle, out BatchSceneObject<V, SD>? so, out uint index );
		this._entityToSceneObject.Add( e.Guid, new BatchSceneObjectEntityConnection( e, so, index ) );
		UpdateSceneObjectComponent( e );
	}


	private void GetAvailableSceneObject( BufferedMesh mesh, ShaderBundle bundle, out BatchSceneObject<V, SD> so, out uint index ) {
		for ( int i = 0; i < this._sceneObjects.Count; i++ ) {
			BatchSceneObject<V, SD> sceneObject = this._sceneObjects[ i ];
			if ( sceneObject.Mesh == mesh && sceneObject.ShaderBundle == bundle && sceneObject.Available ) {
				so = sceneObject;
				index = so.TakeSlot();
				return;
			}
		}
		{
			BatchSceneObject<V, SD> sceneObject = new( mesh, bundle, this._batchSize );
			this._sceneObjects.Add( sceneObject );
			so = sceneObject;
			index = so.TakeSlot();
			this._scene.AddSceneObject( sceneObject );
		}
	}

	private void RemoveSceneObjectComponent( Entity e ) {
		e.RemoveComponent<TupleComponent<R, T>>();
		BatchSceneObjectEntityConnection? connection = this._entityToSceneObject.GetValueOrDefault( e.Guid );
		if ( connection is null )
			return;
		connection.SceneObject.ReleaseSlot( connection.Index );
		this._entityToSceneObject.Remove( e.Guid );
	}

	private void UpdateSceneObjectComponent( Entity e ) {
		BatchSceneObjectEntityConnection? connection = this._entityToSceneObject.GetValueOrDefault( e.Guid );
		if ( connection is null )
			return;
		connection.SceneObject.SetSceneData( connection.Index, GetSceneData( e ) );
	}

	protected abstract SD GetSceneData( Entity e );

	private class BatchSceneObjectEntityConnection {
		public Entity Entity { get; }
		public BatchSceneObject<V, SD> SceneObject { get; }
		public uint Index { get; }

		public BatchSceneObjectEntityConnection( Entity entity, BatchSceneObject<V, SD> sceneObject, uint index ) {
			this.Entity = entity;
			this.SceneObject = sceneObject;
			this.Index = index;
		}
	}

}
