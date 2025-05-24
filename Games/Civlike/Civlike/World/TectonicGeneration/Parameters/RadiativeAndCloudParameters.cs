namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class RadiativeAndCloudParameters {
	public double LandEmissivity { get; set; } = 0.98;
	public double OceanEmissivity { get; set; } = 0.96;
	public double CriticalRelativeHumidityForRain { get; set; } = 1.02;
	public double ShortwaveScatteringCoefficient { get; set; } = 0.2;
	public double LongwaveAbsorptionCoefficient { get; set; } = 0.9;
}