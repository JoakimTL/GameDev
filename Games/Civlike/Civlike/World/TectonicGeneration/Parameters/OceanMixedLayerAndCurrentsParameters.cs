namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class OceanMixedLayerAndCurrentsParameters {
	/// <summary>
	/// Depth of ocean surface layer participating in heat exchange<br/>
	/// Defined in m
	/// </summary>
	public double MixedLayerDepth { get; set; } = 50;
	/// <summary>
	/// Density of seawater in kg/m^3<br/> Might want to allow different salinity levels?=??
	/// </summary>
	public double SeawaterDensity { get; set; } = 1025;
	/// <summary>
	/// Drag coefficient of the ocean surface<br/>
	/// Defined as dimensionless
	/// </summary>
	public double OceanDragCoefficient { get; set; } = 1.2e-3;
	/// <summary>
	/// Horizontal diffusion of heat/salinity in ocean mixed layer<br/>
	/// Defined in m^2/s
	/// </summary>
	public double EddyDiffusivity { get; set; } = 1000;
}