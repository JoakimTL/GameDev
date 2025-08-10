using Civlike.World.Geometry;
using Civlike.World.State;

namespace Civlike.World;

public static class GlobeGenerator {
	public static Globe CreateNew( IGlobeGenerator generator ) {
		ReadOnlyGlobe readOnlyGlobe = ReadOnlyGlobeStore.Get( generator.Subdivisions );
		Globe globe = new( Guid.NewGuid(), readOnlyGlobe, generator.Radius );
		generator.GenerateInitialGlobeState( globe );
		return globe;
	}
}
