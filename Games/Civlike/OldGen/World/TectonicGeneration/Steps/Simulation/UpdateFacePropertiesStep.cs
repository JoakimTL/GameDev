//using OldGen.World.GenerationState;
//using OldGen.World.TectonicGeneration.Parameters;
//using System.Runtime.CompilerServices;

//namespace OldGen.World.TectonicGeneration.Steps.Simulation;

//public sealed class UpdateFacePropertiesStep : ISimulationStep {
//	[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
//		PhysicalProperties snowProperties = globe.EndMemberProperties.Snow;
//		PhysicalProperties drySoilProperties = globe.EndMemberProperties.DrySoil;
//		PhysicalProperties vegetationProperties = globe.EndMemberProperties.Vegetation;
//		PhysicalProperties waterProperties = globe.EndMemberProperties.Water;
//		PhysicalProperties iceProperties = globe.EndMemberProperties.Ice;

//		float snowEmissivity = (float) snowProperties.Emissivity;
//		float drySoilEmissivity = (float) drySoilProperties.Emissivity;
//		//float iceEmissivity = (float) iceProperties.Emissivity; //TODO: Use ice
//		float waterEmissivity = (float) waterProperties.Emissivity;

//		float drySoilCp = (float) drySoilProperties.ThermalCapacity;
//		float waterCp = (float) waterProperties.ThermalCapacity;

//		float drySoilK = (float) drySoilProperties.ThermalConductivity;
//		float waterK = (float) waterProperties.ThermalConductivity;

//		float landDepth = (float) globe.SeaLandAirConstants.SurfaceThermalLayerDepth;
//		float calculationWaterDepth = (float) globe.SeaLandAirConstants.OceanMixedLayerDepth;

//		float snowAlbedo = (float) snowProperties.Albedo;
//		float vegetationAlbedo = (float) vegetationProperties.Albedo;
//		float bareGroundAlbedo = (float) drySoilProperties.Albedo;
//		float waterAlbedo = (float) waterProperties.Albedo;

//		float drySoilDensity = (float) drySoilProperties.Density;
//		var faces = globe.TectonicFaces;

//		ParallelProcessing.Range( faces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = faces[ i ];
//				TectonicFaceState state = face.State;

//				float faceSnowEmissivity = state.SnowFraction * snowEmissivity;
//				float faceSnowAlbedo = state.SnowFraction * snowAlbedo;

//				float finalAlbedo;
//				float finalEmissivity;
//				float finalThermalCapacityPerVolume;
//				float finalThermalCapacityPerArea;
//				float finalThermalConductivity;

//				float reverseSnowFraction = 1 - state.SnowFraction;

//				if (face.IsLand) {
//					//Land albedo parts
//					float faceVegetationAlbedo = state.VegetationFraction * vegetationAlbedo;
//					float faceBareGroundAlbedo = (1 - state.VegetationFraction) * bareGroundAlbedo;

//					float moistureFrac = state.SoilMoisture / state.SoilMoistureCapacity;
//					float reverseMoistureFrac = 1 - moistureFrac;

//					//Set values
//					finalAlbedo = faceSnowAlbedo + reverseSnowFraction * (faceVegetationAlbedo + faceBareGroundAlbedo);
//					finalEmissivity = faceSnowEmissivity + reverseSnowFraction * drySoilEmissivity;
//					finalThermalCapacityPerVolume = (reverseMoistureFrac * drySoilCp + moistureFrac * waterCp) * drySoilDensity;
//					finalThermalCapacityPerArea = finalThermalCapacityPerVolume * landDepth;
//					finalThermalConductivity = reverseMoistureFrac * drySoilK + moistureFrac * waterK;
//				} else {
//					//Water depth
//					float depth = state.WaterDepth;
//					float effectiveDepth = MathF.Min( MathF.Max( depth, 1 ), calculationWaterDepth );

//					//Set values
//					finalAlbedo = faceSnowAlbedo + reverseSnowFraction * waterAlbedo;
//					finalEmissivity = faceSnowEmissivity + reverseSnowFraction * waterEmissivity;
//					finalThermalCapacityPerVolume = waterCp;
//					finalThermalCapacityPerArea = finalThermalCapacityPerVolume * effectiveDepth;
//					finalThermalConductivity = waterK;
//				}

//				state.Albedo = finalAlbedo;
//				state.Emissivity = finalEmissivity;
//				state.ThermalCapacityPerVolume = finalThermalCapacityPerVolume;
//				state.ThermalCapacityPerArea = finalThermalCapacityPerArea;
//				state.ThermalConductivity = finalThermalConductivity;

//				if (face.IsOcean)
//					continue;

//				state.PressureElevationMean = state.BaselineValues.ElevationMean + state.WaterDepth;
//				state.Pressure = PhysicsHelpers.GetPressure( globe, state );
//				state.CombinedPressure = state.Pressure + state.SeaPressure;
//			}
//		} );
//	}
//}
