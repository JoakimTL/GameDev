namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class AtmosphericDynamicsParameters {
	/// <summary>Frictional turn‐down of geostrophic wind toward actual wind (0–1).</summary>
	public double LinearFrictionCoefficient { get; set; } = 1e-5;
	public double QuadraticFrictionCoefficient { get; set; } = 7e-6;
	public double MinimumQuadraticFrictionCoefficient { get; set; } = 1e-7;
}