using System.Runtime.Intrinsics.Arm;

namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class AtmosphericDynamicsParameters {
	/// <summary>Frictional turn‐down of geostrophic wind toward actual wind (0–1).</summary>
	public double LinearFrictionCoefficient { get; set; } = 1e-5;
	public double QuadraticFrictionCoefficient { get; set; } = 1e-1;
	public double MinimumQuadraticFrictionCoefficient { get; set; } = 1e-7;
	public float HadleyCellLatitudeWidth { get; set; } = 30 * (float) Math.PI / 180f;
	public float HadleyStrength { get; set; } = 2f;
	public float PressureGradientCoefficient { get; set; } = 400;
}