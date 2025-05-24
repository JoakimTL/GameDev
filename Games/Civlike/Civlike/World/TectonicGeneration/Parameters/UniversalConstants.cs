namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class UniversalConstants {
	/// <summary>
	/// Ideal-gas law constant<br/>
	/// Defined in J/(mol*K)
	/// </summary>
	public double UniversalGasConstant { get; set; } = 8.31446261815324;
	/// <summary>
	/// The specific gas constant is a physical constant that describes the relationship between the pressure, volume, and temperature of a gas. It is used in the calculation of the ideal gas law and is defined as the ratio of the universal gas constant to the molar mass of the gas.<br/>
	/// Defined in J/(kg*K).
	/// </summary>
	public double SpecificGasConstant { get; set; } = 287.058;
	/// <summary>
	/// The specific heat capacity of air is the amount of energy required to raise the temperature of a unit mass of air by one degree Celsius. It is used in the calculation of the amount of energy required to heat or cool air.<br/>
	/// Defined in J/(kg*K).
	/// </summary>
	public double SpecificHeatCapacity { get; set; } = 1004.68506;
	/// <summary>
	/// The latent heat of fusion is the amount of energy required to change a substance from a solid to a liquid at constant temperature and pressure. It is used in the calculation of the amount of energy required to melt ice or freeze water.<br/>
	/// Defined in J/kg.
	/// </summary>
	public double LatentHeatOfVaporization { get; set; } = 2.5e6;
	/// <summary>
	/// The Stefan-Boltzmann constant is a physical constant that describes the power radiated from a black body in terms of its temperature. It is used in the calculation of the amount of energy radiated by a surface per unit area.<br/>
	/// Defined in W/(m^2*K^4).
	/// </summary>
	public double StefanBoltzmannConstant { get; set; } = 5.670374419e-8;
	/// <summary>
	/// The von Kármán constant is a dimensionless constant that is used in the calculation of the roughness length of a surface. It is used to calculate the roughness length of a surface in the logarithmic wind profile equation.<br/>
	/// Defined as a dimensionless constant.
	/// </summary>
	public double VonKármánConstant { get; set; } = 0.5;
	/// <summary>
	/// Ratio of latent heat of vaporization (Lᵥ) to gas constant for water vapor (Rᵥ), used in exponent of saturation vapor formula.<br/>
	/// Defined in Kelvin (K).
	/// </summary>
	public double ClausiusClapeyronExponent { get; set; } = 5410;
	/// <summary>
	/// Ratio of molecular weight of water vapor to dry air (M_v/M_d).
	/// </summary>
	public double MolecularWeightRatioVaporDryAir { get; set; } = 0.622;
}
