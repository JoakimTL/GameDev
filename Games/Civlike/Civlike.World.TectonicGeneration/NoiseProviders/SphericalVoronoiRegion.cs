using Civlike.World.State;
using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class SphericalVoronoiRegion( Vector3<float> position ) : StateContainerBase<SphericalVoronoiRegion> {
	public readonly Vector3<float> Position = position;
}
