using Engine.Data;
using Engine.Data.Transforms;
using Engine.Modules.Entity.Components;
using System.Numerics;

namespace Engine.Modules.Entity.Containers;
internal class Class1 {
}


public sealed class GridContainer2 : EntityContainerBase<Vector2> {

	private readonly DynamicLookup<Entity, Vector2i> _currentEntityGrids;
	private readonly DynamicLookup<Vector2i, Entity> _entitiesByGrid;

	public int GridSize { get; }

	public GridContainer2( EntityManager entityManager, int gridSize = 32 ) : base( entityManager ) {
		this._currentEntityGrids = new();
		this._entitiesByGrid = new();
		this.GridSize = gridSize;
	}

	public override IEnumerable<Entity> GetEntities( Vector2 t )
		=> this._entitiesByGrid[ ((int) MathF.Floor( t.X / this.GridSize ), (int) MathF.Floor( t.Y / this.GridSize )) ];

	protected override void AddEntity( Entity entity ) {
		if ( !entity.TryGetComponent( out CollisionShape2Component? cs ) )
			return;
		using ( UnmanagedList gridList = new() ) {
			GetGrids( cs.Shape, gridList );
			Span<Vector2i> grids = stackalloc Vector2i[(int) gridList.GetElementCount<Vector2i>()];
			if ( grids.Length == 0 && !gridList.TryPopulate( grids, 0 ) ) {
				this.LogWarning( $"Found no grids for {entity}!" );
				return;
			}
			foreach ( Vector2i grid in grids )
				this._entitiesByGrid.Add( grid, entity );
			this._currentEntityGrids.Remove( entity );
			tc.ComponentChanged += OnComponentChanged;
		}
	}

	private void OnComponentChanged( ComponentBase component ) {
		if ( component is not Transform2Component tc )
			return;
		using ( UnmanagedList gridList = new() ) {
			GetGrids( tc.Transform, gridList );
			Span<Vector2i> grids = stackalloc Vector2i[(int) gridList.GetElementCount<Vector2i>()];
			if ( grids.Length == 0 && !gridList.TryPopulate( grids, 0 ) ) {
				this.LogWarning( $"Found no grids for {tc.Entity}!" );
				return;
			}
			foreach ( Vector2i grid in grids )
				this._entitiesByGrid.Add( grid, tc.Entity );
		}
	}

	private void GetGrids( TransformInterface<Vector2, float, Vector2> transform, UnmanagedList gridList ) {
		Vector2i minGrid = GetGrid( transform.GlobalTranslation - transform.GlobalScale * MathFConstants.Sqrt2 );
		Vector2i maxGrid = GetGrid( transform.GlobalTranslation - transform.GlobalScale * MathFConstants.Sqrt2 );

		for ( int x = minGrid.X; x <= maxGrid.X; x++ ) {
			for ( int y = minGrid.Y; y <= maxGrid.Y; y++ ) {
				gridList.Add( new Vector2i( x, y ) );
			}
		}
	}

	private Vector2i GetGrid( Vector2 position ) 
		=> Vector2i.Floor( position );

	protected override void RemoveEntity( Entity entity ) {
		throw new NotImplementedException();
	}

	protected override void AddAll( IEnumerable<Entity> allEntities ) {
		throw new NotImplementedException();
	}
}