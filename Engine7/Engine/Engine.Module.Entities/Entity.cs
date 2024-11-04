using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Entities;

public sealed class Entity : Identifiable {

	//Entity components can be added or removed. There should be events for this.

	private readonly Dictionary<Type, ComponentBase> _components;

	public event ComponentListChangedHandler? ComponentAdded;
	public event ComponentListChangedHandler? ComponentRemoved;
	public event EntityRelationChangedHandler? ParentChanged;
	public event Action<Entity>? OnEntityShouldBeRemoved;
	private readonly ParentIdChanged _parentIdChangedDelegate;

	public Guid EntityId { get; }
	public Guid? ParentId { get; private set; }
	public Entity? Parent { get; private set; }

	internal ComponentBase GetComponent( Type t ) => this._components[ t ];

	internal Entity( Guid entityId, ParentIdChanged parentIdChangedDelegate ) {
		this.EntityId = entityId;
		this._parentIdChangedDelegate = parentIdChangedDelegate;
		this.ParentId = null;
		this.Parent = null;
		this._components = [];
	}

	public void SetParent( Guid? parentId ) {
		this.ParentId = parentId;
		this._parentIdChangedDelegate( this );
	}

	internal void SetParentInternal( Entity? parentEntity ) {
		Entity? oldParent = this.Parent;
		this.Parent = parentEntity;
		ParentChanged?.Invoke( this, oldParent, parentEntity );
	}

	public T AddComponent<T>() where T : ComponentBase, new() {
		T component = new();
		component.SetEntity( this );
		this._components.Add( typeof( T ), component );
		ComponentAdded?.Invoke( component );
		component.ComponentChanged += OnComponentChanged;
		return component;
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
		: throw new ComponentNotFoundException(typeof( T ));

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
		if (this._components.TryGetValue( typeof( T ), out ComponentBase? component )) {
			this._components.Remove( typeof( T ) );
			ComponentRemoved?.Invoke( component );
			component.ComponentChanged -= OnComponentChanged;
		}
	}

	private void OnComponentChanged( ComponentBase component ) {
		if (component is ICleanupController cleanupController)
			if (cleanupController.ShouldBeRemoved) {
				ComponentRequestsRemoval();
				return;
			}
	}

	private void ComponentRequestsRemoval() => OnEntityShouldBeRemoved?.Invoke( this );

}
