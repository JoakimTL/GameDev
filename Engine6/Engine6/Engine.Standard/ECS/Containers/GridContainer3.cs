using Engine.Data.Bounds;
using Engine.Data;
using System.Numerics;
using Engine.Modules.ECS;
using Engine.Standard.ECS.Components;

namespace Engine.Standard.ECS.Containers;

public sealed class GridContainer3 : EntityContainerBase<Vector3> {

	private readonly DynamicLookup<CollisionShape3Component, Vector3i> _currentComponentGrids;
	private readonly DynamicLookup<Vector3i, CollisionShape3Component> _componentsByGrid;

	private readonly UnmanagedList _gridList;

	public int GridSize { get; }

	public GridContainer3( EntityManager entityManager, int gridSize = 32 ) : base( entityManager ) {
		this._currentComponentGrids = new();
		this._componentsByGrid = new();
		_gridList = new();
		this.GridSize = gridSize;
	}

	public override IEnumerable<Entity> GetEntities( Vector3 t )
		=> this._componentsByGrid[ GetGrid( t ) ].Select( p => p.Entity );


	protected override void ComponentAdded( Entity e, ComponentBase component ) {
		if (component is CollisionShape3Component csc) {
			csc.ComponentChanged += OnCollisionShapeComponentChanged;
			SetGrids( csc );
		}

		if (component is Transform3Component tc)
			tc.ComponentChanged += OnTransformComponentChanged;
	}

	protected override void ComponentRemoved( Entity e, ComponentBase component ) {
		if (component is CollisionShape3Component csc) {
			csc.ComponentChanged -= OnCollisionShapeComponentChanged;
			RemoveGrids( csc );
		}

		if (component is Transform3Component tc)
			tc.ComponentChanged -= OnTransformComponentChanged;
	}

	protected override void AddAll( IEnumerable<Entity> allEntities ) {
		foreach (Entity entity in allEntities) {
			if (entity.TryGetComponent( out Transform3Component? tc ))
				ComponentAdded( entity, tc );
			if (entity.TryGetComponent( out CollisionShape3Component? csc ))
				ComponentAdded( entity, csc );
		}
	}

	private void OnTransformComponentChanged( ComponentBase component ) {
		if (component is not Transform3Component tc)
			return;
		if (!tc.Entity.TryGetComponent( out CollisionShape3Component? csc ))
			return;
		SetGrids( csc );
	}

	private void OnCollisionShapeComponentChanged( ComponentBase component ) {
		if (component is not CollisionShape3Component csc)
			return;

		if (!csc.Shape?.IsValid() ?? false) { //Might be funky
			IReadOnlyCollection<Vector3i> oldGrids = this._currentComponentGrids[ csc ];
			if (oldGrids.Count > 0) {
				foreach (Vector3i oldGrid in this._currentComponentGrids[ csc ])
					this._componentsByGrid.Remove( oldGrid, csc );
				this._currentComponentGrids.Remove( csc );
			}
			return;
		}

		SetGrids( csc );
	}

	private void GetGrids( AABB3 aabb, UnmanagedList gridList ) {
		Vector3i minGrid = GetGrid( aabb.Min );
		Vector3i maxGrid = GetGrid( aabb.Max );

		for (int x = minGrid.X; x <= maxGrid.X; x++)
			for (int y = minGrid.Y; y <= maxGrid.Y; y++)
				for (int z = minGrid.Z; z <= maxGrid.Z; z++)
					gridList.Add( new Vector3i( x, y, z ) );
	}

	private Vector3i GetGrid( Vector3 position )
		=> Vector3i.Floor( position / GridSize );

	private void SetGrids( CollisionShape3Component csc ) {
		if (csc.TryGetAABB( out AABB3 aabb )) {
			_gridList.Flush();
			GetGrids( aabb, _gridList );
			Span<Vector3i> grids = stackalloc Vector3i[ (int) _gridList.GetElementCount<Vector3i>() ];
			if (grids.Length == 0 && !_gridList.TryPopulate( grids, 0 )) {
				this.LogWarning( $"Found no grids for {csc.Entity}!" );
				return;
			}
			foreach (Vector3i oldGrid in this._currentComponentGrids[ csc ])
				this._componentsByGrid.Remove( oldGrid, csc );
			foreach (Vector3i grid in grids)
				this._componentsByGrid.Add( grid, csc );
			this._currentComponentGrids.Remove( csc );
			this._currentComponentGrids.AddRange( csc, grids );
		}
	}

	private void RemoveGrids( CollisionShape3Component csc ) {
		foreach (Vector3i oldGrid in this._currentComponentGrids[ csc ])
			this._componentsByGrid.Remove( oldGrid, csc );
		this._currentComponentGrids.Remove( csc );
	}

	public override void Dispose() {
		_gridList.Dispose();
	}
}