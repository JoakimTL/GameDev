//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Civlike.Logic.Setup;
using Engine;

namespace Civlike.WorldOld;

public sealed class FaceState( Face face ) {
	private readonly Face _face = face;
	private uint _terrainTypeId = 0;
	private FaceResources? _resources;

	/// <summary>
	/// How likely in a given year that a seismic event takes place.
	/// </summary>
	public float SeismicActivity { get; private set; }
	public float Height { get; private set; } = 0.0f;
	public Vector3<float> Gradient { get; private set; }
	public Temperature Temperature { get; private set; }
	public Pressure StaticPressure { get; private set; } = Pressure.FromAtmosphere( 1 ); // 1 atm
	public Pressure BaseDynamicPressure { get; private set; } = Pressure.FromAtmosphere( 1 ); // 1 atm
	public Pressure DynamicPressure { get; private set; } = Pressure.FromAtmosphere( 1 ); // 1 atm
	public Vector3<float> WindDirection { get; private set; }
	public float WindSpeed { get; private set; }
	public float LinearDistanceFromOcean { get; private set; } = float.PositiveInfinity;
	public float UpwindDistanceFromOcean { get; private set; } = float.PositiveInfinity;
	public float LocalRelief { get; private set; }
	public float LocalPressureRelief { get; private set; }
	public float Ruggedness { get; private set; }
	public float EvaporationMm { get; private set; }
	public float AbsoluteHumidityMm { get; private set; }
	public float PrecipitationMm { get; private set; }
	public Vector3<float> RiverFlow { get; private set; } = new( 0, 0, 0 ); // TODO: Implement river flow
	public Vector4<float> Color;

	public float PressureHeight => float.Max( 0.0f, this.Height );
	public float RelativeHumidity => this.AbsoluteHumidityMm / GetMoistureCapacityMm();

	public TerrainTypeBase TerrainType => TerrainTypeList.GetTerrainType( this._terrainTypeId );

	public FaceResources? Resources => this._resources;

	public float GetMoistureCapacityMm() {
		const float g = 9.80665f; // m/s^2

		float celsius = this.Temperature.Celsius;
		float vaporPressure_hPa = 6.112f * MathF.Exp( 17.62f * celsius / (celsius + 243.12f) );
		float vaporPressure_Pa = vaporPressure_hPa * 100;

		float atmosphereSaturationMixingRatio = 0.622f * vaporPressure_Pa / (this.StaticPressure - vaporPressure_Pa);

		float massDryAirPerM2 = this.StaticPressure / g;

		return massDryAirPerM2 * atmosphereSaturationMixingRatio;
	}

	public void SetTerrainType( TerrainTypeBase terrainType ) {
		if (terrainType.Id == this._terrainTypeId)
			return;
		this._terrainTypeId = terrainType.Id;
		this._resources = terrainType.HasResources ? new() : null;
		this._face.TriggerFaceStateChanged();
	}

	public void SetColor( Vector3<float> color ) {
		this.Color = new Vector4<float>( color.X, color.Y, color.Z, 1.0f );
		this._face.TriggerFaceStateChanged();
	}

	public void SetSeismicActivity( float activity ) {
		if (activity == this.SeismicActivity)
			return;
		this.SeismicActivity = activity;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetHeight( float height ) {
		if (height == this.Height)
			return;
		this.Height = height;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetGradient( Vector3<float> slope ) {
		if (slope == this.Gradient)
			return;
		this.Gradient = slope;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetTemperature( Temperature temperature ) {
		if (temperature == this.Temperature)
			return;
		this.Temperature = temperature;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetStaticPressure( Pressure pressure ) {
		if (pressure == this.StaticPressure)
			return;
		this.StaticPressure = pressure;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetBaseDynamicPressure( Pressure pressure ) {
		if (pressure == this.BaseDynamicPressure)
			return;
		this.BaseDynamicPressure = pressure;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetDynamicPressure( Pressure pressure ) {
		if (pressure == this.DynamicPressure)
			return;
		this.DynamicPressure = pressure;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetWindDirection( Vector3<float> windDirection ) {
		if (windDirection == this.WindDirection)
			return;
		this.WindDirection = windDirection;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetWindSpeed( float windSpeed ) {
		if (windSpeed == this.WindSpeed)
			return;
		this.WindSpeed = windSpeed;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetLinearDistanceFromOcean( float distance ) {
		if (distance == this.LinearDistanceFromOcean)
			return;
		this.LinearDistanceFromOcean = distance;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetUpwindDistanceFromOcean( float distance ) {
		if (distance == this.UpwindDistanceFromOcean)
			return;
		this.UpwindDistanceFromOcean = distance;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetLocalRelief( float relief ) {
		if (relief == this.LocalRelief)
			return;
		this.LocalRelief = relief;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetLocalPressureRelief( float relief ) {
		if (relief == this.LocalPressureRelief)
			return;
		this.LocalPressureRelief = relief;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetRuggedness( float ruggedness ) {
		if (ruggedness == this.Ruggedness)
			return;
		this.Ruggedness = ruggedness;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetEvaporationMm( float evaporation ) {
		if (evaporation == this.EvaporationMm)
			return;
		this.EvaporationMm = evaporation;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetAbsoluteHumidityMm( float humidityMm ) {
		if (humidityMm == this.AbsoluteHumidityMm)
			return;
		this.AbsoluteHumidityMm = humidityMm;
		this._face.TriggerFaceStateChanged();
	}

	internal void SetPrecipitationMm( float precipitation ) {
		if (precipitation == this.PrecipitationMm)
			return;
		this.PrecipitationMm = precipitation;
		this._face.TriggerFaceStateChanged();
	}

	//internal void SetMoisture( float moisture ) {
	//	if (moisture == Moisture)
	//		return;
	//	Moisture = moisture;
	//	_face.TriggerFaceStateChanged();
	//}

	//internal void SetPrecipitation( float precipitation ) {
	//	if (precipitation == Precipitation)
	//		return;
	//	Precipitation = precipitation;
	//	_face.TriggerFaceStateChanged();
	//}
}


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