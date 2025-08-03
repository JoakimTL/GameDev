//using OldGen.World.GenerationState;
//using OldGen.World.TectonicGeneration.Steps.Simulation;

//namespace OldGen.World.TectonicGeneration.Steps;

//[Engine.Processing.Do<IGenerationStep>.After<DefineGradientStep>]
//public sealed class SimulationStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	//private readonly ISimulationStep[] _internalSteps;
//	private double _daysSimulated = 0;

//	private readonly UpdateFacePropertiesStep _updateFaceProperties;
//	private readonly RadiationStep _radiationStep;
//	private readonly AtmosphericDynamicsStep _atmosphericDynamicsStep;
//	private readonly EvaporationStep _evaporationStep;
//	private readonly PrecipitationStep _precipitationStep;
//	private readonly SnowMeltStep _snowMeltStep;
//	private readonly HydrologyRoutingStep _hydrologyRoutingStep;

//	public SimulationStep() {
//		this._updateFaceProperties = new();
//		this._radiationStep = new();
//		this._atmosphericDynamicsStep = new();
//		this._evaporationStep = new();
//		this._precipitationStep = new();
//		this._snowMeltStep = new();
//		this._hydrologyRoutingStep = new();

//		//_internalSteps = [
//		//	new UpdateFacePropertiesStep(),
//		//	new RadiationStep(),
//		//	new AtmosphericDynamicsStep(),
//		//	//new AdvectionStep(),
//		//	//new EvaporationStep(),
//		//	//new PrecipitationStep(),
//		//	//new SnowMeltStep(),
//		//	//new HydrologyRoutingStep(),
//		//];
//	}

//	public override string StepDisplayName => "Simulation";

//	public override int GetLoopCount( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters )
//		=> (int) Math.Ceiling( parameters.GenerationParameters.SpinUpDurationSeconds / parameters.GenerationParameters.SimulationTimeStepSeconds );

//	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
//		double secondsToSimulate = parameters.GenerationParameters.SimulationTimeStepSeconds;
//		double daysToSimulate = globe.PlanetaryConstants.RotationPeriod / secondsToSimulate;
//		this._updateFaceProperties.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		this._radiationStep.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		this._atmosphericDynamicsStep.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		this._evaporationStep.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		this._precipitationStep.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		this._snowMeltStep.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		this._hydrologyRoutingStep.Process( globe, parameters, this._daysSimulated, secondsToSimulate );
//		//for (int i = 0; i < _internalSteps.Length; i++) {
//		//	_internalSteps[ i ].Process( globe, parameters, _daysSimulated, secondsToSimulate );
//		//}
//		this._daysSimulated += daysToSimulate;
//	}
//}
