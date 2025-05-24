using Civlike.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard;
using Civlike.Client.Render.World.Lines;
using Civlike.World.GameplayState;

namespace Civlike.Client.Render.World;

public sealed class WorldSelectedTileEdgeRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _outerLineCollection;
	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _innerLineCollection;

	private Face? _currentlyDisplayedSelectedFace;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {

		VertexMesh<LineVertex> halfLineInstanceMesh = this.RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
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

		this._outerLineCollection = this.RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 1, halfLineInstanceMesh, 6 );
		this._innerLineCollection = this.RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 2, halfLineInstanceMesh, 6 );
	}

	public override void Update( double time, double deltaTime ) {
		if (this._outerLineCollection is null || this._innerLineCollection is null)
			return;

		Face? selectedFace = this.RenderEntity.ServiceAccess.Get<GameStateProvider>().Get<Face>( "selectedTile" );

		//if (_currentlyDisplayedSelectedFace == selectedFace)
		//	return;

		this._currentlyDisplayedSelectedFace = selectedFace;

		uint activeLines = 0;
		Span<Line3SceneData> lines = stackalloc Line3SceneData[ 6 ];
		AddOuterLines( selectedFace, ref activeLines, lines );
		this._outerLineCollection.WriteRange( 0, lines );
		this._outerLineCollection.SetActiveElements( activeLines );

		activeLines = 0;
		AddInnerLines( selectedFace, ref activeLines, lines, time );
		this._innerLineCollection.WriteRange( 0, lines );
		this._innerLineCollection.SetActiveElements( activeLines );
	}

	private static void AddInnerLines( Face? face, ref uint activeLines, Span<Line3SceneData> edges, double time ) {
		if (face is null)
			return;
		Span<(Vector3<float>, Vector3<float>)> edgeSpan =
		[
			(face.Blueprint.VectorA, face.Blueprint.VectorB),
			(face.Blueprint.VectorB, face.Blueprint.VectorC),
			(face.Blueprint.VectorC, face.Blueprint.VectorA),
		];
		float timeSine = (MathF.Sin( (float) time * MathF.PI * 1.618f ) * 0.5f) + 0.5f;
		Span<float> timeSineThickness =
		[
			(MathF.Sin( (float) time * MathF.PI ) * 0.5f) + 0.5f,
			(MathF.Sin( ((float) time * MathF.PI) + (MathF.PI * 2f / 3f) ) * 0.5f) + 0.5f,
			(MathF.Sin( ((float) time * MathF.PI) + (MathF.PI * 4f / 3f) ) * 0.5f) + 0.5f,
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
			edges[ (int) activeLines++ ] = new( a, thickness * timeSineThickness[ i ], b, thickness * timeSineThickness[ (i + 1) % timeSineThickness.Length ], normal, 0, 1, (-1, 0, 1 * (0.75f + (timeSine * 0.25f))), .25f, 0, 0, innerColor );
			i++;
		}
		i = 2;
		foreach ((Vector3<float>, Vector3<float>) edge in edgeSpan) {
			Vector3<float> a = edge.Item2;
			Vector3<float> b = edge.Item1;
			float length = (a - b).Magnitude<Vector3<float>, float>();
			float thickness = length * 0.015f;
			Vector3<float> normal = Edge.GetNormal( a, b );
			edges[ (int) activeLines++ ] = new( a, thickness * timeSineThickness[ (i + 1) % timeSineThickness.Length ], b, thickness * timeSineThickness[ i ], normal, 0, 1, (-1, 0, 1 * (0.75f + (timeSine * 0.25f))), .25f, 0, 0, innerColor );
			i--;
		}
	}

	private static void AddOuterLines( Face? face, ref uint activeEdges, Span<Line3SceneData> edges ) {
		if (face is null)
			return;
		Span<(Vector3<float>, Vector3<float>)> edgeSpan =
		[
			(face.Blueprint.VectorA, face.Blueprint.VectorB),
			(face.Blueprint.VectorB, face.Blueprint.VectorC),
			(face.Blueprint.VectorC, face.Blueprint.VectorA),
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
