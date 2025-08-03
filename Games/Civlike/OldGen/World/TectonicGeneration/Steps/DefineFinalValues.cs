//using OldGen.World.GenerationState;
//using OldGen.World.TerrainTypes;

//namespace OldGen.World.TectonicGeneration.Steps;

//[Engine.Processing.Do<IGenerationStep>.After<SimulationStep>]
//public sealed class DefineFinalValues : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	public override string StepDisplayName => "Defining final values per tile";

//	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
//		var adp = globe.AtmosphericDynamicsParameters;

//		float daysSimulated = (float) (parameters.GenerationParameters.SpinUpDurationSeconds / parameters.GenerationParameters.SimulationTimeStepSeconds);

//		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
//				TectonicFaceState state = face.State;

//				state.AverageAirTemperature /= daysSimulated;
//				state.AverageSurfaceTemperature /= daysSimulated;
//			}
//		} );
//	}
//}

//[Engine.Processing.Do<IGenerationStep>.After<DefineFinalValues>]
//public sealed class AssignTerrainTypes : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	public override string StepDisplayName => "Assigning terrain types";

//	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
//		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
//				TectonicFaceState state = face.State;

//				bool isLand = state.BaselineValues.ElevationMean > 0;
//				if ( isLand )
//					face.TerrainType = TerrainTypeList.GetTerrainType<GrasslandTerrain>();
//			}
//		} );
//	}
//}
