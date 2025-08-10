namespace Civlike.World.TectonicGeneration.Landscape;

public sealed class LandscapeParameters {
	public int PlateCount { get; set; } = 13;
	public float MinimumPlateMidpointDistance { get; set; } = 0.1f;
	public float ApproximateLandPercentage { get; set; } = 0.35f;
}
