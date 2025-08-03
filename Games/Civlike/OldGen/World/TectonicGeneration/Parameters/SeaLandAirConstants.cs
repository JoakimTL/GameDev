namespace OldGen.World.TectonicGeneration.Parameters;

public sealed class SeaLandAirConstants {
	/// <summary>
	/// The depth of the surface thermal layer, which is the layer of the Earth's crust that is directly affected by solar heating and atmospheric conditions.<br/>
	/// Defined in meters.
	/// Default value is 1 meter, which is a common depth for the surface layer in many models.
	/// </summary>
	public double SurfaceThermalLayerDepth { get; set; } = 1;
	/// <summary>
	/// Depth of ocean surface layer participating in heat exchange.<br/>
	/// Defined in meters.
	/// Default value is 50 meters, which is a common depth for the mixed layer in oceans.
	/// </summary>
	public double OceanMixedLayerDepth { get; set; } = 50;
	/// <summary>
	/// Saturation vapor pressure at T=273.15 K, used as base in Clausius–Clapeyron equation.<br/>
	/// Define in Pascals (Pa).<br/>
	/// Default value is 610.94 Pa, which is the saturation vapor pressure of water at 0 degrees Celsius (273.15 K).
	/// </summary>
	public double ReferenceSaturationVaporPressure { get; set; } = 610.94;
	/// <summary>
	/// Ratio of molecular weight of water vapor to dry air (M_v/M_d).<br/>
	/// Default value is 0.622, which is the ratio of the molecular weight of water vapor (18.01528 g/mol) to that of dry air (28.9644 g/mol).<br/>
	/// </summary>
	public double MolecularWeightRatioVaporDryAir { get; set; } = 0.622;
	/// <summary>
	/// The molar mass of dry air.<br/>
	/// Defined in kilograms per mole (kg/mol).<br/>
	/// Default value is 0.0289644 kg/mol, which is the molar mass of dry air at sea level.
	/// </summary>
	public double DryAirMolarMass { get; set; } = 0.0289644;
	/// <summary>
	/// The pressure at sea level, in Pascals.
	/// Default value is 101325 Pa, which is the standard atmospheric pressure at sea level.
	/// </summary>
	public double SeaLevelPressure { get; set; } = 101_325;
	/// <summary>
	/// Controls exponential decay of soil depth with terrain gradient<br/>
	/// Defined as unitless constant.
	/// </summary>
	public double SlopeDecayConstant { get; set; } = 3;
	/// <summary>
	/// Reference salinity of ocean surface water<br/>
	/// Defined in parts per thousand (ppt), typical value is around 35 ppt
	/// </summary>
	public double OceanSalinityReference { get; set; } = 35.0;
}