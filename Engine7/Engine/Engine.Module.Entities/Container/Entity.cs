using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Engine.Module.Entities.Container;

public sealed class Entity : Identifiable {

	//Entity components can be added or removed. There should be events for this.

	private readonly Dictionary<Type, ComponentBase> _components;
	private readonly Dictionary<Type, ArchetypeBase> _archetypes;
	private readonly HashSet<IMessageReadingComponent> _messageReaders;
	private readonly ConcurrentQueue<object> _internalMessageQueue;

	public event ComponentChangeHandler? ComponentChanged;
	public event ComponentListChangedHandler? ComponentAdded;
	public event ComponentListChangedHandler? ComponentRemoved;
	public event EntityRelationChangedHandler? ParentChanged;
	public event Action<Entity>? OnEntityShouldBeRemoved;
	private readonly ParentIdChanged? _parentIdChangedDelegate;

	public event EntityArchetypeChangeHandler? ArchetypeAdded;
	public event EntityArchetypeChangeHandler? ArchetypeRemoved;

	//RenderEntities must hold all serializable components and listen for componentchanged events from the original entity. That way it can serialize the original entity component and deserialize it into it's own and update render behaviours as a result.

	public Guid EntityId { get; }
	public Guid? ParentId { get; private set; }
	public Entity? Parent { get; private set; }

	internal ComponentBase GetComponent( Type t ) => this._components[ t ];

	internal Entity( Guid entityId, ParentIdChanged? parentIdChangedDelegate ) {
		this.EntityId = entityId;
		this._parentIdChangedDelegate = parentIdChangedDelegate;
		this.ParentId = null;
		this.Parent = null;
		this._components = [];
		this._archetypes = [];
		this._messageReaders = [];
		this._internalMessageQueue = [];
	}

	public void SetParent( Guid? parentId ) {
		this.ParentId = parentId;
		this._parentIdChangedDelegate?.Invoke( this );
	}

	internal void SetParentInternal( Entity? parentEntity ) {
		Entity? oldParent = this.Parent;
		this.Parent = parentEntity;
		ParentChanged?.Invoke( this, oldParent, parentEntity );
	}

	internal void AddArchetype( ArchetypeBase archetype ) {
		this._archetypes.Add( archetype.GetType(), archetype );
		ArchetypeAdded?.Invoke( archetype );
	}

	internal void RemoveArchetype( Type archetypeType ) {
		if (!this._archetypes.Remove( archetypeType, out ArchetypeBase? archetype ))
			return;
		archetype.OnArchetypeRemoved();
		ArchetypeRemoved?.Invoke( archetype );
	}

	internal IReadOnlyDictionary<Type, ComponentBase> ComponentsWithKeys => this._components;

	public IReadOnlyCollection<ArchetypeBase> CurrentArchetypes => this._archetypes.Values;

	public T AddComponent<T>( Action<T>? componentInitialization = null ) where T : ComponentBase, new() {
		T component = new();
		component.SetEntity( this );
		componentInitialization?.Invoke( component );
		InternalAddComponent( typeof( T ), component );
		return component;
	}

	internal ComponentBase CreateComponent( Type t ) {
		ComponentBase component = t.CreateInstance( null ) as ComponentBase ?? throw new InvalidOperationException( $"Type {t.Name} is not a component." );
		component.SetEntity( this );
		return component;
	}

	internal ComponentBase AddComponent( Type t, ComponentBase component ) {
		InternalAddComponent( t, component );
		return component;
	}

	private void InternalAddComponent( Type t, ComponentBase component ) {
		this._components.Add( t, component );
		if (component is IMessageReadingComponent messageReader)
			this._messageReaders.Add( messageReader );
		ComponentAdded?.Invoke( this, component );
		CheckAndAddArchetypes( t );
		component.ComponentChanged += OnComponentChanged;
	}

	public bool HasComponent( Type t )
		=> this._components.ContainsKey( t );

	public bool IsArchetype( Type archetypeType ) {
		IReadOnlyCollection<Type> requiredTypes = EntityArchetypeTypeManager.GetRequirementsForArchetype( archetypeType );
		foreach (Type requiredType in requiredTypes)
			if (!HasComponent( requiredType ))
				return false;
		return true;
	}

	public bool HasComponent<T>() where T : ComponentBase, new()
		=> HasComponent( typeof( T ) );

	public bool IsArchetype<T>() where T : ArchetypeBase
		=> IsArchetype( typeof( T ) );

	public bool TryGetComponent<T>( [NotNullWhen( true )] out T? component ) where T : ComponentBase, new()
		=> (component = this._components.TryGetValue( typeof( T ), out ComponentBase? baseComponent ) && baseComponent is T t ? t : null) is not null;

	public T? GetComponentOrDefault<T>() where T : ComponentBase, new() => this._components.GetValueOrDefault( typeof( T ) ) as T;

	/// <summary>
	/// Tries to get the requested component, but if it's not available, returns false.<br />
	/// Throws an <see cref="InvalidOperationException"/> if the component type is not a component or does not have a parameterless constructor.
	/// </summary>
	/// <param name="componentType">The type of the component, must derive from <see cref="ComponentBase"/> and have a parameterless constructor.</param>
	/// <exception cref="InvalidOperationException"></exception>
	public bool TryGetComponent( Type componentType, [NotNullWhen( true )] out ComponentBase? component ) {
		if (!componentType.IsAssignableTo( typeof( ComponentBase ) ))
			throw new InvalidOperationException( $"Type {componentType.Name} is not a component." );
		if (!TypeManager.ResolveType( componentType ).HasParameterlessConstructor)
			throw new InvalidOperationException( $"Component {componentType.Name} must have a parameterless constructor." );
		return (component = this._components.TryGetValue( componentType, out ComponentBase? baseComponent ) ? baseComponent : null) is not null;
	}

	/// <summary>
	/// Gets the requested component, but if it's not available a <see cref="ComponentNotFoundException"/> is thrown.<br />
	/// Use this method if you expect the component to be available, otherwise use <see cref="TryGetComponent{T}(out T)"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <exception cref="ComponentNotFoundException"></exception>
	public T GetComponentOrThrow<T>() where T : ComponentBase, new() => this._components.TryGetValue( typeof( T ), out ComponentBase? component )
		? (T) component
		: throw new ComponentNotFoundException( typeof( T ) );

	/// <summary>
	/// Gets the requested component, but if it's not available a <see cref="ComponentNotFoundException"/> is thrown.<br />
	/// Use this method if you expect the component to be available, otherwise use <see cref="TryGetComponent(Type, out ComponentBase?)"/>.<br />
	/// Throws an <see cref="InvalidOperationException"/> if the component type is not a component or does not have a parameterless constructor.
	/// </summary>
	/// <param name="componentType">The type of the component, must derive from <see cref="ComponentBase"/> and have a parameterless constructor.</param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="ComponentNotFoundException"></exception>
	public ComponentBase GetComponentOrThrow( Type componentType ) {
		if (!componentType.IsAssignableTo( typeof( ComponentBase ) ))
			throw new InvalidOperationException( $"Type {componentType.Name} is not a component." );
		if (!TypeManager.ResolveType( componentType ).HasParameterlessConstructor)
			throw new InvalidOperationException( $"Component {componentType.Name} must have a parameterless constructor." );
		if (!this._components.TryGetValue( componentType, out ComponentBase? component ))
			throw new ComponentNotFoundException( componentType );
		return component;
	}

	public void RemoveComponent<T>() where T : ComponentBase, new() {
		if (this._components.Remove( typeof( T ), out ComponentBase? component )) {
			if (component is IMessageReadingComponent messageReader)
				this._messageReaders.Remove( messageReader );
			ComponentRemoved?.Invoke( this, component );
			RemoveArchetypes( typeof( T ) );
			component.ComponentChanged -= OnComponentChanged;
		}
	}

	internal void RemoveComponent( Type t ) {
		if (this._components.Remove( t, out ComponentBase? component )) {
			if (component is IMessageReadingComponent messageReader)
				this._messageReaders.Remove( messageReader );
			ComponentRemoved?.Invoke( this, component );
			RemoveArchetypes( t );
			component.ComponentChanged -= OnComponentChanged;
		}
	}

	private void CheckAndAddArchetypes( Type componentType ) {
		IReadOnlyCollection<Type> potentialArchetypes = EntityArchetypeTypeManager.GetArchetypesRequiringComponent( componentType );
		foreach (Type archetypeType in potentialArchetypes) {
			bool isArchetype = IsArchetype( archetypeType );
			if (!isArchetype)
				continue;
			AddArchetype( this.CreateArchetypeInstance( archetypeType ) );
		}
	}

	private void RemoveArchetypes( Type componentType ) {
		IReadOnlyCollection<Type> affectedArchetypes = EntityArchetypeTypeManager.GetArchetypesRequiringComponent( componentType );
		foreach (Type archetypeType in affectedArchetypes)
			RemoveArchetype( archetypeType );
	}

	private void OnComponentChanged( ComponentBase component ) {
		if (component is ICleanupController cleanupController)
			if (cleanupController.ShouldBeRemoved) {
				ComponentRequestsRemoval();
				return;
			}
		ComponentChanged?.Invoke( component );
	}

	public void AddMessage( object message ) => this._internalMessageQueue.Enqueue( message );

	internal void ReadInternalMessages() {
		while (this._internalMessageQueue.TryDequeue( out object? message )) {
			if (message is SynchronizedEntity synchronizedEntity)
				synchronizedEntity.Synchronize();
			foreach (IMessageReadingComponent messageReader in this._messageReaders)
				messageReader.ReadMessage( message );
		}
	}

	internal void ReadExternalMessage( object message ) {
		foreach (IMessageReadingComponent messageReader in this._messageReaders)
			messageReader.ReadMessage( message );
	}

	private void ComponentRequestsRemoval() => OnEntityShouldBeRemoved?.Invoke( this );
}
