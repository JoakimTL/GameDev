namespace Engine.Modules.Entity;

public class EntityManager : IUpdateable {

	public delegate void EntityListChangeHandler( Entity e );
	public delegate void EntityComponentChangeHandler( Entity e, ComponentBase component );

	private readonly Dictionary<Guid, Entity> _entities;
	private readonly Queue<Guid> _removedEntities;

	private readonly Dictionary<Type, EntityContainerBase> _entityContainers;

	public event EntityListChangeHandler? EntityAdded;
	public event EntityListChangeHandler? EntityRemoved;
	public event EntityComponentChangeHandler? ComponentAdded;
	public event EntityComponentChangeHandler? ComponentRemoved;

	public EntityManager() {
		this._entities = new();
		this._removedEntities = new();
		this._entityContainers = new();
	}

	internal IReadOnlyCollection<Entity> AllEntities => this._entities.Values;

	public Entity? Get( Guid entityId )
		=> this._entities.TryGetValue( entityId, out Entity? e ) ? e : null;

	public void AddContainer<T>( EntityContainerBase<T> container ) {
		if ( this._entityContainers.ContainsKey( typeof( T ) ) )
			throw new ArgumentException( $"Container for type {typeof( T )} already exists." );
		this._entityContainers.Add( typeof( T ), container );
		container.AddAll( this.AllEntities );
	}

	public Entity Create( Guid owner ) {
		Entity newEntity = new( owner );
		this._entities.Add( newEntity.EntityId, newEntity );
		newEntity.EntityKilled += OnEntityKilled;
		newEntity.ComponentAdded += OnComponentAdded;
		newEntity.ComponentRemoved += OnComponentRemoved;
		return newEntity;
	}

	internal void CreateEntities( IReadOnlyList<EntityData> data ) {
		foreach ( EntityData entityData in data ) {
			Entity newEntity = new( entityData );
			this._entities.Add( entityData.EntityId, newEntity );
			newEntity.EntityKilled += OnEntityKilled;
			newEntity.ComponentAdded += OnComponentAdded;
			newEntity.ComponentRemoved += OnComponentRemoved;
		}
		foreach ( EntityData entityData in data ) {
			if ( entityData.EntityId == entityData.ParentId )
				continue;
			if ( !this._entities.TryGetValue( entityData.ParentId, out Entity? parent ) ) {
				this.LogWarning( $"Failed to find parent entity with id {entityData.ParentId} for {entityData.EntityId}." );
				continue;
			}
			this._entities[ entityData.EntityId ].SetParent( parent );
		}
	}


	private void OnEntityKilled( Entity entity )
		=> this._removedEntities.Enqueue( entity.EntityId );

	private void OnComponentAdded( Entity entity, ComponentBase component )
		=> ComponentAdded?.Invoke( entity, component );

	private void OnComponentRemoved( Entity entity, ComponentBase component )
		=> ComponentRemoved?.Invoke( entity, component );

	public void Update( in double time, in double deltaTime ) {
		while ( this._removedEntities.TryDequeue( out Guid entityId ) )
			if ( this._entities.Remove( entityId, out Entity? entity ) ) {
				entity.Dispose();
			} else
				this.LogWarning( $"Failed to remove entity with id {entityId}." );

	}
}