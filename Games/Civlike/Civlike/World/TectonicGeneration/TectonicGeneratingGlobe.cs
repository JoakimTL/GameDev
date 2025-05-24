using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civlike.World.TectonicGeneration;
public sealed class TectonicGeneratingGlobe : GeneratingGlobeBase {
	public UniversalConstants UniversalConstants { get; } = new();
	public PlanetaryParameters PlanetaryParameters { get; } = new();
	public RadiativeAndCloudParameters RadiativeAndCloudParameters { get; } = new();
	public OceanMixedLayerAndCurrentsParameters OceanMixedLayerAndCurrents { get; } = new();
	public SurfaceRoughnessLengthConstants SurfaceRoughnessLengthConstants { get; } = new();
	public HorizontalDiffusionAndBoundaryLayerParameters HorizontalDiffusionAndBoundaryLayerParameters { get; } = new();
	public EndMemberSurfaceAlbedos EndMemberSurfaceAlbedos { get; } = new();
	public EndMemberThermalProperties EndMemberThermalProperties { get; } = new();
	public HydrologyAndRoutingConstants HydrologyAndRoutingConstants { get; } = new();
}
