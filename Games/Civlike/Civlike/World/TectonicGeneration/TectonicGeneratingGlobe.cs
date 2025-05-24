using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration.Parameters;

namespace Civlike.World.TectonicGeneration;
public sealed class TectonicGeneratingGlobe : GeneratingGlobeBase {
	public TectonicGeneratingGlobe() {
		InsolationProvider = new DefaultInsolationProvider( this );
	}

	public UniversalConstants UniversalConstants { get; } = new();
	public PlanetaryParameters PlanetaryParameters { get; } = new();
	public RadiativeAndCloudParameters RadiativeAndCloudParameters { get; } = new();
	public OceanMixedLayerAndCurrentsParameters OceanMixedLayerAndCurrents { get; } = new();
	public SurfaceRoughnessLengthConstants SurfaceRoughnessLengthConstants { get; } = new();
	public HorizontalDiffusionAndBoundaryLayerParameters HorizontalDiffusionAndBoundaryLayerParameters { get; } = new();
	public EndMemberSurfaceAlbedos EndMemberSurfaceAlbedos { get; } = new();
	public EndMemberThermalProperties EndMemberThermalProperties { get; } = new();
	public HydrologyAndRoutingConstants HydrologyAndRoutingConstants { get; } = new();
	public SoilDepthGenerationConstants SoilDepthGenerationConstants { get; } = new();
	public DynamicInitializationConstants DynamicInitializationConstants { get; } = new();
	public EvaporationParameters EvaporationParameters { get; } = new();
	public IInsolationProvider InsolationProvider { get; set; }
}
