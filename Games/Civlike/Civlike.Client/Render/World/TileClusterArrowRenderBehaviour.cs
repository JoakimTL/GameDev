namespace Civlike.Client.Render.World;

//public sealed class TileClusterArrowRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype>, IInitializable {

//	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _lineCollection;

//	protected override void OnRenderEntitySet() {
//		base.OnRenderEntitySet();
//	}

//	public void Initialize() {
//		VertexMesh<LineVertex> lineInstanceMesh = this.RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
//			[
//				new LineVertex( (0, 1), (0, 1), 255 ),
//				new LineVertex( (1, 1), (1, 1), 255 ),
//				new LineVertex( (1, 0), (1, 0), 255 ),
//				new LineVertex( (0, 0), (0, 0), 255 ),
//				new LineVertex( (-1, 0), (1, 0), 255 ),
//				new LineVertex( (-1, 1), (1, 1), 255 )
//			], [
//				0, 2, 1,
//				0, 3, 2,
//				0, 5, 4,
//				0, 4, 3
//			]
//		);

//		this._lineCollection = this.RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 0, lineInstanceMesh, (uint) this.Archetype.ClusterComponent.Cluster.Faces.Count );

//		int elementsPerSpan = int.Min( (int) this._lineCollection.MaxElements, 8192 );
//		Span<Line3SceneData> data = stackalloc Line3SceneData[ elementsPerSpan ];
//		int offset = 0;
//		IReadOnlyList<Face> faces = this.Archetype.ClusterComponent.Cluster.Faces;
//		while (offset < this._lineCollection.MaxElements) {
//			int i = 0;
//			for (; i < elementsPerSpan; i++) {
//				int index = offset + i;
//				if (index >= faces.Count)
//					break;
//				Face face = faces[ index ];

//				float len = (face.Blueprint.VectorA - face.Blueprint.VectorB).Magnitude<Vector3<float>, float>();

//				Vector3<float> faceCenter = face.Blueprint.GetCenter();
//				Vector3<float> windTranslation = faceCenter + face.State.Arrow * len / 0.4f;// * (face.State.WindSpeed / 10);

//				AddFreeEdge( faceCenter, windTranslation, data, i );
//			}
//			this._lineCollection.WriteRange( (uint) offset, data[ ..i ] );
//			offset += i;
//		}
//	}

//	private void AddFreeEdge( Vector3<float> a, Vector3<float> b, Span<Line3SceneData> edges, int index ) {
//		float dstSqA = (a - a).MagnitudeSquared();
//		float dstSqB = (b - a).MagnitudeSquared();

//		float length = (a - b).Magnitude<Vector3<float>, float>();
//		float thickness = length * 0.02f;
//		Vector3<float> normal = Edge.GetNormal( a, b );
//		Vector4<byte> colorA = new Vector4<float>( 0, 0, 0, 255 ).Clamp<Vector4<float>, float>( 0, 255 ).CastSaturating<float, byte>();
//		Vector4<byte> colorB = new Vector4<float>( 255, 0, 0, 255 ).Clamp<Vector4<float>, float>( 0, 255 ).CastSaturating<float, byte>();
//		edges[ index ] = new( a, thickness * 4, b, thickness, normal, 0, 1, (-1, 0, 1), 0.5f, 0.75f, 0.5f, colorA, colorB );
//	}

//	public override void Update( double time, double deltaTime ) {
//		if (!this.RenderEntity.TryGetBehaviour( out ClusterVisibilityRenderBehaviour? visibilityBehaviour ) || this._lineCollection is null)
//			return;
//		this._lineCollection.SetActiveElements( visibilityBehaviour.IsVisible ? this._lineCollection.MaxElements : 0 );
//	}

//	protected override bool InternalDispose() {
//		return true;
//	}

//}
