namespace Engine.Modularity.ECS;

public class ComponentOrganizer<T> : Identifiable where T : Component {

	private readonly EntityManager _manager;
	private readonly HashSet<T> _components;

	public IReadOnlyCollection<T> Components => this._components;

	public event Action<Entity, T>? OnComponentAdded;
	public event Action<Entity, T>? OnComponentRemoved;
	public event Action<Entity, T>? OnComponentChanged;

	public ComponentOrganizer( EntityManager manager ) {
		this._manager = manager;
		this._components = new HashSet<T>();
		foreach ( Entity e in this._manager.Entities )
			if ( e.TryGetComponent( out T? c ) )
				this._components.Add( c );
		this._manager.OnEntityAdded += EntityAdded;
		this._manager.OnEntityRemoved += EntityRemoved;
		this._manager.OnEntityComponentAdded += ComponentAdded;
		this._manager.OnEntityComponentRemoved += ComponentRemoved;
		this._manager.OnEntityComponentChanged += ComponentChanged;
	}

	private void EntityAdded( Entity e ) {
		if ( e.TryGetComponent( out T? c ) )
			if ( this._components.Add( c ) )
				OnComponentAdded?.Invoke( e, c );
	}

	private void EntityRemoved( Entity e ) {
		if ( e.TryGetComponent( out T? c ) )
			if ( this._components.Remove( c ) )
				OnComponentRemoved?.Invoke( e, c );
	}

	private void ComponentAdded( Entity e, Component c ) {
		if ( c is T t )
			if ( this._components.Add( t ) )
				OnComponentAdded?.Invoke( e, t );
	}

	private void ComponentRemoved( Entity e, Component c ) {
		if ( c is T t )
			if ( this._components.Remove( t ) )
				OnComponentRemoved?.Invoke( e, t );
	}
	private void ComponentChanged( Entity e, Component c ) {
		if ( c is T t )
			OnComponentChanged?.Invoke( e, t );
	}
}
