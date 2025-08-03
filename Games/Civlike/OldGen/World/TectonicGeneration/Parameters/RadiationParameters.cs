namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class RadiationParameters {
	public double AtmosphericShortwaveAbsorption { get; set; } = 0.2;
	public double AtmosphericLongwaveAbsorption { get; set; } = 0.2;
	public double AirSeaCouplingCoefficient { get; set; } = 0.5;
}
