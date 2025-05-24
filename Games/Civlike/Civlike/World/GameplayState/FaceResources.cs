using Civlike.Logic.Setup;

namespace Civlike.World.GameplayState;

public sealed class FaceResources {

	private readonly ResourceContainer _resources;
	private readonly Dictionary<ResourceTypeBase, double> _renewingRates;

	public FaceResources() {
		this._resources = new();
		this._renewingRates = [];
	}

	internal void Set( IEnumerable<(ResourceTypeBase, double current, double? renewingRate, double? limit)> resources ) {
		foreach ((ResourceTypeBase resource, double current, double? renewingRate, double? limit) in resources) {
			if (limit.HasValue)
				this._resources.SetLimit( resource, limit.Value );
			this._resources.Change( resource, current );
			if (renewingRate.HasValue)
				this._renewingRates[ resource ] = renewingRate.Value;
		}
	}

	public bool DrawResourcesInto( ResourceTypeBase resource, double amount, ResourceContainer container ) {
		if (!this._resources.Change( resource, -amount ))
			return false;
		container.Change( resource, amount );
		return true;
	}

}