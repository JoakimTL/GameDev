using System.Numerics;

namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class AtmosphericDynamicsParameters {
	/// <summary>Frictional turn‐down of geostrophic wind toward actual wind (0–1).</summary>
	public double LinearFrictionCoefficient { get; set; } = 1e-4;
	public double QuadraticFrictionCoefficient { get; set; } = 1e-1;
	public double MinimumQuadraticFrictionCoefficient { get; set; } = 1e-7;
	public float HadleyCellLatitudeWidth { get; set; } = 30 * (float) Math.PI / 180f;
	public float HadleyHorizontalStrength { get; set; } = 8f;
	public float HadleyVerticalStrength { get; set; } = 2f;
	public float PressureGradientCoefficient { get; set; } = 100;
	/// <summary>
	/// Max built up pressure out at the open ocean. The pressure buildup is most heavily felt at the coastal areas.
	/// </summary>
	public float SeaPressureBuildup { get; set; } = 2500;
	/// <summary>
	/// Build-up of sea pressure in Pa/m.
	/// </summary>
	public float SeaPressureDecayDistance { get; set; } = 1e-5f;
	public float CoriolisStrength { get; set; } = 0.001f;
	public Vector3 UpAxis { get; set; } = Vector3.UnitY;
}