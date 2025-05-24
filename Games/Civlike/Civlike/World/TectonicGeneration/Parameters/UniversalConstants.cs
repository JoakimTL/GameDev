namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class UniversalConstants {
	/// <summary>
	/// Heat capacity of water in J/(m^3*K)
	/// </summary>
	public float UniversalGasConstant { get; set; } = 8.31446261815324f;
	/// <summary>
	/// The specific gas constant is a physical constant that describes the relationship between the pressure, volume, and temperature of a gas. It is used in the calculation of the ideal gas law and is defined as the ratio of the universal gas constant to the molar mass of the gas.<br/>
	/// Defined in J/(kg*K).
	/// </summary>
	public float SpecificGasConstant { get; set; } = 287.058f;
	/// <summary>
	/// The specific heat capacity of air is the amount of energy required to raise the temperature of a unit mass of air by one degree Celsius. It is used in the calculation of the amount of energy required to heat or cool air.<br/>
	/// Defined in J/(kg*K).
	/// </summary>
	public float SpecificHeatCapacity { get; set; } = 1005f;
	/// <summary>
	/// The latent heat of fusion is the amount of energy required to change a substance from a solid to a liquid at constant temperature and pressure. It is used in the calculation of the amount of energy required to melt ice or freeze water.<br/>
	/// Defined in J/kg.
	/// </summary>
	public float LatentHeatOfVaporization { get; set; } = 2.5e6f;
	/// <summary>
	/// The Stefan-Boltzmann constant is a physical constant that describes the power radiated from a black body in terms of its temperature. It is used in the calculation of the amount of energy radiated by a surface per unit area.<br/>
	/// Defined in W/(m^2*K^4).
	/// </summary>
	public float StefanBoltzmannConstant { get; set; } = 5.670374419e-8f;
	/// <summary>
	/// The von Kármán constant is a dimensionless constant that is used in the calculation of the roughness length of a surface. It is used to calculate the roughness length of a surface in the logarithmic wind profile equation.<br/>
	/// Defined as a dimensionless constant.
	/// </summary>
	public float VonKármánConstant { get; set; } = 0.5f;
}
