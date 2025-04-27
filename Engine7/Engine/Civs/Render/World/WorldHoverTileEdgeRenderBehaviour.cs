using Civs.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Civs.Render.World.Lines;
using Engine.Standard;
using Civs.World.NewWorld;

namespace Civs.Render.World;

public sealed class WorldHoverTileEdgeRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _lineCollection;

	private readonly HashSet<Face> _faces = [];
	private readonly HashSet<Connection> _connections = [];
	private Face? _currentlyDisplayedHoveredFace;

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

		_lineCollection = RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 1, lineInstanceMesh, 1024 );
	}

	public override void Update( double time, double deltaTime ) {
		if (_lineCollection is null)
			return;

		Face? hoveredFace = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Face>( "hoveringTile" );
		;
		if (_currentlyDisplayedHoveredFace == hoveredFace)
			return;

		_currentlyDisplayedHoveredFace = hoveredFace;

		if (_currentlyDisplayedHoveredFace is null)
			return;

		_connections.Clear();
		_faces.Clear();
		AddFaces( 7, _currentlyDisplayedHoveredFace, _faces );

		foreach (var face in _faces) {
			foreach (var connection in face.Blueprint.Connections) {
				if (_connections.Contains( connection ))
					continue;
				_connections.Add( connection );
			}
		}


		int activeEdges = 0;
		Span<Line3SceneData> edges = stackalloc Line3SceneData[ _connections.Count ];
		foreach (var connection in _connections)
			AddEdge( connection, edges, ref activeEdges );
		_lineCollection.WriteRange( 0, edges );
		_lineCollection.SetActiveElements( (uint) activeEdges );

		/*
		 * 
		int elementsPerSpan = int.Min( (int) _lineCollection.MaxElements, 8192 );
		Span<Line3SceneData> data = stackalloc Line3SceneData[ elementsPerSpan ];
		int offset = 0;
		var edges = Archetype.ClusterComponent.Cluster.Edges;
		while (offset < _lineCollection.MaxElements) {
			int i = 0;
			for (; i < elementsPerSpan; i++) {
				int index = offset + i;
				if (index >= edges.Count)
					break;
				var edge = edges[ index ];
				Vector3<float> a = edge.VertexA;
				Vector3<float> b = edge.VertexB;
				float length = (b - a).Magnitude<Vector3<float>, float>();
				float thickness = length * 0.02f;
				var normal = Edge.GetNormal(a, b);
				data[ i ] = new Line3SceneData( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), 0.5f, 0.75f, 0.5f, (90, 90, 90, 120) );
			}
			_lineCollection.WriteRange( (uint) offset, data[ ..i ] );
			offset += i;
		}
		 */
	}

	private void AddEdge( Connection connection, Span<Line3SceneData> edges, ref int activeEdges ) {
		var edge = connection.SharedEdge;
		Vector3<float> a = edge.VertexA;
		Vector3<float> b = edge.VertexB;
		float length = (a - b).Magnitude<Vector3<float>, float>();
		float thickness = length * 0.02f;
		Vector3<float> normal = Edge.GetNormal( a, b );
		edges[ activeEdges++ ] = new( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), 0.5f, 0.75f, 0.5f, (90, 90, 90, 120) );
	}

	private void AddFaces( int passes, Face face, HashSet<Face> faces ) {
		if (passes <= 0)
			return;
		if (!faces.Add( face ))
			return;
		foreach (var connection in face.Blueprint.Connections) {
			AddFaces( passes - 1, connection.FaceB, faces );
			AddFaces( passes - 1, connection.FaceA, faces );
		}
	}
	
	protected override bool InternalDispose() {
		return true;
	}

}
