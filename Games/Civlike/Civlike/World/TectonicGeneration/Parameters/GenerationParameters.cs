namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class GenerationParameters {
	/// <summary>
	/// How many seconds pass each simulation step.
	/// </summary>
	public double SimulationTimeStepSeconds { get; set; } = 86400f;
	public double SpinUpDurationSeconds { get; set; } = 86400f;// * 365.2422f * .1f;
}