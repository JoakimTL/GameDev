using Engine.Datatypes.Vectors;
using Engine.GameLogic.ECPS.Components;
using Engine.Utilities;
using System.Buffers;
using System.Numerics;

namespace Engine.GameLogic.ECPS;

public class EntitySpatialGrid2Service : IGameLogicService {

	public const float GridScale = 64;
	public const float InverseGridScale = 1f / GridScale;

	private readonly Dictionary<Entity, AABB2i> _areaOccupiedByEntity;
	private readonly Dictionary<Vector2i, EntityByComponentContainer> _entityContainerGrid;
	private readonly EntityContainerService _entityContainerService;
	private readonly ComponentTypeCollectionService _componentTypeCollectionService;

	public EntitySpatialGrid2Service( EntityContainerService entityContainerService, ComponentTypeCollectionService componentTypeCollectionService ) {
		_entityContainerService = entityContainerService ?? throw new ArgumentNullException( nameof( entityContainerService ) );
		_componentTypeCollectionService = componentTypeCollectionService ?? throw new ArgumentNullException( nameof( componentTypeCollectionService ) );
		_entityContainerGrid = new();
		_areaOccupiedByEntity = new();

		_entityContainerService._container.ComponentAdded += OnComponentAdded;
		_entityContainerService._container.ComponentRemoved += OnComponentRemoved;
	}

	private void OnComponentAdded( ComponentBase component ) {
		if ( component.Owner is null )
			return;

		var type = component.GetType();
		if ( component is Transform2Component t2c ) {
			Vector2 gScale = t2c.Transform.GlobalScale;
			Vector2 gTranslation = t2c.Transform.GlobalTranslation;
			var equalSidedBound = new Vector2( MathF.MaxMagnitude( gScale.X, gScale.Y ) * Constants.Sqrt2 );
			var area = new AABB2i( GetGridCoordinate( gTranslation - equalSidedBound ), GetGridCoordinate( gTranslation + equalSidedBound ) );
			_areaOccupiedByEntity[ component.Owner ] = area;

			AddToGrids( area.GetPointsInAreaInclusive(), component.Owner );
			t2c.ComponentChanged += TransformChanged;
		} else
			if ( component.Owner.Get<Transform3Component>() is not null && _areaOccupiedByEntity.TryGetValue( component.Owner, out var area ) )
			foreach ( var gridCoordinate in area.GetPointsInAreaInclusive() )
				GetGrid( gridCoordinate ).ComponentAdded( component );
	}

	private void OnComponentRemoved( ComponentBase component ) {
		if ( component.Owner is null )
			return;
		if ( component is Transform2Component t2c ) {
			if ( _areaOccupiedByEntity.TryGetValue( component.Owner, out var area ) )
				RemoveFromGrids( area.GetPointsInAreaInclusive(), component.Owner );
			t2c.ComponentChanged -= TransformChanged;
			_areaOccupiedByEntity.Remove( component.Owner );
		} else
			if ( component.Owner.Get<Transform3Component>() is not null && _areaOccupiedByEntity.TryGetValue( component.Owner, out var area ) )
			foreach ( var gridCoordinate in area.GetPointsInAreaInclusive() )
				GetGrid( gridCoordinate ).ComponentRemoved( component );
	}

	private void TransformChanged( ComponentBase component ) {
		if ( component.Owner is null || component is not Transform2Component t2c )
			return;

		var area = _areaOccupiedByEntity[ component.Owner ];

		Vector2 gScale = t2c.Transform.GlobalScale;
		Vector2 gTranslation = t2c.Transform.GlobalTranslation;
		var equalSidedBound = new Vector2( MathF.MaxMagnitude( gScale.X, gScale.Y ) * Constants.Sqrt2 );
		var newArea = new AABB2i( GetGridCoordinate( gTranslation - equalSidedBound ), GetGridCoordinate( gTranslation + equalSidedBound ) );

		if ( area == newArea )
			return;

		var newGrids = newArea.ExceptInclusive( area );
		var oldGrids = area.ExceptInclusive( newArea );

		AddToGrids( newGrids, component.Owner );
		RemoveFromGrids( oldGrids, component.Owner );

		_areaOccupiedByEntity[ component.Owner ] = newArea;
	}

	private void AddToGrids( IEnumerable<Vector2i> gridCoordinates, Entity e ) {
		var grids = gridCoordinates.Select( GetGrid ).ToArray();
		foreach ( var c in e.Components )
			foreach ( var g in grids )
				g.ComponentAdded( c );
	}

	private void RemoveFromGrids( IEnumerable<Vector2i> gridCoordinates, Entity e ) {
		var grids = gridCoordinates.Select( GetGrid ).ToArray();
		foreach ( var c in e.Components )
			foreach ( var g in grids )
				g.ComponentRemoved( c );
	}

	private EntityByComponentContainer GetGrid( Vector2i gridCoordinate ) {
		if ( !_entityContainerGrid.TryGetValue( gridCoordinate, out var grid ) )
			_entityContainerGrid.Add( gridCoordinate, grid = new( _componentTypeCollectionService ) );
		return grid;
	}

	public IEnumerable<EntityByComponentContainer> GetActiveGrids() {
		return _entityContainerGrid.Values.Where( p => p.Any() ); //Can be optimized to not use LINQ
	}

	private Vector2i GetGridCoordinate( Vector2 worldTranslation ) => Vector2i.Floor( worldTranslation * InverseGridScale );

}