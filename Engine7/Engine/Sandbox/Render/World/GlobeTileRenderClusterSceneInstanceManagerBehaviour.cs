using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using Sandbox.Logic.World;
using Sandbox.Render.Oldworld;

namespace Sandbox.Render.World;

[Do<IInitializable>.After<GlobeTileClusterBehaviour>]
public sealed class GlobeTileRenderClusterSceneInstanceManagerBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {
	private SceneInstanceCollection<Vertex3, Entity3SceneData> _sceneInstanceCollection = null!;
	private readonly List<TileRenderClusterSceneInstance> _tileRenderClusterSceneInstances = [];

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out GlobeTileClusterBehaviour? globeTileClusterBehaviour ))
			throw new InvalidOperationException( "GlobeTileClusterBehaviour not found" );

		_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<Vertex3, Entity3SceneData, TestShaderBundle>( "gameObjects", 0 );
		foreach (RenderCluster cluster in globeTileClusterBehaviour.RenderClusters) {
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

[Do<IInitializable>.After<GlobeTileClusterBehaviour>]
public sealed class GlobeEdgeRenderClusterSceneInstanceManagerBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {
	private SceneInstanceCollection<LineVertex, Line3SceneData> _sceneInstanceCollection = null!;
	private readonly List<TileRenderClusterSceneInstance> _tileRenderClusterSceneInstances = [];

	public void Initialize() {
		if (!RenderEntity.TryGetBehaviour( out GlobeTileClusterBehaviour? globeTileClusterBehaviour ))
			throw new InvalidOperationException( "GlobeTileClusterBehaviour not found" );

		_sceneInstanceCollection = RenderEntity.RequestSceneInstanceCollection<LineVertex, Line3SceneData, Line3ShaderBundle>( "gameObjects", 0 );
		foreach (RenderCluster cluster in globeTileClusterBehaviour.RenderClusters) {
			var renderClusterSceneInstance = _sceneInstanceCollection.Create<Line3Instance>();
			renderClusterSceneInstance.SetTileRenderCluster( cluster );
			_tileRenderClusterSceneInstances.Add( renderClusterSceneInstance ); //TODO: need some way to control 1000s of instances without having 1000s of instances. Create a particle system in the scene system?
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
