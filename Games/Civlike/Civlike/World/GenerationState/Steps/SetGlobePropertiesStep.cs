namespace Civlike.World.GenerationState.Steps;

[Engine.Processing.Do<IGenerationStep>.After<CreateFacesStep>]
public sealed class SetGlobePropertiesStep : GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> {
	public override string StepDisplayName => "Setting globe properties";

	public override void Process( GeneratingGlobeBase globe, GlobeGeneratorParameterBase parameters ) {
		if (globe.Faces is null)
			throw new InvalidOperationException( "Faces are null." );
		globe.SetRadius( parameters.GlobeRadius );
		globe.SetTileArea( globe.Area / globe.Faces.Count );
		globe.SetApproximateTileLength( 2 * parameters.GlobeRadius * Math.Sin( Math.PI / (10 * Math.Pow( 2, parameters.Subdivisions )) ) );
	}
}
