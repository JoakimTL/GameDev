using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;
using Sandbox.Render.Oldworld;

namespace Sandbox.Render.World;

[Do<IInitializable>.After<GlobeRenderClusteringBehaviour>]
public sealed class GlobeRenderClusterTileSceneInstanceManagerBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {
	private SceneInstanceCollection<Vertex3, Entity3SceneData> _sceneInstanceCollection = null!;
	private readonly List<GlobeRenderClusterTileSceneInstance> _tileRenderClusterSceneInstances = [];

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out GlobeRenderClusteringBehaviour? globeTileClusterBehaviour ))
			throw new InvalidOperationException( "GlobeTileClusterBehaviour not found" );

		_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity3SceneData, TestShaderBundle>( "gameObjects", 0 );
		foreach (RenderCluster cluster in globeTileClusterBehaviour.RenderClusters) {
			GlobeRenderClusterTileSceneInstance renderClusterSceneInstance = _sceneInstanceCollection.Create<GlobeRenderClusterTileSceneInstance>();
			renderClusterSceneInstance.SetTileRenderCluster( cluster );
			_tileRenderClusterSceneInstances.Add( renderClusterSceneInstance );
		}
	}

	public override void Update( double time, double deltaTime ) {
		foreach (GlobeRenderClusterTileSceneInstance instance in _tileRenderClusterSceneInstances)
			instance.Update( RenderEntity.ServiceAccess.MeshProvider );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
