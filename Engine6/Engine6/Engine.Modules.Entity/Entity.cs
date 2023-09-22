using Engine.Data;

namespace Engine.Modules.Entity;
public sealed class Entity : Identifiable {

	public delegate void EntityEventHandler( Entity entity );
	public delegate void ParentEventHandler( Entity? oldParent, Entity? newParent );
	public delegate void ComponentEventHandler( Entity entity, ComponentBase component );

	/// <summary>
	/// If OwnerId > 0, this entity is owned by a player. This means that the entity is networked.
	/// </summary>
	public Guid OwnerId { get; }

	public Guid EntityId { get; }

	public Entity? Parent { get; private set; }

	private readonly Dictionary<Type, ComponentBase> _components;
	private readonly HashSet<Entity> _children;

	public IReadOnlyCollection<ComponentBase> Components => _components.Values;
	public IReadOnlyCollection<Entity> Children => _children;

	public event EntityEventHandler? EntityKilled;
	public event ParentEventHandler? BeforeParentChange;
	public event ParentEventHandler? AfterParentChange;
	public event ComponentEventHandler? ComponentAdded;
	public event ComponentEventHandler? ComponentRemoved;

	public bool Alive { get; private set; }

	private Entity( Guid entityId, Guid ownerId ) {
		this.EntityId = entityId;
		this.OwnerId = ownerId;
		this._components = new();
		this._children = new();
		Alive = true;
	}

	internal Entity( Guid ownerId ) : this( ownerId, GuidGenerator.GenerateGuid() ) { }

	internal Entity( EntityData data ) : this( data.EntityId, data.OwnerId ) { }

	public void SetParent( Entity? entity ) {
		if ( entity == this.Parent )
			return;
		if ( IsRecursiveParenting( this, entity ) )
			return;

		BeforeParentChange?.Invoke( this.Parent, entity );
		if ( Parent is not null )
			Parent._children.Remove( this );
		this.Parent = entity;
		if ( Parent is not null )
			Parent._children.Add( this );
		AfterParentChange?.Invoke( this.Parent, entity );
	}

	private bool IsRecursiveParenting( Entity child, Entity? parent ) {
		if ( parent is null )
			return false;
		if ( parent == child ) {
			child.LogWarning( "Cannot parent an entity to itself." );
			return true;
		}
		return IsRecursiveParenting( child, parent.Parent );
	}

	public T? RemoveComponent<T>() where T : ComponentBase {
		Type type = typeof( T );
		if ( !this._components.ContainsKey( type ) )
			return null;
		ComponentBase componentBase = this._components[ type ];
		if ( componentBase is not T component )
			throw new EntityException( this, $"Component assigned to type {type.Name} was instead of type {componentBase.GetType()}." );
		this._components.Remove( type );
		ComponentRemoved?.Invoke( this, component );
		return component;
	}

	public T? GetComponent<T>() where T : ComponentBase => this._components[ typeof( T ) ] as T;

	private ComponentBase AddComponentInternal( ComponentBase component ) {
		Type type = component.GetType();
		if ( this._components.ContainsKey( type ) ) {
			this.LogLine( $"Entity already has a component of type {type.Name}.", Log.Level.NORMAL );
			return this._components[ type ];
		}
		//Might crash! Certainly in need of optimization.
		typeof( ComponentBase ).GetProperty( nameof( ComponentBase.Entity ) )!.SetValue( component, this );
		this._components.Add( type, component );
		ComponentAdded?.Invoke( this, component );
		return component;
	}

	public T AddComponent<T>( T component ) where T : ComponentBase {
		var componentBase = AddComponentInternal( component );
		return componentBase as T ?? throw new EntityException( this, $"Component assigned to type {typeof( T ).Name} was instead of type {componentBase.GetType()}." );
	}

	public T AddComponent<T>() where T : ComponentBase, new() => AddComponent( new T() );

	public SerializableComponentBase? GetOrCreateComponent( Guid guid ) {
		Type? t = ComponentTypeHelper.GetComponentType( guid );
		if ( t is null )
			return null;
		if ( this._components.ContainsKey( t ) )
			return this._components[ t ] as SerializableComponentBase;
		SerializableComponentBase component = Activator.CreateInstance( t ) as SerializableComponentBase ?? throw new EntityException( this, $"Failed to create component of type {t.Name}." );
		AddComponentInternal( component );
		return component;
	}

	public void Kill() {
		Alive = false;
		EntityKilled?.Invoke( this );
	}

	internal void Dispose() {
		foreach ( Entity child in Children )
			child.SetParent( null );
		foreach ( ComponentBase component in _components.Values )
			if ( component is IDisposable disposable )
				disposable.Dispose();
	}
}

public abstract class ComponentBase : Identifiable {

	public Entity Entity { get; } = null!;

	//USing SpinWait to control timing on modules?

}

public class EntityContainer : IUpdateable {

	private readonly Dictionary<Guid, Entity> _entities;
	private readonly Queue<Guid> _removedEntities;

	public EntityContainer() {
		this._entities = new();
		_removedEntities = new();
	}

	public Entity? Get( Guid entityId ) {
		if ( !_entities.ContainsKey( entityId ) )
			return null;
		return _entities[ entityId ];
	}

	public Entity Create( Guid owner ) {
		Entity newEntity = new( owner );
		_entities.Add( newEntity.EntityId, newEntity );
		newEntity.EntityKilled += OnEntityKilled;
		return newEntity;
	}

	internal void CreateEntities( IReadOnlyList<EntityData> data ) {
		foreach ( EntityData entityData in data ) {
			Entity newEntity = new( entityData );
			_entities.Add( entityData.EntityId, newEntity );
			newEntity.EntityKilled += OnEntityKilled;
		}
		foreach ( EntityData entityData in data ) {
			if ( entityData.EntityId == entityData.ParentId )
				continue;
			if ( !_entities.TryGetValue( entityData.ParentId, out Entity? parent ) ) {
				this.LogWarning( $"Failed to find parent entity with id {entityData.ParentId} for {entityData.EntityId}." );
				continue;
			}
			_entities[ entityData.EntityId ].SetParent( parent );
		}
	}

	private void OnEntityKilled( Entity entity ) {
		_removedEntities.Enqueue( entity.EntityId );
	}

	public void Update( in double time, in double deltaTime ) {
		while ( _removedEntities.TryDequeue( out Guid entityId ) )
			if ( _entities.Remove( entityId, out Entity? entity ) ) {
				entity.Dispose();
			} else
				this.LogWarning( $"Failed to remove entity with id {entityId}." );

	}
}