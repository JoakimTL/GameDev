using OldGen.World.GenerationState;
using OldGen.World.TectonicGeneration.Parameters;

namespace OldGen.World.TectonicGeneration;

public sealed class TectonicGlobeParameters( uint subdivisions, double globeRadius, int seed, int playerCount, TectonicParameters tectonicParameters ) : GlobeGeneratorParameterBase( subdivisions, globeRadius, seed, playerCount ) {
	public TectonicParameters TectonicParameters { get; } = tectonicParameters;
	public GenerationParameters GenerationParameters { get; } = new GenerationParameters();
}
