using Engine.Data;
using Engine.Data.Bounds;
using Engine.Modules.Entity.Components;
using System.Numerics;

namespace Engine.Modules.Entity.Containers;

public sealed class GridContainer2 : EntityContainerBase<Vector2> {

	private readonly DynamicLookup<CollisionShape2Component, Vector2i> _currentComponentGrids;
	private readonly DynamicLookup<Vector2i, CollisionShape2Component> _componentsByGrid;

	private readonly UnmanagedList _gridList;

	public int GridSize { get; }

	public GridContainer2( EntityManager entityManager, int gridSize = 32 ) : base( entityManager ) {
		this._currentComponentGrids = new();
		this._componentsByGrid = new();
		_gridList = new();
		this.GridSize = gridSize;
	}

	public override IEnumerable<Entity> GetEntities( Vector2 t )
		=> this._componentsByGrid[ GetGrid( t ) ].Select( p => p.Entity );


	protected override void ComponentAdded( Entity e, ComponentBase component ) {
		if ( component is CollisionShape2Component csc ) {
			csc.ComponentChanged += OnCollisionShapeComponentChanged;
			SetGrids( csc );
		}

		if ( component is Transform2Component tc )
			tc.ComponentChanged += OnTransformComponentChanged;
	}

	protected override void ComponentRemoved( Entity e, ComponentBase component ) {
		if ( component is CollisionShape2Component csc ) {
			csc.ComponentChanged -= OnCollisionShapeComponentChanged;
			RemoveGrids( csc );
		}

		if ( component is Transform2Component tc )
			tc.ComponentChanged -= OnTransformComponentChanged;
	}

	protected override void AddAll( IEnumerable<Entity> allEntities ) {
		foreach ( Entity entity in allEntities ) {
			if ( entity.TryGetComponent( out Transform2Component? tc ) )
				ComponentAdded( entity, tc );
			if ( entity.TryGetComponent( out CollisionShape2Component? csc ) )
				ComponentAdded( entity, csc );
		}
	}

	private void OnTransformComponentChanged( ComponentBase component ) {
		if ( component is not Transform2Component tc )
			return;
		if ( !tc.Entity.TryGetComponent( out CollisionShape2Component? csc ) )
			return;
		SetGrids( csc );
	}

	private void OnCollisionShapeComponentChanged( ComponentBase component ) {
		if ( component is not CollisionShape2Component csc )
			return;

		if ( ( !csc.Shape?.IsValid() ) ?? false ) { //Might be funky
			IReadOnlyCollection<Vector2i> oldGrids = this._currentComponentGrids[ csc ];
			if ( oldGrids.Count > 0 ) {
				foreach ( Vector2i oldGrid in this._currentComponentGrids[ csc ] )
					this._componentsByGrid.Remove( oldGrid, csc );
				this._currentComponentGrids.Remove( csc );
			}
			return;
		}

		SetGrids( csc );
	}

	private void GetGrids( AABB2 aabb, UnmanagedList gridList ) {
		Vector2i minGrid = GetGrid( aabb.Min );
		Vector2i maxGrid = GetGrid( aabb.Max );

		for ( int x = minGrid.X; x <= maxGrid.X; x++ ) {
			for ( int y = minGrid.Y; y <= maxGrid.Y; y++ ) {
				gridList.Add( new Vector2i( x, y ) );
			}
		}
	}

	private Vector2i GetGrid( Vector2 position )
		=> Vector2i.Floor( position / GridSize );

	private void SetGrids( CollisionShape2Component csc ) {
		if ( csc.TryGetAABB( out AABB2 aabb ) ) {
			_gridList.Flush();
			GetGrids( aabb, _gridList );
			Span<Vector2i> grids = stackalloc Vector2i[ (int) _gridList.GetElementCount<Vector2i>() ];
			if ( grids.Length == 0 && !_gridList.TryPopulate( grids, 0 ) ) {
				this.LogWarning( $"Found no grids for {csc.Entity}!" );
				return;
			}
			foreach ( Vector2i oldGrid in this._currentComponentGrids[ csc ] )
				this._componentsByGrid.Remove( oldGrid, csc );
			foreach ( Vector2i grid in grids )
				this._componentsByGrid.Add( grid, csc );
			this._currentComponentGrids.Remove( csc );
			this._currentComponentGrids.AddRange( csc, grids );
		}
	}

	private void RemoveGrids( CollisionShape2Component csc ) {
		foreach ( Vector2i oldGrid in this._currentComponentGrids[ csc ] )
			this._componentsByGrid.Remove( oldGrid, csc );
		this._currentComponentGrids.Remove( csc );
	}

	public override void Dispose() {
		_gridList.Dispose();
	}
}