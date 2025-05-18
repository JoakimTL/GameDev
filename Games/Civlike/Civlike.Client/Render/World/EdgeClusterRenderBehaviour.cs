//using Civlike.Logic.World;
//using Civlike.Render.World.Lines;
//using Civlike.World.NewWorld;
//using Engine;
//using Engine.Module.Render.Entities;
//using Engine.Module.Render.Ogl.Scenes;

//namespace Civlike.Render.World;

//public sealed class EdgeClusterRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype>, IInitializable {

//	private SceneObjectFixedCollection<LineVertex, Line3SceneData>? _lineCollection;

//	protected override void OnRenderEntitySet() {
//		base.OnRenderEntitySet();
//	}

//	public void Initialize() {
//		if (Archetype.ClusterComponent.Cluster.Edges.Count == 0)
//			return;

//		VertexMesh<LineVertex> lineInstanceMesh = RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
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

//		_lineCollection = RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( RenderConstants.GridSceneName, 0, lineInstanceMesh, (uint) Archetype.ClusterComponent.Cluster.Edges.Count );

//		int elementsPerSpan = int.Min( (int) _lineCollection.MaxElements, 8192 );
//		Span<Line3SceneData> data = stackalloc Line3SceneData[ elementsPerSpan ];
//		int offset = 0;
//		var edges = Archetype.ClusterComponent.Cluster.Edges;
//		while (offset < _lineCollection.MaxElements) {
//			int i = 0;
//			for (; i < elementsPerSpan; i++) {
//				int index = offset + i;
//				if (index >= edges.Count)
//					break;
//				var edge = edges[ index ];
//				Vector3<float> a = edge.VertexA;
//				Vector3<float> b = edge.VertexB;
//				float length = (b - a).Magnitude<Vector3<float>, float>();
//				float thickness = length * 0.02f;
//				var normal = Edge.GetNormal(a, b);
//				data[ i ] = new Line3SceneData( a, thickness, b, thickness, normal, 0, 1, (-1, 0, 1), 0.5f, 0.75f, 0.5f, (90, 90, 90, 120) );
//			}
//			_lineCollection.WriteRange( (uint) offset, data[ ..i ] );
//			offset += i;
//		}
//	}

//	public override void Update( double time, double deltaTime ) {
//		if (!RenderEntity.TryGetBehaviour( out ClusterVisibilityRenderBehaviour? visibilityBehaviour ) || _lineCollection is null)
//			return;
//		_lineCollection.SetActiveElements( visibilityBehaviour.IsVisible ? _lineCollection.MaxElements : 0 );
//	}

//	protected override bool InternalDispose() {
//		return true;
//	}

//}
