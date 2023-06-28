using Engine.Structure;
using Engine.Structure.Interfaces;

namespace Engine.GameLogic.ECPS;

public class EntitySystemContainerService : DependencyInjectorBase, IGameLogicService, IUpdateable {

	private ulong _currentTick = 0;
	private readonly EntityContainerService _entityContainerService;
	private readonly ComponentTypeCollectionService _componentTypeCollectionService;
	private readonly EntitySpatialGrid3Service _entitySpatialGrid3Service;
	private readonly EntitySpatialGrid2Service _entitySpatialGrid2Service;
	private readonly EntityByComponentContainer _entitiesByComponents;

	private readonly BidirectionalTypeTree _systemSortTree;
	private readonly Dictionary<Type, SystemBase> _presentSystems; //ComponentListeners? How can we have a spatial separation here like quadtrees and octtrees?
	private readonly List<SystemBase> _systemsSorted; //ComponentListeners? How can we have a spatial separation here like quadtrees and octtrees?

	private readonly Dictionary<Type, object> _loadedDependencies;

	protected override string UniqueNameTag => $"Tick#:{_currentTick} / {_systemsSorted.Count} S: [{string.Join( "->", _systemsSorted.Select( p => p.TypeName ) )}]";

	public EntitySystemContainerService(
		EntityContainerService entityContainerService, ComponentTypeCollectionService componentTypeCollectionService,
		EntitySpatialGrid3Service entitySpatialGrid3Service, EntitySpatialGrid2Service entitySpatialGrid2Service ) {
		_entityContainerService = entityContainerService ?? throw new ArgumentNullException( nameof( entityContainerService ) );
		_componentTypeCollectionService = componentTypeCollectionService ?? throw new ArgumentNullException( nameof( componentTypeCollectionService ) );
		_entitySpatialGrid3Service = entitySpatialGrid3Service ?? throw new ArgumentNullException( nameof( entitySpatialGrid3Service ) );
		_entitySpatialGrid2Service = entitySpatialGrid2Service ?? throw new ArgumentNullException( nameof( entitySpatialGrid2Service ) );
		_presentSystems = new();
		_systemsSorted = new();
		_systemSortTree = new( typeof( SystemBase ) );
		_loadedDependencies = new();

		_entitiesByComponents = new( componentTypeCollectionService );

		_entityContainerService._container.ComponentAdded += OnComponentAdded;
		_entityContainerService._container.ComponentRemoved += OnComponentRemoved;
		_entitiesByComponents.ComponentTypeCollectionAdded += OnComponentTypeCollectionAdded;
		_entitiesByComponents.ComponentTypeCollectionRemoved += OnComponentTypeCollectionRemoved;

		OnComponentTypeCollectionAdded( ComponentTypeCollection.Empty );
	}

	private void OnComponentTypeCollectionAdded( ComponentTypeCollection ctc ) {
		foreach ( var systemType in _componentTypeCollectionService.GetSystemsRequiringComponentTypeCollection( ctc ) )
			if ( !_presentSystems.ContainsKey( systemType ) && GetInternal( systemType ) is SystemBase system ) {
				_presentSystems.Add( systemType, system );
				_systemSortTree.Add( systemType );
				_systemSortTree.Update();
			}
		_systemsSorted.Clear();
		foreach ( var type in _systemSortTree.GetNodesSorted() )
			_systemsSorted.Add( _presentSystems[ type ] );
	}

	private void OnComponentTypeCollectionRemoved( ComponentTypeCollection ctc ) {
		foreach ( var systemType in _componentTypeCollectionService.GetSystemsRequiringComponentTypeCollection( ctc ) )
			if ( _presentSystems.TryGetValue( systemType, out var system ) ) {
				_presentSystems.Remove( systemType );
				_systemsSorted.Remove( system );
				_systemSortTree.Remove( systemType );
				system.Dispose();
			}
	}

    private void OnComponentAdded( ComponentBase component ) => _entitiesByComponents.ComponentAdded( component );

    private void OnComponentRemoved( ComponentBase component ) => _entitiesByComponents.ComponentRemoved( component );

    public void Update( float time, float deltaTime ) {
		++_currentTick;

		for ( int i = 0; i < _systemsSorted.Count; i++ ) {
			var system = _systemsSorted[ i ];
			var requiredComponentTypes = _componentTypeCollectionService.GetRequiredComponentTypes( system.GetType() );
			if ( requiredComponentTypes is null )
				continue;
			switch ( system.UpdateMode ) {
				case SystemUpdateMode.None:
					system.Update( Enumerable.Empty<Entity>(), time, deltaTime );
					break;
				case SystemUpdateMode.All:
					system.Update( _entitiesByComponents.GetEntities( requiredComponentTypes ), time, deltaTime );
					break;
				case SystemUpdateMode.Grid2:
					foreach ( var grid in _entitySpatialGrid2Service.GetActiveGrids() )
						system.Update( grid.GetEntities( requiredComponentTypes ), time, deltaTime );
					break;
				case SystemUpdateMode.Grid3:
					foreach ( var grid in _entitySpatialGrid3Service.GetActiveGrids() )
						system.Update( grid.GetEntities( requiredComponentTypes ), time, deltaTime );
					break;
				default:
					this.LogWarning( $"System using unknown {nameof( SystemUpdateMode )}!" );
					break;
			}
		}
	}

	protected override object? GetInternal( Type t ) {
		var altType = GetImplementingType( t );
		object? value;
		if ( _loadedDependencies.TryGetValue( t, out value ) || _loadedDependencies.TryGetValue( altType, out value ) )
			return value;
		value = Create( t, false );
		if ( value is null )
			return null;
		_loadedDependencies.Add( t, value );
		if ( t != altType )
			_loadedDependencies.Add( altType, value );
		return value;
		//In this case it would be nice to be able to define the implementation of an interface rather than use class types. If I want to create a gravity system, but might want a 3d gravity field instead of a flat 9.81m/s^2 I should be able to create just that. That requires implementing an interface, and not just referencing a complete class.

		//Should there be a registration part to this, or should the chosen implementation be based on a set of rules? In which case how can one with those rules?
	}
}
