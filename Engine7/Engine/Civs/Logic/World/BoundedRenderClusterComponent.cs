using Civs.World;
using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.World;

public sealed class BoundedRenderClusterComponent : ComponentBase {

	private GlobeModel? _globe;
	private BoundedRenderCluster? _cluster;

	public GlobeModel Globe => _globe ?? throw new InvalidOperationException( "Globe is not set." );
	public BoundedRenderCluster Cluster => _cluster ?? throw new InvalidOperationException( "Cluster is not set." );

	public void Set( GlobeModel globe, int clusterIndex ) {
		if (this._globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this._globe = globe;
		this._cluster = globe.Clusters[clusterIndex];
	}

	public AABB<Vector3<float>> Bounds => Cluster.Bounds;
}