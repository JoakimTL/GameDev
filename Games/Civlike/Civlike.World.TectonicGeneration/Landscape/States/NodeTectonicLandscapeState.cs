using Civlike.World.State;
using Civlike.World.TectonicGeneration.NoiseProviders;

namespace Civlike.World.TectonicGeneration.Landscape.States;

public sealed class NodeTectonicLandscapeState : StateBase<Node> {
	public SphericalVoronoiRegion Region { get; set; } = null!;

	public float CrustThicknessKm { get; set; }
	public float CrustDensity { get; set; }
	public float BaseElevation { get; set; }
	public float DistanceToPlateEdgeRadians { get; set; }
	public bool IsEdgeNode { get; set; } = false;
	public bool IsRidgeSeed { get; set; }
	public bool IsConvergentSeed { get; set; }
	public float TransformSlipKmPerMa { get; set; }
	public float RidgeHalfRateKmPerMa { get; set; }
}