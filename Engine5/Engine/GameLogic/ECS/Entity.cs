using Engine.Structure;

namespace Engine.GameLogic.ECS;
public sealed class Entity : DependencyInjectorBase {

	private readonly Dictionary<Type, ComponentBase> _components;
	public event EntityComponentEvent? ComponentAdded;
	public event EntityComponentEvent? ComponentRemoved;

	protected override string UniqueNameTag => string.Join( ", ", _components.Values );

	public Entity() {
		_components = new();
	}

	internal IEnumerable<ComponentBase> Components => _components.Values;
	public T AddOrGet<T>() where T : ComponentBase, new() => (T) AddOrGet( typeof( T ) );
	public T? Get<T>() where T : ComponentBase, new() => Get( typeof( T ) ) as T;
	public T? RemoveAndGet<T>() where T : ComponentBase, new() => RemoveAndGet( typeof( T ) ) as T;
	public void Remove<T>() where T : ComponentBase, new() => _components.Remove( typeof( T ) );

	private ComponentBase? Get( Type t ) => _components.TryGetValue( t, out var c ) ? c : null;

	private ComponentBase? RemoveAndGet( Type type ) {
		if ( _components.Remove( type, out var value ) ) {
			ComponentRemoved?.Invoke( value );
			return value;
		}
		return null;
	}

	protected override object? GetInternal( Type t ) => AddOrGet( t );

	private ComponentBase AddOrGet( Type type ) {
		if ( _components.TryGetValue( type, out ComponentBase? value ) )
			return value;
		return Add( type ) ?? throw new NullReferenceException( "Value should not be null here." );
	}

	private ComponentBase? Add( Type type ) {
		if ( Create( type, false ) is not ComponentBase value )
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
