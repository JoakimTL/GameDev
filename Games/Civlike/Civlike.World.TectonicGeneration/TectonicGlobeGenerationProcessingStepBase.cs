using Civlike.World.State;

namespace Civlike.World.TectonicGeneration;

public abstract class TectonicGlobeGenerationProcessingStepBase( TectonicGenerationParameters parameters ) : IGlobeGenerationProcessingStep {
	protected TectonicGenerationParameters Parameters { get; } = parameters;
	public abstract void Process( Globe globe );
}
