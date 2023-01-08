namespace Engine.ECS;
public sealed class Entity {

	private readonly Dictionary<Type, ComponentBase> _components;
	public delegate void EntityComponentEvent( ComponentBase component );
	public event EntityComponentEvent? ComponentAdded;
	public event EntityComponentEvent? ComponentRemoved;

	public Entity() {
		_components = new();
	}

	internal IEnumerable<ComponentBase> Components => _components.Values;
	public T AddOrGet<T>() where T : ComponentBase, new() => (T) AddOrGet( typeof( T ) );
	public T? RemoveAndGet<T>() where T : ComponentBase, new() => RemoveAndGet( typeof( T ) ) as T;

	private ComponentBase? RemoveAndGet( Type type ) {
		if ( _components.Remove( type, out var value ) ) {
			ComponentRemoved?.Invoke( value );
			return value;
		}
		return null;
	}

	private ComponentBase AddOrGet( Type type ) {
		if ( _components.TryGetValue( type, out ComponentBase? value ) )
			return value;
		return Add( type ) ?? throw new NullReferenceException( "Value should not be null here." );
	}

	private ComponentBase? Add( Type type ) {
		ComponentBase? value = Activator.CreateInstance( type ) as ComponentBase;
		if ( value is null )
			return null;
		value.Owner = this;
		_components.Add( type, value );
		ComponentAdded?.Invoke( value );
		return value;
	}

	public bool HasAllComponents( ComponentTypeCollection componentTypeCollection ) {
		foreach ( var componentType in componentTypeCollection.ComponentTypes )
			if ( !_components.ContainsKey( componentType ) )
				return false;
		return true;
	}
}
