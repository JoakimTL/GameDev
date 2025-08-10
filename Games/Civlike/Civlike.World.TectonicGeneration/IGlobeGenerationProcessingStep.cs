using Civlike.World.State;

namespace Civlike.World.TectonicGeneration;

public interface IGlobeGenerationProcessingStep {
	void Process( Globe globe );
}
