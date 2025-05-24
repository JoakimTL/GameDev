namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class PlanetaryParameters {
	/// <summary>
	/// How fast matter is moving towards the surface of the planet.<br/>
	/// Defined in m/s.
	/// </summary>
	public float Gravity { get; set; } = 9.81f;

	/// <summary>
	/// The mean solar constant is the amount of solar energy received per unit area at the top of the Earth's atmosphere on a surface perpendicular to the Sun's rays. It is used in the calculation of the amount of solar energy received by the Earth.<br/>
	/// Defined in W/m^2.
	/// </summary>
	public float MeanSolarConstant { get; set; } = 1361f;

	/// <summary>
	/// The time it takes for the Earth to complete one full rotation on its axis.<br/>
	/// Defined in seconds.
	/// </summary>
	public float RotationPeriod { get; set; } = 86400f; // 24 hours in seconds
	/// <summary>
	/// The tilt of the Earth's axis in degrees. This affects the amount of sunlight received at different latitudes and is responsible for the seasons.<br/>
	/// Defined in degrees.
	/// </summary>
	public float Obliquity { get; set; } = 23.44f;

	/// <summary>
	/// The variation in the Earth's orbit around the Sun.<br/>
	/// Defined in degrees.
	public float Eccentricity { get; set; } = 0.0167f;

}
