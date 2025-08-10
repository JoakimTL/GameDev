using Civlike.World.Render.Shaders;
using Engine;
using Engine.Module.Render.Entities;
using Engine.Standard.Render;

namespace Civlike.World.Render;

public sealed class GlobeRenderBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {

	private readonly List<BoundedTileClusterRenderer> _clusterRenderers;

	public GlobeRenderBehaviour() {
		_clusterRenderers = [];
	}

	public void Initialize() {
		foreach (State.BoundedTileCluster cluster in Archetype.GlobeComponent.Globe.Clusters) {
			TileFaceSceneInstance sceneInstance = RenderEntity.RequestSceneInstance<TileFaceSceneInstance>( "terrain" /*RenderConstants.TerrainSceneName*/, 0 );
			sceneInstance.SetShaderBundle( this.RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<GlobeTerrainShaderBundle>() );
			sceneInstance.SetVertexArrayObject( this.RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity3SceneData>() );
			_clusterRenderers.Add( new BoundedTileClusterRenderer( cluster, sceneInstance ) );
		}
	}

	public override void Update( double time, double deltaTime ) {
		foreach (BoundedTileClusterRenderer renderer in _clusterRenderers)
			renderer.UpdateMesh( RenderEntity.ServiceAccess.MeshProvider );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
