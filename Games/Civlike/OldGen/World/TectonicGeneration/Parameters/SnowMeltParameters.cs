namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class SnowMeltParameters {
	public double SnowMeltTemperatureThreshold { get; set; } = Temperature.ZeroCelsius;
	/// <summary>
	/// The depth of snow at which the snow layer starts acting more as insulation rather than a heat sink.<br/>
	/// Defined in meters.
	/// </summary>
	public double SnowInsulationCoefficient { get; set; } = 0.2;
}