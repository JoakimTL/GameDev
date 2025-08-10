using Civlike.World.State;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;

namespace Civlike.World.TectonicGeneration.Landscape.States;

public sealed class SphericalVoronoiRegionTectonicPlateState : StateBase<SphericalVoronoiRegion> {
	public Vector4<float> PlateColor { get; set; }
	public List<Node> Nodes { get; } = [];
	public Dictionary<SphericalVoronoiRegion, PlateEdge> Edges { get; } = [];

	public bool IsContinentalCore { get; set; }   // “kernel” flag
	public bool IsOceanicPlate { get; set; }
	public float MeanCrustThicknessKm { get; set; }  // 35-70 km vs 7-10 km
	public float MeanCrustDensity { get; set; }  // 2.8 vs 2.9 g/cm³
	public float BaseIsoHeight { get; set; }  // metres above datum

	/// <summary>
	/// The tangential velocity of the plate centered on the plate midpoint. The vector is the plane of rotation. The magnitude of the velocity is in radians/million years.
	/// </summary>
	public Vector3<float> AngularVelocity { get; set; }
}


public sealed class PlateEdge( SphericalVoronoiRegionTectonicPlateState currentRegion, SphericalVoronoiRegionTectonicPlateState neighbourRegion ) {
	public SphericalVoronoiRegionTectonicPlateState Current { get; } = currentRegion;
	public SphericalVoronoiRegionTectonicPlateState Neighbour { get; } = neighbourRegion;
	public HashSet<Node> Nodes { get; } = [];
	public List<PlateEdgeSegment> Segments { get; } = [];
}

public sealed class PlateEdgeSegment( IEnumerable<Node> nodes ) {
	public IReadOnlyList<Node> Nodes { get; } = nodes.ToList().AsReadOnly();
}