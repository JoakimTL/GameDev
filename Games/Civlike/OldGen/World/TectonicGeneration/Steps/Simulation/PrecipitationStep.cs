//using OldGen.World.GenerationState;

//namespace OldGen.World.TectonicGeneration.Steps.Simulation;

//public sealed class PrecipitationStep : ISimulationStep {
//	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {

//		float dt = (float) secondsToSimulate;
//		float dtDays = dt / (float) globe.PlanetaryConstants.RotationPeriod;

//		float layerH = (float) globe.EvaporationParameters.BoundaryLayerHeight;

//		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
//				if (face.IsOcean)
//					continue;
//				TectonicFaceState state = face.State;

//				float rho = state.AirDensity;

//				float e_sat = state.SaturationVaporPressure;
//				float q_sat = state.SaturationSpecificHumidity;

//				float q_excess = state.SpecificHumidity - q_sat;
//				float precipitation_mm = 0;
//				if (q_excess > 0) {
//					precipitation_mm = q_excess * state.AirDensity * layerH;
//					state.SpecificHumidity = q_sat;
//				}

//				if (precipitation_mm <= 0)
//					continue;
//				float T_c = state.AirTemperature.Celsius;
//				float precipitationFraction = float.Clamp( 1 + T_c / 5, 0, 1 );
//				state.PrecipitationRate = precipitationFraction * precipitation_mm / dtDays;
//				state.SnowDepth += precipitationFraction * precipitation_mm / 1000;
//			}
//		} );
//	}
//}


