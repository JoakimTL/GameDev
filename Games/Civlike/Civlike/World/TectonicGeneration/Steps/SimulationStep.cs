using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration.Steps.Simulation;
using Civlike.WorldOld;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<DefineGradientStep>]
public sealed class SimulationStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	private readonly ISimulationStep[] _internalSteps;
	private double _daysSimulated = 0;

	public SimulationStep() {
		_internalSteps = [
			new UpdateFacePropertiesStep(),
			new RadiationStep(),
			new AtmosphericDynamicsStep(),
			//new AdvectionStep(),
			//new EvaporationStep(),
			//new PrecipitationStep(),
			//new SnowMeltStep(),
			//new HydrologyRoutingStep(),
		];
	}

	public override string StepDisplayName => "Simulation";

	public override int GetLoopCount( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters )
		=> (int) Math.Ceiling( parameters.GenerationParameters.SpinUpDurationSeconds / parameters.GenerationParameters.SimulationTimeStepSeconds );

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		double secondsToSimulate = parameters.GenerationParameters.SimulationTimeStepSeconds;
		double daysToSimulate = globe.PlanetaryConstants.RotationPeriod / secondsToSimulate;
		for (int i = 0; i < _internalSteps.Length; i++) {
			_internalSteps[ i ].Process( globe, parameters, _daysSimulated, secondsToSimulate );
		}
		_daysSimulated += daysToSimulate;
	}
}
