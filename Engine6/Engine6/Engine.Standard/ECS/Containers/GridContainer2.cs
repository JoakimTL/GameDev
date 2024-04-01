using Engine.Data;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Operations;
using Engine.Modules.ECS;
using Engine.Standard.ECS.Components;

namespace Engine.Standard.ECS.Containers;

public sealed class GridContainer2 : EntityContainerBase<Vector2<double>> {

	private readonly DynamicLookup<CollisionShape2Component, Vector2<int>> _currentComponentGrids;
	private readonly DynamicLookup<Vector2<int>, CollisionShape2Component> _componentsByGrid;

	private readonly UnmanagedList _gridList;

	public int GridSize { get; }

	public GridContainer2( EntityManager entityManager, int gridSize = 32 ) : base( entityManager ) {
		this._currentComponentGrids = new();
		this._componentsByGrid = new();
		_gridList = new();
		this.GridSize = gridSize;
	}

	public override IEnumerable<Entity> GetEntities( Vector2<double> t )
		=> this._componentsByGrid[ GetGrid( t ) ].Select( p => p.Entity );


	protected override void ComponentAdded( Entity e, ComponentBase component ) {
		if (component is CollisionShape2Component csc) {
			csc.ComponentChanged += OnCollisionShapeComponentChanged;
			SetGrids( csc );
		}

		if (component is Transform2Component tc)
			tc.ComponentChanged += OnTransformComponentChanged;
	}

	protected override void ComponentRemoved( Entity e, ComponentBase component ) {
		if (component is CollisionShape2Component csc) {
			csc.ComponentChanged -= OnCollisionShapeComponentChanged;
			RemoveGrids( csc );
		}

		if (component is Transform2Component tc)
			tc.ComponentChanged -= OnTransformComponentChanged;
	}

	protected override void AddAll( IEnumerable<Entity> allEntities ) {
		foreach (Entity entity in allEntities) {
			if (entity.TryGetComponent( out Transform2Component? tc ))
				ComponentAdded( entity, tc );
			if (entity.TryGetComponent( out CollisionShape2Component? csc ))
				ComponentAdded( entity, csc );
		}
	}

	private void OnTransformComponentChanged( ComponentBase component ) {
		if (component is not Transform2Component tc)
			return;
		if (!tc.Entity.TryGetComponent( out CollisionShape2Component? csc ))
			return;
		SetGrids( csc );
	}

	private void OnCollisionShapeComponentChanged( ComponentBase component ) {
		if (component is not CollisionShape2Component csc)
			return;

		if (!csc.Shape?.IsValid() ?? false) { //Might be funky
			IReadOnlyCollection<Vector2<int>> oldGrids = this._currentComponentGrids[ csc ];
			if (oldGrids.Count > 0) {
				foreach (Vector2<int> oldGrid in this._currentComponentGrids[ csc ])
					this._componentsByGrid.Remove( oldGrid, csc );
				this._currentComponentGrids.Remove( csc );
			}
			return;
		}

		SetGrids( csc );
	}

	private void GetGrids( in AABB2<double> aabb, UnmanagedList gridList ) {
		Vector2<int> minGrid = GetGrid( aabb.Minima );
		Vector2<int> maxGrid = GetGrid( aabb.Maxima );

		for (int x = minGrid.X; x <= maxGrid.X; x++)
			for (int y = minGrid.Y; y <= maxGrid.Y; y++)
				gridList.Add( new Vector2<int>( x, y ) );
	}

	private Vector2<int> GetGrid( Vector2<double> position )
		=> ( position / GridSize ).Floor().CastSaturating<double, int>();

	private void SetGrids( CollisionShape2Component csc ) {
		if (csc.TryGetAABB( out AABB2<double> aabb )) {
			_gridList.Flush();
			GetGrids( aabb, _gridList );
			Span<Vector2<int>> grids = stackalloc Vector2<int>[ (int) _gridList.GetElementCount<Vector2<int>>() ];
			if (grids.Length == 0 && !_gridList.TryRead( 0, grids )) {
				this.LogWarning( $"Found no grids for {csc.Entity}!" );
				return;
			}
			foreach (Vector2<int> oldGrid in this._currentComponentGrids[ csc ])
				this._componentsByGrid.Remove( oldGrid, csc );
			foreach (Vector2<int> grid in grids)
				this._componentsByGrid.Add( grid, csc );
			this._currentComponentGrids.Remove( csc );
			this._currentComponentGrids.AddRange( csc, grids );
		}
	}

	private void RemoveGrids( CollisionShape2Component csc ) {
		foreach (Vector2<int> oldGrid in this._currentComponentGrids[ csc ])
			this._componentsByGrid.Remove( oldGrid, csc );
		this._currentComponentGrids.Remove( csc );
	}

	public override void Dispose() {
		_gridList.Dispose();
	}
}