//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Civs.Logic.Setup;

namespace Civs.World;

public sealed class FaceState {
	private readonly Face _face;
	private uint _terrainTypeId;
	private FaceResources? _resources;

	public FaceState( Face face ) {
		this._face = face;
		_terrainTypeId = 0;
	}

	public TerrainTypeBase TerrainType => TerrainTypeList.GetTerrainType( _terrainTypeId );

	public FaceResources? Resources => _resources;

	public void SetTerrainType( TerrainTypeBase terrainType ) {
		if (terrainType.Id == _terrainTypeId)
			return;
		_terrainTypeId = terrainType.Id;
		_resources = terrainType.HasResources ? new() : null;
		_face.TriggerFaceStateChanged();
	}
}

public sealed class FaceResources {

	private readonly ResourceContainer _resources;
	private readonly Dictionary<ResourceTypeBase, double> _renewingRates;

	public FaceResources() {
		_resources = new();
		_renewingRates = [];
	}

	internal void Set( IEnumerable<(ResourceTypeBase, double current, double? renewingRate, double? limit)> resources ) {
		foreach (var (resource, current, renewingRate, limit) in resources) {
			if (limit.HasValue)
				_resources.SetLimit( resource, limit.Value );
			_resources.Change( resource, current );
			if (renewingRate.HasValue)
				_renewingRates[ resource ] = renewingRate.Value;
		}
	}

	public bool DrawResourcesInto(ResourceTypeBase resource, double amount, ResourceContainer container ) {
		if (!_resources.Change( resource, -amount ))
			return false;
		container.Change( resource, amount );
		return true;
	}

}