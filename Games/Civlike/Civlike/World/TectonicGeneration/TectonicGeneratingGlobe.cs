using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration.Parameters;
using Engine.Structures;
using Engine;

namespace Civlike.World.TectonicGeneration;
public sealed class TectonicGeneratingGlobe : GeneratingGlobeBase {
	public TectonicGeneratingGlobe() : base( typeof( TectonicFaceState ) ) {
		InsolationProvider = new DefaultInsolationProvider( this );
	}
	//public RadiativeAndCloudParameters RadiativeAndCloudParameters { get; } = new();
	//public OceanMixedLayerAndCurrentsParameters OceanMixedLayerAndCurrents { get; } = new();
	//public SurfaceRoughnessLengthConstants SurfaceRoughnessLengthConstants { get; } = new();
	//public HorizontalDiffusionAndBoundaryLayerParameters HorizontalDiffusionAndBoundaryLayerParameters { get; } = new();
	//public EndMemberSurfaceAlbedos EndMemberSurfaceAlbedos { get; } = new();
	//public EndMemberThermalProperties EndMemberThermalProperties { get; } = new();
	//public HydrologyAndRoutingConstants HydrologyAndRoutingConstants { get; } = new();
	//public SoilDepthGenerationConstants SoilDepthGenerationConstants { get; } = new();
	//public DynamicInitializationConstants DynamicInitializationConstants { get; } = new();

	public UniversalConstants UniversalConstants { get; } = new();
	public PlanetaryConstants PlanetaryConstants { get; } = new();
	public SeaLandAirConstants SeaLandAirConstants { get; } = new();
	public EndMemberProperties EndMemberProperties { get; } = new();
	public InitializationParameters InitializationParameters { get; } = new();
	public RadiationParameters RadiationParameters { get; } = new();
	public EvaporationParameters EvaporationParameters { get; } = new();
	public AtmosphericDynamicsParameters AtmosphericDynamicsParameters { get; } = new();
	public HydrologyParameters HydrologyParameters { get; } = new();
	public SnowMeltParameters SnowMeltParameters { get; } = new();
	public IInsolationProvider InsolationProvider { get; set; }

	public IReadOnlyList<Face<TectonicFaceState>> TectonicFaces { get; private set; } = [];
	public IReadOnlyList<Face<TectonicFaceState>> NonOceanTectonicFacesByElevationMean { get; private set; } = [];
	public OcTree<Face<TectonicFaceState>, float> FaceTree { get; } = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 4, false );

	protected override void OnFacesSet() {
		TectonicFaces = Faces.OfType<Face<TectonicFaceState>>().ToList();
		FaceTree.Clear();
		foreach (Face<TectonicFaceState> face in TectonicFaces)
			FaceTree.Add( face );
	}

	public void UpdateFaceLists() {
		NonOceanTectonicFacesByElevationMean = TectonicFaces
			.Where( face => !face.IsOcean )
			.OrderByDescending( face => face.State.BaselineValues.ElevationMean )
			.ToList();
	}
}