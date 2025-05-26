namespace Civlike.World.TectonicGeneration.Parameters.Old;

public sealed class SurfaceRoughnessLengthConstants {
	/// <summary>
	/// The surface lowest roughness length for bare ground.<br/>
	/// Defined in meters.
	/// </summary>
	public float BareGroundMinimum { get; set; } = 0.001f;
	/// <summary>
	/// The surface highest roughness length for bare ground.<br/>
	/// Defined in meters.
	/// </summary>
	public float BareGroundMaximum { get; set; } = 0.05f;
	/// <summary>
	/// The surface roughness length for full vegetation.<br/>
	/// Defined in meters.
	/// </summary>
	public float FullVegetation { get; set; } = 2.5f;
}
