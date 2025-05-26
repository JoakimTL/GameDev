namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class PhysicalProperties(double density, double thermalCapacity, double thermalConductivity, double emissivity, double albedo, double minimumRoughnessLength, double maximumRoughnessLength ) {
	/// <summary>
	/// Density of the material in kg/m^3
	/// </summary>
	public double Density { get; set; } = density;
	/// <summary>
	/// Heat capacity of the material in J/(kg*K)
	/// </summary>
	public double ThermalCapacity { get; set; } = thermalCapacity;
	/// <summary>
	/// Thermal conductivity of the material in W/(m*K)
	/// </summary>
	public double ThermalConductivity { get; set; } = thermalConductivity;
	/// <summary>
	/// The fraction of longwave radiation emitted by the surface, ranging from 0 (no emission) to 1 (perfect emission).
	/// </summary>
	public double Emissivity { get; set; } = emissivity;
	/// <summary>
	/// The percentage of shortwave radiation reflected by the surface, ranging from 0 (no reflection) to 1 (total reflection).
	/// </summary>
	public double Albedo { get; set; } = albedo;
	/// <summary>
	/// The minimum roughness of terrain in meters.
	/// </summary>
	public double MinimumRoughnessLength { get; set; } = minimumRoughnessLength;
	/// <summary>
	/// The maximum roughness of terrain in meters.
	/// </summary>
	public double MaximumRoughnessLength { get; set; } = maximumRoughnessLength;
}