//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World.NewWorld;

public sealed class FaceState {
	private readonly Face _face;
	private uint _terrainTypeId;

	public FaceState( Face face, uint terrainType ) {
		this._face = face;
		_terrainTypeId = terrainType;
	}

	public TerrainTypeBase TerrainType => TerrainTypeList.GetTerrainType(_terrainTypeId);

	public void SetTerrainType( TerrainTypeBase terrainType ) {
		if (terrainType.Id == _terrainTypeId)
			return;
		_terrainTypeId = terrainType.Id;
		_face.TriggerFaceStateChanged();
	}
}