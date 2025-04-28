using Civs.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Civs.Render.World.Lines;
using Engine.Standard;
using Civs.World.NewWorld;

namespace Civs.Render.World;

public sealed class WorldSelectedTileEdgeRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _outerLineCollection;
	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _innerLineCollection;

	private Face? _currentlyDisplayedSelectedFace;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {

		VertexMesh<LineVertex> halfLineInstanceMesh = RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
			[
				new LineVertex( (0, 1), (0, 1), 255 ),
				new LineVertex( (1, 1), (1, 1), 255 ),
				new LineVertex( (1, 0), (1, 0), 255 ),
				new LineVertex( (0, 0), (0, 0), 255 )
				//new LineVertex( (-1, 0), (1, 0), 255 ),
				//new LineVertex( (-1, 1), (1, 1), 255 )
			], [
				0, 2, 1,
				0, 3, 2
				//0, 5, 4,
				//0, 4, 3
			]
		);

		_outerLineCollection = RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 1, halfLineInstanceMesh, 6 );
		_innerLineCollection = RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 2, halfLineInstanceMesh, 6 );
	}

	public override void Update( double time, double deltaTime ) {
		if (_outerLineCollection is null || _innerLineCollection is null)
			return;

		Face? selectedFace = RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Face>( "selectedTile" );

		//if (_currentlyDisplayedSelectedFace == selectedFace)
		//	return;

		_currentlyDisplayedSelectedFace = selectedFace;

		uint activeLines = 0;
		Span<Line3SceneData> lines = stackalloc Line3SceneData[ 6 ];
		AddOuterLines( selectedFace, ref activeLines, lines );
		_outerLineCollection.WriteRange( 0, lines );
		_outerLineCollection.SetActiveElements( activeLines );

		activeLines = 0;
		AddInnerLines( selectedFace, ref activeLines, lines, time );
		_innerLineCollection.WriteRange( 0, lines );
		_innerLineCollection.SetActiveElements( activeLines );
	}

	private void AddInnerLines( Face? face, ref uint activeLines, Span<Line3SceneData> edges, double time ) {
		if (face is null)
			return;
		Span<(Vector3<float>, Vector3<float>)> edgeSpan =
		[
			(face.Blueprint.VertexA, face.Blueprint.VertexB),
			(face.Blueprint.VertexB, face.Blueprint.VertexC),
			(face.Blueprint.VertexC, face.Blueprint.VertexA),
		];
		float timeSine = MathF.Sin( (float) time * MathF.PI * 1.618f ) * 0.5f + 0.5f;
		Span<float> timeSineThickness =
		[
			MathF.Sin( (float) time * MathF.PI ) * 0.5f + 0.5f,
			MathF.Sin( (float) time * MathF.PI + MathF.PI * 2f / 3f ) * 0.5f + 0.5f,
			MathF.Sin( (float) time * MathF.PI + MathF.PI * 4f / 3f ) * 0.5f + 0.5f,
		];
		Vector4<byte> innerColor = new Vector4<float>( 130, 255, 80, 255 )
			.Clamp<Vector4<float>, float>( 0, 255 )
			.CastSaturating<float, byte>();
		int i = 0;
		foreach ((Vector3<float>, Vector3<float>) edge in edgeSpan) {
			Vector3<float> a = edge.Item1;
			Vector3<float> b = edge.Item2;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.015f;
			Vector3<float> normal = Edge.GetNormal( a, b );
			edges[ (int) activeLines++ ] = new( a, thickness * timeSineThickness[ i ], b, thickness * timeSineThickness[ (i + 1) % timeSineThickness.Length ], normal, 0, 1, (-1, 0, 1 * (0.75f + timeSine * 0.25f)), .25f, 0, 0, innerColor );
			i++;
		}
		i = 2;
		foreach ((Vector3<float>, Vector3<float>) edge in edgeSpan) {
			Vector3<float> a = edge.Item2;
			Vector3<float> b = edge.Item1;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.015f;
			Vector3<float> normal = Edge.GetNormal( a, b );
			edges[ (int) activeLines++ ] = new( a, thickness * timeSineThickness[ (i + 1) % timeSineThickness.Length ], b, thickness * timeSineThickness[ i ], normal, 0, 1, (-1, 0, 1 * (0.75f + timeSine * 0.25f)), .25f, 0, 0, innerColor );
			i--;
		}
	}

	private void AddOuterLines( Face? face, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (face is null)
			return;
		Span<(Vector3<float>, Vector3<float>)> edgeSpan =
		[
			(face.Blueprint.VertexA, face.Blueprint.VertexB),
			(face.Blueprint.VertexB, face.Blueprint.VertexC),
			(face.Blueprint.VertexC, face.Blueprint.VertexA),
		];
		Vector4<byte> outerColor = (255, 255, 255, 255);
		foreach ((Vector3<float>, Vector3<float>) edge in edgeSpan) {
			Vector3<float> a = edge.Item1;
			Vector3<float> b = edge.Item2;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.035f;
			Vector3<float> normal = Edge.GetNormal( a, b );
			edges[ (int) activeEdges++ ] = new( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), .25f, 0, 0, outerColor );
		}
		foreach ((Vector3<float>, Vector3<float>) edge in edgeSpan) {
			Vector3<float> a = edge.Item2;
			Vector3<float> b = edge.Item1;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.035f;
			Vector3<float> normal = Edge.GetNormal( a, b );
			edges[ (int) activeEdges++ ] = new( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), .25f, 0, 0, outerColor );
		}
	}

	protected override bool InternalDispose() {
		return true;
	}

}
