using Civs.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Civs.Render.World.Lines;
using Engine.Standard;
using Civs.World.NewWorld;

namespace Civs.Render.World;

public sealed class WorldSelectedTileEdgeRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _lineCollection;

	private Face? _currentlyDisplayedSelectedFace;

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

		Face? selectedFace = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Face>( "selectedTile" );
		Face? hoveredFace = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Face>( "hoveringTile" );

		if (_currentlyDisplayedSelectedFace == selectedFace)
			return;

		_currentlyDisplayedSelectedFace = selectedFace;

		uint activeEdges = 0;
		Span<Line3SceneData> edges = stackalloc Line3SceneData[ 3 ];
		AddSelectionEdges( selectedFace, ref activeEdges, edges );
		_lineCollection.WriteRange( 0, edges );
		_lineCollection.SetActiveElements( activeEdges );
	}

	private void AddSelectionEdges( Face? face, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (face is null)
			return;
		Span<(Vector3<float>, Vector3<float>)> edgeSpan =
		[
			(face.Blueprint.VertexA, face.Blueprint.VertexB),
			(face.Blueprint.VertexB, face.Blueprint.VertexC),
			(face.Blueprint.VertexC, face.Blueprint.VertexA),
		];
		foreach ((Vector3<float>, Vector3<float>) edge in edgeSpan) {
			Vector3<float> a = edge.Item1;
			Vector3<float> b = edge.Item2;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.025f;
			Vector3<float> normal = Edge.GetNormal( a, b );
			edges[ (int) activeEdges++ ] = new( a, thickness, b, thickness, normal, 0, 1, (-0.79370052598f, 0, 1), .25f, 0, 0, 255 );
		}
	}


	protected override bool InternalDispose() {
		return true;
	}

}
