using Civs.World;
using Engine;
using Engine.Module.Entities.Container;

namespace Civs.Logic.World;

public sealed class BoundedRenderClusterComponent : ComponentBase {

	private GlobeModel? _globe;

	public GlobeModel Globe => _globe ?? throw new InvalidOperationException( "Globe is not set." );
	public int ClusterIndex { get; private set; } = -1;

	public void Set( GlobeModel globe, int clusterIndex ) {
		if (this._globe is not null)
			throw new InvalidOperationException( "Globe is already set." );
		this._globe = globe;
		this.ClusterIndex = clusterIndex;
	}

	public AABB<Vector3<float>> Bounds => Globe.Blueprint.Clusters[ ClusterIndex ].Bounds;
}