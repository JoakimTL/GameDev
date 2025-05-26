namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class AtmosphericDynamicsParameters {
	/// <summary>Frictional turn‐down of geostrophic wind toward actual wind (0–1).</summary>
	public double FrictionCoefficient { get; set; } = 0.1;
}