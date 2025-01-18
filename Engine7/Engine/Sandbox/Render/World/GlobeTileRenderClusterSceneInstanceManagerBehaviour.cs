using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.OldWorld;
using Sandbox.Logic.World;

namespace Sandbox.Render.World;

[Do<IInitializable>.After<GlobeTileClusterBehaviour>]
public sealed class GlobeTileRenderClusterSceneInstanceManagerBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {
	private SceneInstanceCollection<Vertex3, Entity3SceneData> _sceneInstanceCollection = null!;
	private readonly List<TileRenderClusterSceneInstance> _tileRenderClusterSceneInstances = [];

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out GlobeTileClusterBehaviour? globeTileClusterBehaviour ))
			throw new InvalidOperationException( "GlobeTileClusterBehaviour not found" );

		_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity3SceneData, TestShaderBundle>( "gameObjects", 0 );
		foreach (TileRenderCluster cluster in globeTileClusterBehaviour.RenderClusters) {
			var renderClusterSceneInstance = _sceneInstanceCollection.Create<TileRenderClusterSceneInstance>();
			renderClusterSceneInstance.SetTileRenderCluster( cluster );
			_tileRenderClusterSceneInstances.Add( renderClusterSceneInstance );
		}
	}

	public override void Update( double time, double deltaTime ) {
		foreach (TileRenderClusterSceneInstance instance in _tileRenderClusterSceneInstances)
			instance.Update( RenderEntity.ServiceAccess.MeshProvider );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
