namespace Civlike.World.TectonicGeneration.Parameters.Old;

public sealed class DynamicInitializationConstants {
	/// <summary>
	/// The initial relative humidity of the entire atmosphere.<br/>
	/// Defined as a fraction between 0 and 1, where 1 is 100% relative humidity.
	/// </summary>
	public double InitialRelativeHumidity { get; set; } = 0.8;
	/// <summary>
	/// The pressure at sea level.<br/>
	/// Defined in Pascals.
	/// </summary>
	public double SeaLevelPressure { get; set; } = 101325;
	/// <summary>
	/// Initial air temperature at the equatiorial region.<br/>
	/// Defined in Kelvin (K).
	/// </summary>
	public double EquatorialAirTemperature { get; set; } = 303.15;
	/// <summary>
	/// The reduction of air temperature from the equator to the poles.<br/>
	/// Defined in Kelvin (K).
	/// </summary>
	public double PolarAirTemperatureReduction { get; set; } = 70;
	/// <summary>
	/// Temperature drop per meter elevation.<br/>
	/// Defined in K/m.
	/// </summary>
	public double LapseRate { get; set; } = 0.0065;
	/// <summary>
	/// Initial sea surface temperature at the equator.<br/>
	/// Defined in Kelvin (K).
	/// </summary>
	public double EquatorSST { get; set; } = 300.15;
	/// <summary>
	/// The reduction of sea surface temperature from the equator to the poles.<br/>
	/// Defined in Kelvin (K).
	/// </summary>
	public double PolarSSTReduction { get; set; } = 30;
	/// <summary>
	/// The molar mass of dry air.<br/>
	/// Defined in kilograms per mole (kg/mol).<br/>
	/// Default value is 0.0289644 kg/mol, which is the molar mass of dry air at sea level.
	/// </summary>
	public double DryAirMolarMass { get;  set; } = 0.0289644;
}
