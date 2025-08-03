//using OldGen.World.GenerationState;

//namespace OldGen.World.TectonicGeneration.Steps.Simulation;

//public sealed class HydrologyRoutingStep : ISimulationStep {
//	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {

//		float cellArea = (float) globe.TileArea;
//		float dt = (float) secondsToSimulate;
//		float dtDays = dt / (float) globe.PlanetaryConstants.RotationPeriod; // Convert seconds to days for the simulation step.

//		float maxInfiltration = (float) globe.HydrologyParameters.MaxInfiltrationRate;
//		float maxInfiltrationDay = maxInfiltration * dtDays;
//		float runoffCoefficient = (float) globe.HydrologyParameters.RunoffCoefficient;
//		float streamAreaThreshold = (float) globe.HydrologyParameters.StreamInitiationArea; // This is in m², so we can use it directly.
//		float manningN = (float) globe.HydrologyParameters.ManningN; // Manning's roughness coefficient, used for calculating flow depth.

//		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
//				if (face.IsOcean)
//					continue;
//				TectonicFaceState state = face.State;
//				state.RunoffAccumulation = 0;
//				state.RiverDischarge = 0;

//				//2) Need to define PrecipitationRate and SnowMeltRate somewhere in the simulation loop.
//				float precipitation = state.PrecipitationRate * dtDays;
//				float melt = state.SnowMeltRate * dtDays;
//				float input = precipitation + melt;

//				float infil = float.Min( input, maxInfiltrationDay );
//				state.SoilMoisture = float.Min(
//					state.SoilMoisture + infil,
//					state.SoilMoistureCapacity
//				);

//				float runoffDepthM = (input - infil) * runoffCoefficient / 1000;
//				state.RunoffAccumulation = runoffDepthM * cellArea / dt;
//				state.RunoffAccumulatedArea = cellArea;
//			}
//		} );

//		for (int i = 0; i < globe.NonOceanTectonicFacesByElevationMean.Count; i++) {
//			Face<TectonicFaceState> face = globe.NonOceanTectonicFacesByElevationMean[ i ];
//			TectonicFaceState state = face.State;

//			float Q = state.RunoffAccumulation;

//			if (state.DownslopeNeighbour is not null) {
//				state.DownslopeNeighbour.State.RunoffAccumulation += Q;
//				state.DownslopeNeighbour.State.RunoffAccumulatedArea += state.RunoffAccumulatedArea;
//			}

//			if (state.RunoffAccumulatedArea < streamAreaThreshold)
//				continue;

//			float h = MathF.Pow( Q * manningN / (state.DownslopeSlopeSqrt * (float) globe.TileLength), 3f/5f );
//			state.RiverDischarge = Q; //7) What units are we working with here?
//			state.ChannelDepth = h;
//		}
//	}
//}
