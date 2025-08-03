//namespace OldGen.World.TectonicGeneration.Steps.Simulation;

//public sealed class EvaporationStep : ISimulationStep {
//	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
//		float dt = (float) secondsToSimulate;
//		float invT0 = 1f / Temperature.ZeroCelsius;
//		float Rinv = 1f / (float) globe.UniversalConstants.SpecificGasConstant;
//		float cwRat = (float) globe.SeaLandAirConstants.MolecularWeightRatioVaporDryAir;
//		float ccExp = (float) globe.UniversalConstants.ClausiusClapeyronExponent;
//		float refEs = (float) globe.SeaLandAirConstants.ReferenceSaturationVaporPressure;
//		float layerH = (float) globe.EvaporationParameters.BoundaryLayerHeight;
//		float landC = (float) globe.EvaporationParameters.LandBulkTransferCoefficient;
//		float waterC = (float) globe.EvaporationParameters.WaterBulkTransferCoefficient;

//		var faces = globe.TectonicFaces;  // local alias

//		ParallelProcessing.Range( faces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				var face = faces[ i ];
//				var state = face.State;

//				// -- Thermodynamics
//				float T = state.AirTemperature;
//				float P = state.Pressure;
//				float rho = P * Rinv / T;

//				// inline ComputeEsat
//				float invT = 1f / T;
//				float e_sat = refEs * MathF.Exp( ccExp * (invT0 - invT) );

//				float q_sat = cwRat * e_sat / (P - e_sat);

//				// -- Aerodynamics
//				float windSpeed = state.Wind.Length();
//				float cT = face.IsLand ? landC : waterC;

//				// evaporation (kg/m²/s -> mm/s)
//				float evapRate = cT * rho * windSpeed * (q_sat - state.SpecificHumidity);
//				evapRate = MathF.Max( 0f, evapRate );
//				float evap = evapRate * dt;

//				// soil moisture update if land
//				if (face.IsLand) {
//					evap = MathF.Min( evap, state.SoilMoisture );
//					state.SoilMoisture = MathF.Max( 0f, state.SoilMoisture - evap );
//				}

//				// humidity mixing
//				float dq = evap / (rho * layerH);
//				state.SpecificHumidity = MathF.Min( q_sat, state.SpecificHumidity + dq );

//				// write back densities & pressures
//				state.AirDensity = rho;
//				state.SaturationVaporPressure = e_sat;
//				state.SaturationSpecificHumidity = q_sat;
//			}
//		} );
//	}
//}
