namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class HydrologyParameters {
	/// <summary>
	/// Fraction of incoming water (from rainfall + snowmelt) that becomes surface runoff rather than infiltrating immediately.<br/>
	/// Dimensionless.<br/>
	/// Default value is 0.3, which is a typical runoff coefficient for many landscapes.<br/>
	/// </summary>
	public double RunoffCoefficient { get; set; } = 0.3;
	/// <summary>
	/// Minimum upstream catchment area (in m²) required before a channel (“stream”) is considered initiated. Faces with less contributing area remain overland flow.<br/>
	/// Defined in m².<br/>
	/// Default value is 1e6 m², which is a typical threshold for first-order streams in many landscapes.<br/>
	/// </summary>
	public double StreamInitiationArea { get; set; } = 1e6;
	/// <summary>
	/// Manning's roughness coefficient for channel flow.
	/// Defined in s/m^(1/3).<br/>
	/// Default value is 0.03 s·m⁻¹/³. Typical natural‐stream values run from ~0.025 – 0.05 s/m^(1/3).<br/>
	/// </summary>
	public double ManningN { get; set; } = 0.03;
	/// <summary>
	/// Maximum rate at which water can infiltrate into the ground<br/>
	/// Defined in mm/day
	/// Default value is 10 mm/day, which is a typical maximum infiltration rate for well-drained soils.<br/>
	/// </summary>
	public double MaxInfiltrationRate { get; set; } = 10;
}