using Civlike.World.State;
using Civlike.World.TectonicGeneration.NoiseProviders;

namespace Civlike.World.TectonicGeneration.Landscape.States;

public sealed class GlobeTectonicPlateState : StateBase<Globe> {
	public List<SphericalVoronoiRegion> Regions { get; } = [];
}
