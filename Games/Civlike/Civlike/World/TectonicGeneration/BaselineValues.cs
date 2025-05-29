using System.Numerics;

namespace Civlike.World.TectonicGeneration;

public sealed class BaselineValues {
	/// <summary>
	/// The mean height of the face, in meters.
	/// </summary>
	public float ElevationMean { get; set; }
	/// <summary>
	/// The standard deviation of the height of the face, in meters.
	/// </summary>
	public float ElevationStandardDeviation { get; set; }
	/// <summary>
	/// The gradient of the face, in 3d vector form. The magnitude of the vector is the percent slope.
	/// </summary>
	public Vector3 Gradient { get; set; }
	/// <summary>
	/// A generalized measure of how often seismic events occur on this tile. Seismic events propagate to other nearby tiles as well, but the magnitude of the event drops off with distance.
	/// </summary>
	public float SeismicActivity { get; set; }
	public float RuggednessFactor { get; set; }
}