using Civs.World;
using Engine.Module.Entities.Container;

namespace Civs.Logic.World;

public sealed class TileEdgeClusterComponent : ComponentBase {

	private BoundedTileEdgeCluster? _cluster;

	public BoundedTileEdgeCluster Cluster => _cluster ?? throw new InvalidOperationException( "Cluster is not set." );

	public void SetCluster( BoundedTileEdgeCluster cluster ) {
		if (this._cluster is not null)
			throw new InvalidOperationException( "Cluster is already set." );
		this._cluster = cluster;
	}

}
