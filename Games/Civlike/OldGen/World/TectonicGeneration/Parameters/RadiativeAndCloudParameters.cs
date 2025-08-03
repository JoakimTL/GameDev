namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class RadiativeAndCloudParameters {
	public double LandEmissivity { get; set; } = 0.98;
	public double OceanEmissivity { get; set; } = 0.95;
	public double CriticalRelativeHumidityForRain { get; set; } = 1.02;
	public double ShortwaveScatteringCoefficient { get; set; } = 0.2;
	public double LongwaveAbsorptionCoefficient { get; set; } = 0.9;
	public double AtmosphericShortwaveAbsorption { get; set; } = 0.2;
	public double AtmosphericLongwaveAbsorption { get; set; } = 0.4;
	public double AirSeaCouplingCoefficient { get; set; } = 0.5;
	public float SnowEmissivity { get; internal set; }
}
