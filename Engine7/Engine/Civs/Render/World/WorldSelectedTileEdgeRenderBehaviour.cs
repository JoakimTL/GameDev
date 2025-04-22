using Civs.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Civs.World;
using Engine.Module.Render.Ogl.Scenes;
using Civs.Render.World.Lines;
using Engine.Standard;

namespace Civs.Render.World;

public sealed class WorldSelectedTileEdgeRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _lineCollection;

	private Tile? _currentlyDisplayedSelectedTile;
	private Tile? _currentlyDisplayedHoveredTile;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {

		VertexMesh<LineVertex> lineInstanceMesh = RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
			[
				new LineVertex( (0, 1), (0, 1), 255 ),
				new LineVertex( (1, 1), (1, 1), 255 ),
				new LineVertex( (1, 0), (1, 0), 255 ),
				new LineVertex( (0, 0), (0, 0), 255 ),
				new LineVertex( (-1, 0), (1, 0), 255 ),
				new LineVertex( (-1, 1), (1, 1), 255 )
			], [
				0, 2, 1,
				0, 3, 2,
				0, 5, 4,
				0, 4, 3
			]
		);

		_lineCollection = RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 1, lineInstanceMesh, 3 );
	}

	public override void Update( double time, double deltaTime ) {
		if (_lineCollection is null)
			return;

		Tile? selectedTile = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "selectedTile" );
		Tile? hoveredTile = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "hoveringTile" );

		if (_currentlyDisplayedSelectedTile == selectedTile && _currentlyDisplayedHoveredTile == hoveredTile)
			return;

		_currentlyDisplayedSelectedTile = selectedTile;
		_currentlyDisplayedHoveredTile = hoveredTile;

		uint activeEdges = 0;
		Span<Line3SceneData> edges = stackalloc Line3SceneData[ 3 ];
		AddSelectionEdges( _currentlyDisplayedSelectedTile, ref activeEdges, edges );
		//AddHoveredEdges( _currentlyDisplayedHoveredTile, ref activeEdges, edges );
		_lineCollection.WriteRange( 0, edges );
		_lineCollection.SetActiveElements( activeEdges );
	}

	private void AddSelectionEdges( Tile? tile, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (tile is null)
			return;
		foreach (Edge edge in tile.Edges) {
			Vector3<float> a = edge.VectorA;
			Vector3<float> b = edge.VectorB;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.025f;
			edges[ (int) activeEdges++ ] = new( edge.VectorA, thickness, edge.VectorB, thickness, edge.Normal, 0, 1, (-0.79370052598f, 0, 1), .25f, 0, 0, 255 );
		}
	}

	private void AddHoveredEdges( Tile? tile, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (tile is null)
			return;
		foreach (Edge edge in tile.Edges) {
			Vector3<float> a = edge.VectorA;
			Vector3<float> b = edge.VectorB;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.025f;
			edges[ (int) activeEdges++ ] = new( edge.VectorA, thickness, edge.VectorB, thickness, edge.Normal, 0, 1, (-1, 0, 1), .25f, 0, 0, (120, 120, 120, 50) );
		}
	}


	protected override bool InternalDispose() {
		return true;
	}

}
