using Engine.Generation.Meshing;

namespace Civlike.World.GenerationState.Steps;

public sealed class CreateIcosphereStep : GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> {
	public override string StepDisplayName => "Creating icosphere";

	public override void Process( GeneratingGlobeBase globe, GlobeGeneratorParameterBase parameters ) {
		Icosphere sphere = new( parameters.Subdivisions );
		globe.SetIcosphere( sphere );
	}
}
