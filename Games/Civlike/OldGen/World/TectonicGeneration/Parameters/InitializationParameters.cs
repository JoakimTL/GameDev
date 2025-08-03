namespace OldGen.World.TectonicGeneration.Parameters;

public class InitializationParameters {
	/// <summary>
	/// The initial relative humidity of the entire atmosphere.
	/// Defined as a fraction between 0 and 1, where 1 is 100% RH.
	/// </summary>
	public double InitialRelativeHumidity { get; set; } = 0.8;

	/// <summary>
	/// Initial air temperature at the equatorial region, in Kelvin.
	/// </summary>
	public double EquatorialAirTemperature { get; set; } = 303.15;

	/// <summary>
	/// Reduction of air temperature from equator to poles, in Kelvin.
	/// </summary>
	public double PolarAirTemperatureReduction { get; set; } = 70.0;

	/// <summary>
	/// Temperature drop per meter of elevation, in K/m (environmental lapse rate).
	/// </summary>
	public double LapseRate { get; set; } = 0.0065;

	/// <summary>
	/// Initial sea surface temperature at the equator, in Kelvin.
	/// </summary>
	public double EquatorialSeaSurfaceTemperature { get; set; } = 300.15;

	/// <summary>
	/// Reduction of sea surface temperature from equator to poles, in Kelvin.
	/// </summary>
	public double PolarSeaSurfaceTemperatureReduction { get; set; } = 30.0;
	/// <summary>
	/// Reference base soil depth on flat, wet terrain.<br/>
	/// Defined in meters.
	/// </summary>
	public double SoilDepthBase { get; set; } = 2;
	/// <summary>
	/// Amplitude of fractal noise perturbation for local variation<br/>
	/// Defined as unitless constant.
	/// </summary>
	public double SoilNoiseAmplitude { get; set; } = 0.1;
	/// <summary>
	/// Base soil porosity, representing the fraction of soil volume that is pore space<br/>
	/// Defined as unitless constant, typically around 0.4 for mineral soils.
	/// </summary>
	public double SoilPorosityBase { get; set; } = 0.4;
	/// <summary>
	/// Amplitude of fractal noise perturbation for soil porosity<br/>
	/// Defined as unitless constant.
	/// </summary>
	public double SoilPorosityNoiseAmplitude { get; set; } = 0.1;
}