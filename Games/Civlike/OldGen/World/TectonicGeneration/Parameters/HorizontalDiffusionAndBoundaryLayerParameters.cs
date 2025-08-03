namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class HorizontalDiffusionAndBoundaryLayerParameters {
	/// <summary>
	/// Horizontal mixing of heat; approximates mean advective effects.<br/>
	/// Defined in m^2/s
	/// </summary>
	public double ThermalDiffusivity { get; set; } = 1e5;
	/// <summary>
	/// Horizontal mixing of water vapor.<br/>
	/// Defined in m^2/s
	/// </summary>
	public double MoistureDiffusivity { get; set; } = 1e5;
	/// <summary>
	/// Depth of atmospheric boundary layer used in wind-drag calculations.<br/>
	/// Defined in m
	/// </summary>
	public double BoundaryLayerHeight { get; set; } = 1000;
}