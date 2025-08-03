namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class EvaporationParameters {
	/// <summary>
	/// Bulk transfer coefficient for evaporation over land surfaces.<br/>
	/// Defined as a dimensionless coefficient (unitless).
	/// </summary>
	public double LandBulkTransferCoefficient { get; set; } = 1.2e-3;

	/// <summary>
	/// Bulk transfer coefficient for evaporation over water surfaces.<br/>
	/// Defined as a dimensionless coefficient (unitless).
	/// </summary>
	public double WaterBulkTransferCoefficient { get; set; } = 1.3e-3;

	/// <summary>
	/// Height of the atmospheric boundary layer used when converting evaporation mass flux to humidity change.<br/>
	/// Defined in meters (m).
	/// </summary>
	public double BoundaryLayerHeight { get; set; } = 1000.0;
}
