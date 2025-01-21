using Engine.Module.Render.Entities;
using Sandbox.Logic.World;
using Sandbox.Logic.World.Tiles;
using System.Collections.Generic;

namespace Sandbox.Render.World;

public sealed class GlobeRenderClusteringBehaviour : DependentRenderBehaviourBase<GlobeArchetype>, IInitializable {
	private readonly List<RenderCluster> _renderClusters = [];

	public IReadOnlyList<RenderCluster> RenderClusters => _renderClusters;

	public void Initialize() {
		GlobeComponent globeComponent = Archetype.GlobeComponent;

		IReadOnlyList<IReadOnlyBranch<Tile, float>> tileClusters = globeComponent.TileTree.GetBranches().Where( p => p.Contents.Count > 0 ).ToList();
		IReadOnlyList<IReadOnlyBranch<Edge, float>> edgeClusters = globeComponent.EdgeTree.GetBranches().Where( p => p.Contents.Count > 0 ).ToList();
		List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> clusters = PairClusters( tileClusters, edgeClusters );
		foreach ((IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>) cluster in clusters)
			_renderClusters.Add( new( cluster.Item1.Contents, cluster.Item2.Contents ) );
	}

	private List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> PairClusters( IReadOnlyList<IReadOnlyBranch<Tile, float>> tileClusters, IReadOnlyList<IReadOnlyBranch<Edge, float>> edgeClusters ) {
		List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> result = [];
		foreach (IReadOnlyBranch<Tile, float> tileCluster in tileClusters)
			foreach (IReadOnlyBranch<Edge, float> edgeCluster in edgeClusters)
				if (tileCluster.BranchBounds == edgeCluster.BranchBounds)
					result.Add( (tileCluster, edgeCluster) );
		return result;
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
