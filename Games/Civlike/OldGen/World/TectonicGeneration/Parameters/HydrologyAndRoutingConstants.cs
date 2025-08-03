namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class HydrologyAndRoutingConstants {
	/// <summary>
	/// Flow-resistance coefficient for overland & channel flow<br/>
	/// Defined in m^(-1/3)
	/// </summary>
	public double ManningsN { get; set; } = 0.03;
	/// <summary>
	/// Min. rainfall rate to form first-order streams<br/>
	/// Defined in mm/day
	/// </summary>
	public double SteamInitiationThreshold { get; set; } = 1;
	/// <summary>
	/// Spread of sheet flow before channelization<br/>
	/// Defined in m^2/s
	/// </summary>
	public double OverlandDiffusivity { get; set; } = 10;
	/// <summary>
	/// Fraction of rain that infiltrates per day<br/>
	/// Defined in fraction/day
	/// </summary>
	public double InfiltrationRateCoefficient { get; set; } = 0.5;
	/// <summary>
	/// Rate of evaporation from water bodies<br/>
	/// Defined in m/day per W/m^2
	/// </summary>
	public double EvaporationRateCoefficient { get; set; } = 0.5;
}