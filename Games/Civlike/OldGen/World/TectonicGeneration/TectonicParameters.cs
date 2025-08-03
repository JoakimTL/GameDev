namespace OldGen.World.TectonicGeneration;

public sealed class TectonicParameters {
	public float BaseHeightVariance { get; init; }
	public float FaultMaxHeight { get; init; }
	public float MountainHeight { get; init; }
	public int PlateHeightVariance { get; init; }
	public int PlateHeight { get; init; }
	public int PlateCountVariance { get; init; }
	public int PlateCountBase { get; init; }
	public int OceanSeeds { get; init; }
}