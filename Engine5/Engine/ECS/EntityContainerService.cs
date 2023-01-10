using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.ECS;

public sealed class EntityContainerService : Identifiable, IECSService, IUpdateable {

	private readonly SystemCollectionService _entityManipulatorCollectionService;
	private readonly List<Entity> _entites;
	private readonly Dictionary<ComponentTypeCollection, HashSet<Entity>> _entitesByComponentTypeCollections;
	private readonly Dictionary<Type, HashSet<ComponentBase>> _components;
	private readonly ConcurrentQueue<ComponentBase> _addedComponents;
	private readonly ConcurrentQueue<ComponentBase> _removedComponents;
	private readonly List<SystemBase> _systems; //ComponentListeners? How can we have a spatial separation here like quadtrees and octtrees?

	public EntityContainerService( SystemCollectionService entityManipulatorCollectionService ) {
		this._entityManipulatorCollectionService = entityManipulatorCollectionService ?? throw new ArgumentNullException( nameof( entityManipulatorCollectionService ) );
		_entites = new();
		_entitesByComponentTypeCollections = new();
		_components = new();
		_systems = new();
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

	private void ComponentAddedHandler( ComponentBase component ) => _addedComponents.Enqueue( component );

	private void ComponentRemovedHandler( ComponentBase component ) => _removedComponents.Enqueue( component );

	public void Update( float time, float deltaTime ) {
		while ( _addedComponents.TryDequeue( out ComponentBase? component ) ) {
			Type t = component.GetType();
			if ( !_components.TryGetValue( t, out var set ) )
				_components.Add( t, set = new() );
			set.Add( component );

			var requiringComponentTypeCollections = _entityManipulatorCollectionService.GetRequiringComponentTypeCollections( t );
			if ( requiringComponentTypeCollections is not null ) {
				var ownerEntity = component.Owner;
				if ( ownerEntity is null )
					continue;
				foreach ( var componentTypeCollection in requiringComponentTypeCollections ) {
					if ( ownerEntity.HasAllComponents( componentTypeCollection ) ) {
						if ( !_entitesByComponentTypeCollections.TryGetValue( componentTypeCollection, out var entities ) )
							_entitesByComponentTypeCollections.Add( componentTypeCollection, entities = new() );
						entities.Add( ownerEntity );
					}
				}
			}
		}

		while ( _removedComponents.TryDequeue( out ComponentBase? component ) ) {
			Type t = component.GetType();
			if ( !_components.TryGetValue( t, out var set ) )
				continue;
			set.Remove( component );
			if ( set.Count == 0 )
				_components.Remove( t );

			var requiringComponentTypeCollections = _entityManipulatorCollectionService.GetRequiringComponentTypeCollections( t );
			if ( requiringComponentTypeCollections is not null ) {
				var ownerEntity = component.Owner;
				if ( ownerEntity is null )
					continue;
				foreach ( var componentTypeCollection in requiringComponentTypeCollections ) {
					if ( _entitesByComponentTypeCollections.TryGetValue( componentTypeCollection, out var entities ) ) {
						entities.Remove( ownerEntity );
					}
				}
			}
		}

		//Remove invalid manipulators
		for ( int i = _systems.Count - 1; i >= 0; i-- ) {
			var system = _systems[ i ];
			var requiredComponentTypes = _entityManipulatorCollectionService.GetRequiredComponentTypes( system.GetType() );
			if ( requiredComponentTypes is null )
				throw new NullReferenceException( "Couldn't find the manipulator requested!" );
			for ( int j = 0; j < requiredComponentTypes.ComponentTypes.Count; j++ )
				if ( !_components.ContainsKey( requiredComponentTypes.ComponentTypes[ j ] ) ) {
					_systems.RemoveAt( i );
					break;
				}
		}

		for ( int i = 0; i < _systems.Count; i++ ) {
			var system = _systems[ i ];
			var requiredComponentTypes = _entityManipulatorCollectionService.GetRequiredComponentTypes( system.GetType() );
			if ( requiredComponentTypes is null )
				continue;
			system.Update( _entitesByComponentTypeCollections[ requiredComponentTypes ], time, deltaTime );
		}
	}

	//Update manipulator list
	// (Check if all active manipulators are valid still)
	// (Check is any new components added validates other manipulators)
	//Run Update on manipulators

}
