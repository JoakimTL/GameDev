namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class SurfaceRoughnessLengthConstants {
	/// <summary>
	/// The surface roughness length for bare ground.<br/>
	/// Defined in meters.
	/// </summary>
	public float BareGround { get; set; } = 0.001f;
	/// <summary>
	/// The surface roughness length for full vegetation.<br/>
	/// Defined in meters.
	/// </summary>
	public float FullVegetation { get; set; } = 2.5f;
}