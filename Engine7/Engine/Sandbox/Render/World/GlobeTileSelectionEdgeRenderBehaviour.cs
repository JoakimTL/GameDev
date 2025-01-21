using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

[Do<IInitializable>.After<GlobeRenderClusteringBehaviour>]
public sealed class GlobeTileSelectionEdgeRenderBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {

	private readonly List<GlobeRenderClusterEdgeRenderer> _edgeClusterRenderers = [];
	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _edgeCollection;

	private Tile? _currentlyDisplayedSelectedTile;
	private Tile? _currentlyDisplayedHoveredTile;

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out GlobeRenderClusteringBehaviour? globeTileClusterBehaviour ))
			throw new InvalidOperationException( "GlobeTileClusterBehaviour not found" );

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
			] );

		_edgeCollection = RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "grid", 1, lineInstanceMesh, 6 );
	}

	public override void Update( double time, double deltaTime ) {
		if (_edgeCollection is null)
			return;

		var selectedTile = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "selectedTile" );
		var hoveredTile = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Tile>( "hoveringTile" );

		if (_currentlyDisplayedSelectedTile == selectedTile && _currentlyDisplayedHoveredTile == hoveredTile)
			return;

		_currentlyDisplayedSelectedTile = selectedTile;
		_currentlyDisplayedHoveredTile = hoveredTile;

		uint activeEdges = 0;
		Span<Line3SceneData> edges = stackalloc Line3SceneData[ 6 ];
		AddSelectionEdges( _currentlyDisplayedSelectedTile, ref activeEdges, edges );
		AddHoveredEdges( _currentlyDisplayedHoveredTile, ref activeEdges, edges );
		_edgeCollection.WriteRange( 0, edges );
		_edgeCollection.SetActiveElements( activeEdges );
	}

	private void AddSelectionEdges( Tile? tile, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (tile is null)
			return;
		foreach (var edge in tile.Edges) {
			var a = edge.VectorA;
			var b = edge.VectorB;
			var length = (a - b).Magnitude<Vector3<float>, float>();
			var thickness = length * 0.025f;
			edges[ (int) activeEdges++ ] = new( edge.VectorA, thickness, edge.VectorB, thickness, edge.Normal, 0, 1, (-1, 0, 1), .25f, 0, 0, 255 );
		}
	}

	private void AddHoveredEdges( Tile? tile, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (tile is null)
			return;
		foreach (var edge in tile.Edges) {
			var a = edge.VectorA;
			var b = edge.VectorB;
			var length = (a - b).Magnitude<Vector3<float>, float>();
			var thickness = length * 0.025f;
			edges[ (int) activeEdges++ ] = new( edge.VectorA, thickness, edge.VectorB, thickness, edge.Normal, 0, 1, (-1, 0, 1), .25f, 0, 0, (120, 120, 120, 50) );
		}
	}

	protected override bool InternalDispose() {
		return true;
	}
}
