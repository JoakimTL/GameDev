//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Civlike.Logic.Setup;
using Engine;
using System.Numerics;

namespace Civlike.World;

public sealed class FaceState {
	private readonly Face _face;
	private uint _terrainTypeId;
	private FaceResources? _resources;

	/// <summary>
	/// How likely in a given year that a seismic event takes place.
	/// </summary>
	public float SeismicActivity { get; private set; }
	public float Height { get; private set; } = 0.0f;
	public float Moisture { get; private set; }
	public Vector3<float> WindDirection { get; private set; } = new( 0.0f, 0.0f, 0.0f );
	public Vector4<float> Color;

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

	public void SetColor(Vector3<float> color ) {
		Color = new Vector4<float>( color.X, color.Y, color.Z, 1.0f );
		_face.TriggerFaceStateChanged();
	}

	public void SetSeismicActivity( float activity ) {
		if (activity == SeismicActivity)
			return;
		SeismicActivity = activity;
		_face.TriggerFaceStateChanged();
	}

	internal void SetHeight( float height ) {
		if (height == Height)
			return;
		Height = height;
		_face.TriggerFaceStateChanged();
	}

	internal void SetMoisture( float moisture ) {
		if (moisture == Moisture)
			return;
		Moisture = moisture;
		_face.TriggerFaceStateChanged();
	}

	internal void SetWindDirection( Vector3<float> windDirection ) {
		if (windDirection == WindDirection)
			return;
		WindDirection = windDirection;
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