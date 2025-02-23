using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Sandbox.Logic.World;

namespace Sandbox.Render.World;

[Do<IInitializable>.After<GlobeRenderClusteringBehaviour>]
public sealed class GlobeRenderClusterEdgeRendererManagerBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {

	private readonly List<GlobeRenderClusterEdgeRenderer> _edgeClusterRenderers = [];

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

		foreach (RenderCluster cluster in globeTileClusterBehaviour.RenderClusters)
			_edgeClusterRenderers.Add( new( cluster, RenderEntity.RequestSceneInstanceFixedCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "grid", 0, lineInstanceMesh, (uint) cluster.Edges.Count ) ) );
	}

	public override void Update( double time, double deltaTime ) {
	}

	protected override bool InternalDispose() {
		return true;
	}
}
