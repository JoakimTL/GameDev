using Engine.Structure;
using Engine.Structure.Interfaces;

namespace Engine.GameLogic.ECS;

public class EntitySystemContainerService : Identifiable, IGameLogicService, IUpdateable {

	private ulong _currentTick = 0;
	private readonly EntityContainerService _entityContainerService;
	private readonly ComponentTypeCollectionService _componentTypeCollectionService;
	private readonly EntitySpatialGrid3Service _entitySpatialGrid3Service;
	private readonly EntitySpatialGrid2Service _entitySpatialGrid2Service;
	private readonly EntityByComponentContainer _entitiesByComponents;

	private readonly BidirectionalTypeTree<SystemBase> _systemSortTree;
	private readonly Dictionary<Type, SystemBase> _presentSystems; //ComponentListeners? How can we have a spatial separation here like quadtrees and octtrees?
	private readonly List<SystemBase> _systemsSorted; //ComponentListeners? How can we have a spatial separation here like quadtrees and octtrees?

	protected override string UniqueNameTag => $"Tick#:{_currentTick}";

	public EntitySystemContainerService(
		EntityContainerService entityContainerService, ComponentTypeCollectionService componentTypeCollectionService,
		EntitySpatialGrid3Service entitySpatialGrid3Service, EntitySpatialGrid2Service entitySpatialGrid2Service ) {
		_entityContainerService = entityContainerService ?? throw new ArgumentNullException( nameof( entityContainerService ) );
		_componentTypeCollectionService = componentTypeCollectionService ?? throw new ArgumentNullException( nameof( componentTypeCollectionService ) );
		_entitySpatialGrid3Service = entitySpatialGrid3Service ?? throw new ArgumentNullException( nameof( entitySpatialGrid3Service ) );
		this._entitySpatialGrid2Service = entitySpatialGrid2Service ?? throw new ArgumentNullException( nameof( entitySpatialGrid2Service ) );
		_presentSystems = new();
		_systemsSorted = new();
		_systemSortTree = new();

		_entitiesByComponents = new( componentTypeCollectionService );

		_entityContainerService.ComponentAdded += OnComponentAdded;
		_entityContainerService.ComponentRemoved += OnComponentRemoved;
		_entitiesByComponents.ComponentTypeCollectionAdded += OnComponentTypeCollectionAdded;
		_entitiesByComponents.ComponentTypeCollectionRemoved += OnComponentTypeCollectionRemoved;
	}

	private void OnComponentTypeCollectionAdded( ComponentTypeCollection ctc ) {
		foreach ( var systemType in _componentTypeCollectionService.GetSystemsRequiringComponentTypeCollection( ctc ) )
			if ( !_presentSystems.ContainsKey( systemType ) && Activator.CreateInstance( systemType ) is SystemBase system ) {
				_presentSystems.Add( systemType, system );
				_systemsSorted.Add( system );
				_systemSortTree.Add( systemType );
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
			}
	}

	private void OnComponentAdded( ComponentBase component ) {
		_entitiesByComponents.ComponentAdded( component );
	}

	private void OnComponentRemoved( ComponentBase component ) {
		_entitiesByComponents.ComponentRemoved( component );
	}

	public void Update( float time, float deltaTime ) {
		++_currentTick;

		for ( int i = 0; i < _systemsSorted.Count; i++ ) {
			var system = _systemsSorted[ i ];
			var requiredComponentTypes = _componentTypeCollectionService.GetRequiredComponentTypes( system.GetType() );
			if ( requiredComponentTypes is null )
				continue;
			switch ( system.UpdateMode ) {
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
}
