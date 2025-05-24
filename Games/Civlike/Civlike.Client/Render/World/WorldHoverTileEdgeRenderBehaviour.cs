using Civlike.Logic.World;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard;
using Civlike.Client.Render.World.Lines;
using Civlike.World.GameplayState;

namespace Civlike.Client.Render.World;

public sealed class WorldHoverTileEdgeRenderBehaviour : DependentRenderBehaviourBase<WorldArchetype>, IInitializable {

	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _lineCollection;

	private readonly Queue<(Face, int)> _faceQueue = [];
	private readonly Dictionary<Face, int> _addedFaces = [];
	private readonly HashSet<Edge> _edges = [];
	private Face? _currentlyDisplayedHoveredFace;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
	}

	public void Initialize() {

		VertexMesh<LineVertex> lineInstanceMesh = this.RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
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

		this._lineCollection = this.RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 0, lineInstanceMesh, 4096 );
	}

	public override void Update( double time, double deltaTime ) {
		if (this._lineCollection is null)
			return;

		Face? hoveredFace = this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Get<Face>( "hoveringTile" );

		if (this._currentlyDisplayedHoveredFace == hoveredFace)
			return;

		this._currentlyDisplayedHoveredFace = hoveredFace;

		if (this._currentlyDisplayedHoveredFace is null)
			return;

		this._edges.Clear();
		AddFaces( 9, this._currentlyDisplayedHoveredFace );

		foreach (Face face in this._addedFaces.Keys)
			foreach (Edge edge in face.Blueprint.Edges) {
				if (this._edges.Contains( edge ))
					continue;
				this._edges.Add( edge );
			}

		Vector3<float> centerFace = this._currentlyDisplayedHoveredFace.Blueprint.GetCenter();
		Vector3<float> center = this.RenderEntity.ServiceAccess.Get<InternalStateProvider>().Get<Vector3<float>>( "mousePointerGlobeSphereIntersection" );

		float maxRadius = 0;
		foreach (Edge edge in this._edges) {
			float dstSqA = (edge.VectorA - centerFace).MagnitudeSquared();
			float dstSqB = (edge.VectorB - centerFace).MagnitudeSquared();
			float dstSq = MathF.Min( dstSqA, dstSqB ); //Get the lowest of the two, such that lines that poke outside of the "normal" area covered don't create weird cutoffs elsewhere.
			if (dstSq > maxRadius)
				maxRadius = dstSq;
		}


		int activeEdges = 0;
		Span<Line3SceneData> edges = stackalloc Line3SceneData[ 4096 ];
		foreach (Edge edge in this._edges)
			AddEdge( center, maxRadius, edge, edges, ref activeEdges );

		this._lineCollection.WriteRange( 0, edges );
		this._lineCollection.SetActiveElements( (uint) activeEdges );

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

	private static void AddEdge( Vector3<float> center, float maxRadius, Edge edge, Span<Line3SceneData> edges, ref int activeEdges ) {
		Vector3<float> a = edge.DisplayVectorA;
		Vector3<float> b = edge.DisplayVectorB;

		float dstSqA = (a - center).MagnitudeSquared();
		float dstSqB = (b - center).MagnitudeSquared();
		float alphaModifierA = 1 - (dstSqA / maxRadius);
		float alphaModifierB = 1 - (dstSqB / maxRadius);

		float length = (a - b).Magnitude<Vector3<float>, float>();
		float thickness = length * 0.02f;
		Vector3<float> normal = Edge.GetNormal( a, b );
		Vector4<byte> colorA = new Vector4<float>( 90, 90, 90, 120 * alphaModifierA ).Clamp<Vector4<float>, float>( 0, 255 ).CastSaturating<float, byte>();
		Vector4<byte> colorB = new Vector4<float>( 90, 90, 90, 120 * alphaModifierB ).Clamp<Vector4<float>, float>( 0, 255 ).CastSaturating<float, byte>();
		edges[ activeEdges++ ] = new( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), 0.5f, 0.75f, 0.5f, colorA, colorB );
	}

	private void AddFaces( int passes, Face face ) {
		this._addedFaces.Clear();
		this._faceQueue.Enqueue( (face, passes) );

		while (this._faceQueue.TryDequeue( out (Face, int) facePass )) {
			Face currentFace = facePass.Item1;
			int currentPasses = facePass.Item2;
			if (currentPasses <= 0)
				continue;
			int currentPassValue = this._addedFaces.GetValueOrDefault( currentFace );
			if (currentPassValue > currentPasses)
				continue;
			this._addedFaces[ currentFace ] = currentPasses;
			foreach (Face neighbour in currentFace.Blueprint.Neighbours) {
				this._faceQueue.Enqueue( (neighbour, currentPasses - 1) );
			}
		}
	}

	protected override bool InternalDispose() {
		return true;
	}

}
