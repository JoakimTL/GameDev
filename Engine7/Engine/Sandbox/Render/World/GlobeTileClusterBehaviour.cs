using Engine.Module.Render.Entities;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.World;

public sealed class GlobeTileClusterBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {
	private readonly List<RenderCluster> _renderClusters = [];

	public IReadOnlyList<RenderCluster> RenderClusters => _renderClusters;

	public void Initialize() {
		GlobeComponent globeComponent = Archetype.GlobeComponent;

		IEnumerable<IReadOnlyCollection<Tile>> clusters = globeComponent.TileTree.GetContentsAtLevel( 0 ).Where( p => p.Count > 0 );
		foreach (IReadOnlyCollection<Tile> cluster in clusters)
			_renderClusters.Add( new( cluster ) );
	}

	public override void Update( double time, double deltaTime ) {
		Vector3<float> normalizedTranslation = RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation.Normalize<Vector3<float>, float>();
		foreach (RenderCluster cluster in _renderClusters)
			cluster.CheckVisibilityAgainstCameraTranslation( normalizedTranslation );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
