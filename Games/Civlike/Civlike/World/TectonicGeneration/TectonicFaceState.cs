using Civlike.World.GameplayState;
using Civlike.World.GenerationState;
using Engine;
using System.Numerics;

namespace Civlike.World.TectonicGeneration;

public class TectonicFaceState : FaceStateBase {
	public BaselineValues BaselineValues { get; } = new();

	/// <summary>
	/// The face's downslope neighbour; used for hydrology and erosion. This is the face that receives runoff from this face.
	/// </summary>
	public Face<TectonicFaceState>? DownslopeNeighbour { get; set; } = null!;
	public float DownslopeSlope { get; set; }
	public float DownslopeSlopeSqrt { get; set; }
	public float RunoffAccumulatedArea { get; set; }
	public float ChannelDepth { get; set; }

	public Temperature AirTemperature { get; set; }
	public Temperature AverageAirTemperature { get; set; }
	/// <summary>
	/// The temperature of the surface, be it land or water.
	/// </summary>
	public Temperature SurfaceTemperature { get; set; }
	public Temperature AverageSurfaceTemperature { get; set; }

	public float CachedLapseRate { get; set; }
	public Pressure Pressure { get; set; }
	public Pressure SeaPressure { get; set; }
	public Pressure CombinedPressure { get; set; }
	public float DistanceFromLand { get; set; }
	public Face<TectonicFaceState>? NearestLandFace { get; set; } = null!;
	/// <summary>
	/// Water‑vapor mixing ratio; updated by evaporation and advection. In kg/kg.
	/// </summary>
	public float SpecificHumidity { get; set; }

	public float AirDensity { get; set; }
	public float SaturationVaporPressure { get; set; }
	public float SaturationSpecificHumidity { get; set; }

	/// <summary>
	/// Fractional canopy cover; evolves via ET/PET balance
	/// </summary>
	public float VegetationFraction { get; set; }
	/// <summary>
	/// Effective roughness; blend of <see cref="TectonicGeneratingGlobe.SurfaceRoughnessLengthConstants"/>.<see cref="SurfaceRoughnessLengthConstants.BareGround"/> and <see cref="TectonicGeneratingGlobe.SurfaceRoughnessLengthConstants"/>.<see cref="SurfaceRoughnessLengthConstants.FullVegetation"/> using <see cref="VegetationFraction"/>
	/// </summary>
	public float SurfaceRoughness { get; set; }
	/// <summary>
	/// Surface reflectivity; function of snow (<see cref="SnowFraction"/>), vegetation (<see cref="VegetationFraction"/>), and base albedos
	/// </summary>
	public float Albedo { get; set; }
	/// <summary>
	/// Soil‑water content; updated via infiltration, runoff, and evapotranspiration. In mm.
	/// </summary>
	public float SoilMoisture { get; set; }
	/// <summary>
	/// Fractional seasonal snow cover
	/// </summary>
	public float SnowFraction { get; set; }
	/// <summary>
	/// The snow depth in meters.
	/// </summary>
	public float SnowDepth { get; set; }
	public float SnowMeltRate { get; set; }
	public float PrecipitationRate { get; set; }
	/// <summary>
	/// Surface and channel runoff volume. In m^3/s.
	/// </summary>
	public float RunoffAccumulation { get; set; }
	/// <summary>
	/// The depth of the water table; updated via infiltration, runoff, and evapotranspiration. In m.
	/// </summary>
	public float WaterDepth { get; set; }
	/// <summary>
	/// Flow in river channels after stream initiation. In m^3/s.
	/// </summary>
	public float RiverDischarge { get; set; }
	/// <summary>
	/// Surface ocean currents; driven by wind stress. In m/s.
	/// </summary>
	public Vector3 OceanCurrent { get; set; }
	/// <summary>
	/// The salinity of the ocean surface water, in parts per thousand (ppt). This is a measure of the concentration of dissolved salts in seawater.
	/// </summary>
	public float SeaSalinity { get; set; }
	/// <summary>
	/// Depth of regolith in meters. Regolith is the layer of loose, unconsolidated material covering solid bedrock, including soil, sediment, and weathered rock.
	/// </summary>
	public float SoilDepth { get; set; }
	/// <summary>
	/// The capacity of the soil to hold water, in mm.
	/// </summary>
	public float SoilMoistureCapacity { get; set; }
	/// <summary>
	/// The thermal capacity of the face per volume, in J/(m^3*K).
	/// </summary>
	public float ThermalCapacityPerVolume { get; set; }
	/// <summary>
	/// The thermal capacity of the face per area, in J/(m^2*K). This thermal capacity has taken into account the depth of the face, so it is effectively the thermal capacity per unit area of the face's surface.
	/// </summary>
	public float ThermalCapacityPerArea { get; set; }
	/// <summary>
	/// The thermal conductivity of the face, in W/(m*K).
	/// </summary>
	public float ThermalConductivity { get; set; }
	/// <summary>
	/// The emissivity of the face, which is a measure of how efficiently it emits thermal radiation. This value ranges from 0 to 1, where 1 indicates perfect emission.
	/// </summary>
	public float Emissivity { get; set; }
	public Vector3 HadleyWinds { get; set; }
	/// <summary>
	/// The wind speed at the face, represented as a vector in 3D space. This vector indicates the direction and magnitude of the wind. The magnitude is in m/s.
	/// </summary>
	public Vector3 Wind { get; set; } = Vector3.Zero;
	/// <summary>
	/// The wind portion tangential to the surface of this face.
	/// </summary>
	public Vector3 TangentialWind { get; set; } = Vector3.Zero;
	public float CoriolisFactor { get; set; }

	public int LatitudeIndex { get; set; }

	public float PressureElevationMean { get; set; }
		//get {
		//	float delta = this.FreshwaterDepth;
		//	if (this.Face.IsOcean) 
		//		delta = -this.BaselineValues.ElevationMean;
		//	return BaselineValues.ElevationMean + delta;
		//}
	//}

	public override void Apply( Face.Builder builder ) {
		builder.Debug_Arrow = TangentialWind.FromNumerics<float>() * 0.1f;//(DownslopeNeighbour is not null) ? (DownslopeNeighbour.Center - Face.Center).Normalize<Vector3<float>, float>() * 0.33f : Vector3<float>.Zero;//Wind.Normalize<Vector3<float>, float>();
		builder.Debug_Color = (SpecificHumidity / 0.002f, float.Max( -AverageAirTemperature.Celsius, 0 ) / 120, Face.IsOcean ? 1 : 0, 1);
	}
}
