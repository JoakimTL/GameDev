using Civlike.World.GameplayState;
using Engine;
using Engine.Module.Entities.Container;

namespace Civlike.Logic.World;

public sealed class BoundedRenderClusterComponent : ComponentBase {

	private Globe? _globe;
	private BoundedRenderCluster? _cluster;

	public Globe Globe => this._globe ?? throw new InvalidOperationException( "Globe is not set." );
	public BoundedRenderCluster Cluster => this._cluster ?? throw new InvalidOperationException( "Cluster is not set." );

	public void Set( Globe globe, int clusterIndex ) {
		if (this._globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this._globe = globe;
		this._cluster = globe.Clusters[clusterIndex];
	}

	public AABB<Vector3<float>> Bounds => this.Cluster.Bounds;
}