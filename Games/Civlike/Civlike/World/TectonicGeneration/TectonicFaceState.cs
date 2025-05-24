using Engine;

namespace Civlike.World.TectonicGeneration;

public class TectonicFaceState {
	public BaselineValues BaselineValues { get; } = new();

	public Temperature Temperature { get; set; }
	public Pressure Pressure { get; set; }
	/// <summary>
	/// Water‑vapor mixing ratio; updated by evaporation and advection. In kg/kg.
	/// </summary>
	public float SpecificHumidity { get; set; }
	/// <summary>
	/// Fractional canopy cover; evolves via ET/PET balance
	/// </summary>
	public float VegetationFraction { get; set; }
	/// <summary>
	/// Effective roughness; blend of <see cref="TectonicGeneratingGlobe.SurfaceRoughnessLengthConstants"/>.<see cref="Parameters.SurfaceRoughnessLengthConstants.BareGround"/> and <see cref="TectonicGeneratingGlobe.SurfaceRoughnessLengthConstants"/>.<see cref="Parameters.SurfaceRoughnessLengthConstants.FullVegetation"/> using <see cref="VegetationFraction"/>
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
	/// Surface and channel runoff volume. In m^3/s.
	/// </summary>
	public float RunoffAccumulation { get; set; }
	/// <summary>
	/// The depth of the water table; updated via infiltration, runoff, and evapotranspiration. In m.
	/// </summary>
	public float FreshwaterDepth { get; set; }
	/// <summary>
	/// Flow in river channels after stream initiation. In m^3/s.
	/// </summary>
	public float RiverDischarge { get; set; }
	/// <summary>
	/// Surface ocean currents; driven by wind stress. In m/s.
	/// </summary>
	public Vector3<float> OceanCurrent { get; set; }
	/// <summary>
	/// Temperature of ocean mixed layer; updated via advection and heat exchange. In K.
	/// </summary>
	public float SeaSurfaceTemperature { get; set; }
	/// <summary>
	/// The salinity of the ocean surface water, in parts per thousand (ppt). This is a measure of the concentration of dissolved salts in seawater.
	/// </summary>
	public float SeaSalinity { get; set; }
}

public sealed class BaselineValues {
	/// <summary>
	/// The mean height of the face, in meters.
	/// </summary>
	public float ElevationMean { get; set; }
	/// <summary>
	/// The standard deviation of the height of the face, in meters.
	/// </summary>
	public float ElevationStandardDeviation { get; set; }
	/// <summary>
	/// The gradient of the face, in 3d vector form. The magnitude of the vector is the percent slope.
	/// </summary>
	public Vector3<float> Gradient { get; set; }
	/// <summary>
	/// The base albedo of the face, in the range [0, 1].
	/// </summary>
	public float AlbedoBase { get; set; }
	/// <summary>
	/// The capacity of the soil to hold water, in mm.
	/// </summary>
	public float SoilMoistureCapacity { get; set; }
	/// <summary>
	/// The thermal capacity of the face, in J/(m^3*K).
	/// </summary>
	public float ThermalCapacity { get; set; }
	/// <summary>
	/// The thermal conductivity of the face, in W/(m*K).
	/// </summary>
	public float ThermalConductivity { get; set; }
	/// <summary>
	/// A generalized measure of how often seismic events occur on this tile. Seismic events propagate to other nearby tiles as well, but the magnitude of the event drops off with distance.
	/// </summary>
	public float SeismicActivity { get; set; }
	public float RuggednessFactor { get; set; }
}