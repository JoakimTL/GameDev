using Civlike.World.TectonicGeneration.Landscape;

namespace Civlike.World.TectonicGeneration;

public sealed class TectonicGenerationParameters {
	public Random SeedProvider { get; set; } = new();

	public TectonicLandmassParameters TectonicParameters { get; set; } = new();
	public LandscapeParameters LandscapeParameters { get; set; } = new();
}
