using Civs.World;
using Engine.Module.Entities.Container;

namespace Civs.Logic.World;

public sealed class BoundedRenderClusterComponent : ComponentBase {

	public int ClusterIndex { get; private set; } = -1;
	public GlobeModel? Globe { get; private set; }

	public void Set( GlobeModel globe, int clusterIndex ) {
		if (this.Globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this.Globe = globe;
		this.ClusterIndex = clusterIndex;
	}
}