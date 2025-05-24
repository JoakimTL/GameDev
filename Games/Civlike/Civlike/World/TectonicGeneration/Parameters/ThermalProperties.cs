namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class ThermalProperties( double density, double heatCapacity, double thermalConductivity ) {
	/// <summary>
	/// Density of the material in kg/m^3
	/// </summary>
	public double Density { get; set; } = density;
	/// <summary>
	/// Heat capacity of the material in J/(m^3*K)
	/// </summary>
	public double HeatCapacity { get; set; } = heatCapacity;
	/// <summary>
	/// Thermal conductivity of the material in W/(m*K)
	/// </summary>
	public double ThermalConductivity { get; set; } = thermalConductivity;
}