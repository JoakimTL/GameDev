using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.GameLogic.ECS;

public sealed class EntityContainerService : Identifiable, IGameLogicService, IUpdateable {

	private readonly List<Entity> _entites;
	private readonly Dictionary<Type, HashSet<ComponentBase>> _components;

	private readonly ConcurrentQueue<ComponentBase> _addedComponents;
	private readonly ConcurrentQueue<ComponentBase> _removedComponents;

	internal event EntityComponentEvent? ComponentAdded;
	internal event EntityComponentEvent? ComponentRemoved;

	protected override string UniqueNameTag => $"{_entites.Count} E / {_components.Count} CT / {_components.Values.Sum( p => p.Count )} C";

	public EntityContainerService() {
		_entites = new();
		_components = new();
		_addedComponents = new();
		_removedComponents = new();
	}

	public void Add( Entity e ) {
		_entites.Add( e );
		e.ComponentAdded += ComponentAddedHandler;
		e.ComponentRemoved += ComponentRemovedHandler;
		foreach ( var compontent in e.Components )
			_addedComponents.Enqueue( compontent );
	}

	public void Remove( Entity e ) {
		_entites.Remove( e );
		e.ComponentAdded -= ComponentAddedHandler;
		e.ComponentRemoved -= ComponentRemovedHandler;
		foreach ( var compontent in e.Components )
			_removedComponents.Enqueue( compontent );
	}

	private void ComponentAddedHandler( ComponentBase component ) => _addedComponents.Enqueue( component );

	private void ComponentRemovedHandler( ComponentBase component ) => _removedComponents.Enqueue( component );

	public void Update( float time, float deltaTime ) {
		while ( _addedComponents.TryDequeue( out ComponentBase? component ) ) {
			Type t = component.GetType();
			if ( !_components.TryGetValue( t, out var set ) )
				_components.Add( t, set = new() );
			set.Add( component );
			ComponentAdded?.Invoke( component );
		}

		while ( _removedComponents.TryDequeue( out ComponentBase? component ) ) {
			Type t = component.GetType();
			if ( !_components.TryGetValue( t, out var set ) )
				continue;
			set.Remove( component );
			if ( set.Count == 0 )
				_components.Remove( t );
			ComponentRemoved?.Invoke( component );
		}
	}

	public IEnumerable<T> GetComponents<T>() where T : ComponentBase
		=> _components.TryGetValue( typeof( T ), out var components ) ? components.OfType<T>() : Enumerable.Empty<T>();

	//Update manipulator list
	// (Check if all active manipulators are valid still)
	// (Check is any new components added validates other manipulators)
	//Run Update on manipulators

}
