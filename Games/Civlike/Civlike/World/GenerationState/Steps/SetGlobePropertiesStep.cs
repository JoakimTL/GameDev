namespace Civlike.World.GenerationState.Steps;

[Engine.Processing.Do<IGenerationStep>.After<CreateFacesStep>]
public sealed class SetGlobePropertiesStep : GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> {
	public override string StepDisplayName => "Setting globe properties";

	public override void Process( GeneratingGlobeBase globe, GlobeGeneratorParameterBase parameters ) {
		if (globe.Faces is null)
			throw new InvalidOperationException( "Faces are null." );
		globe.SetRadius( parameters.GlobeRadius );
		globe.SetTileArea( globe.Area / globe.Faces.Count );

		// 1) golden ratio and base‐edge angle (radians):
		double phi = (1 + Math.Sqrt( 5 )) / 2;
		double theta0 = Math.Acos( 1 / phi );

		// 2) angle per subdivided segment:
		double thetaSeg = theta0 / Math.Pow( 2, parameters.Subdivisions );

		// 3) chord length of that segment:
		double tileLength = 2 * parameters.GlobeRadius * Math.Sin( thetaSeg * 0.5 );
		globe.SetTileLength( tileLength );
	}
}
