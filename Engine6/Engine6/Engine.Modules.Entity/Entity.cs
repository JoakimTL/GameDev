using Engine.Data;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Modules.Entity;
public sealed class Entity : Identifiable {

	public delegate void EntityEventHandler( Entity entity );
	public delegate void ParentEventHandler( Entity? oldParent, Entity? newParent );
	public delegate void ChildEventHandler( Entity parent, Entity child );
	public delegate void ComponentEventHandler( Entity entity, ComponentBase component );

	/// <summary>
	/// If OwnerId > 0, this entity is owned by a player. This means that the entity is networked.
	/// </summary>
	public Guid OwnerId { get; }

	public Guid EntityId { get; }

	public Entity? Parent { get; private set; }

	private readonly Dictionary<Type, ComponentBase> _components;
	private readonly HashSet<Entity> _children;

	public IReadOnlyCollection<ComponentBase> Components => this._components.Values;
	public IReadOnlyCollection<Entity> Children => this._children;

	public event EntityEventHandler? EntityKilled;
	public event ParentEventHandler? BeforeParentChange;
	public event ParentEventHandler? AfterParentChange;
	public event ChildEventHandler? ChildAdded;
	public event ChildEventHandler? ChildRemoved;
	public event ComponentEventHandler? ComponentAdded;
	public event ComponentEventHandler? ComponentRemoved;

	public bool Alive { get; private set; }

	private Entity( Guid entityId, Guid ownerId ) {
		this.EntityId = entityId;
		this.OwnerId = ownerId;
		this._components = new();
		this._children = new();
		this.Alive = true;
	}

	internal Entity( Guid ownerId ) : this( ownerId, GuidGenerator.GenerateGuid() ) { }

	internal Entity( EntityData data ) : this( data.EntityId, data.OwnerId ) { }

	public void SetParent( Entity? entity ) {
		if ( entity == this.Parent )
			return;
		if ( IsRecursiveParenting( this, entity ) )
			return;

		BeforeParentChange?.Invoke( this.Parent, entity );
		if ( this.Parent is not null )
			this.Parent.RemoveChild( this );
		this.Parent = entity;
		if ( this.Parent is not null )
			this.Parent.AddChild( this );
		AfterParentChange?.Invoke( this.Parent, entity );
	}

	private void AddChild( Entity e ) {
		if ( this._children.Add( e ) )
			ChildAdded?.Invoke( this, e );
	}

	private void RemoveChild( Entity e ) {
		if ( this._children.Remove( e ) )
			ChildRemoved?.Invoke( this, e );
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

	public bool TryGetComponent<T>( [NotNullWhen( true )] out T? component ) where T : ComponentBase {
		component = GetComponent<T>();
		return component is not null;
	}

	public T? GetComponent<T>() where T : ComponentBase => this._components[ typeof( T ) ] as T;

	private ComponentBase AddComponentInternal( ComponentBase component ) {
		Type type = component.GetType();
		if ( this._components.ContainsKey( type ) ) {
			this.LogLine( $"Entity already has a component of type {type.Name}.", Log.Level.NORMAL );
			return this._components[ type ];
		}
		component.SetParent( this );
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
		Type? t = SerializableComponentTypeHelper.GetComponentType( guid );
		if ( t is null )
			return null;
		if ( this._components.ContainsKey( t ) )
			return this._components[ t ] as SerializableComponentBase;
		SerializableComponentBase component = Activator.CreateInstance( t ) as SerializableComponentBase ?? throw new EntityException( this, $"Failed to create component of type {t.Name}." );
		AddComponentInternal( component );
		return component;
	}

	public void Kill() {
		this.Alive = false;
		EntityKilled?.Invoke( this );
	}

	internal void Dispose() {
		foreach ( Entity child in this.Children )
			child.SetParent( null );
		foreach ( ComponentBase component in this._components.Values )
			if ( component is IDisposable disposable )
				disposable.Dispose();
	}
}
