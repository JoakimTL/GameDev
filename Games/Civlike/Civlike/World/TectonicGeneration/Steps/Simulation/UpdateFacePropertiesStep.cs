using Civlike.World.GenerationState;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

public sealed class UpdateFacePropertiesStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
		Parameters.PhysicalProperties snowProperties = globe.EndMemberProperties.Snow;
		Parameters.PhysicalProperties drySoilProperties = globe.EndMemberProperties.DrySoil;
		Parameters.PhysicalProperties vegetationProperties = globe.EndMemberProperties.Vegetation;
		Parameters.PhysicalProperties waterProperties = globe.EndMemberProperties.Water;
		Parameters.PhysicalProperties iceProperties = globe.EndMemberProperties.Ice;

		float snowEmissivity = (float) snowProperties.Emissivity;
		float drySoilEmissivity = (float) drySoilProperties.Emissivity;
		float iceEmissivity = (float) iceProperties.Emissivity; //TODO: Use ice
		float waterEmissivity = (float) waterProperties.Emissivity;

		float drySoilCp = (float) drySoilProperties.ThermalCapacity;
		float waterCp = (float) waterProperties.ThermalCapacity;

		float drySoilK = (float) drySoilProperties.ThermalConductivity;
		float waterK = (float) waterProperties.ThermalConductivity;

		float landDepth = (float) globe.SeaLandAirConstants.SurfaceThermalLayerDepth;
		float waterDepth = (float) globe.SeaLandAirConstants.OceanMixedLayerDepth;

		float snowAlbedo = (float) snowProperties.Albedo;
		float vegetationAlbedo = (float) vegetationProperties.Albedo;
		float bareGroundAlbedo = (float) drySoilProperties.Albedo;
		float waterAlbedo = (float) waterProperties.Albedo;

		float drySoilDensity = (float) drySoilProperties.Density;

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				Engine.Vector3<float> center = face.Center;
				TectonicFaceState state = face.State;

				float faceSnowEmissivity = state.SnowFraction * snowEmissivity;
				float faceSnowAlbedo = state.SnowFraction * snowAlbedo;

				float finalAlbedo;
				float finalEmissivity;
				float finalThermalCapacityPerVolume;
				float finalThermalCapacityPerArea;
				float finalThermalConductivity;

				if (face.IsLand) {
					//Land albedo parts
					float faceVegetationAlbedo = state.VegetationFraction * vegetationAlbedo;
					float faceBareGroundAlbedo = (1 - state.VegetationFraction) * bareGroundAlbedo;

					float moistureFrac = state.SoilMoisture / state.SoilMoistureCapacity;

					//Set values
					finalAlbedo = faceSnowAlbedo + (1 - state.SnowFraction) * (faceVegetationAlbedo + faceBareGroundAlbedo);
					finalEmissivity = faceSnowEmissivity + (1 - state.SnowFraction) * drySoilEmissivity;
					finalThermalCapacityPerVolume = ((1 - moistureFrac) * drySoilCp + moistureFrac * waterCp) * drySoilDensity;
					finalThermalCapacityPerArea = finalThermalCapacityPerVolume * landDepth;
					finalThermalConductivity = (1 - moistureFrac) * drySoilK + moistureFrac * waterK;
				} else {
					//Water depth
					float depth = state.ElevationMeanAboveSea - state.BaselineValues.ElevationMean;
					float effectiveDepth = float.Clamp( depth, 1, waterDepth );

					//Set values
					finalAlbedo = faceSnowAlbedo + (1 - state.SnowFraction) * waterAlbedo;
					finalEmissivity = faceSnowEmissivity + (1 - state.SnowFraction) * waterEmissivity;
					finalThermalCapacityPerVolume = waterCp;
					finalThermalCapacityPerArea = finalThermalCapacityPerVolume * effectiveDepth;
					finalThermalConductivity = waterK;
				}

				state.Albedo = finalAlbedo;
				state.Emissivity = finalEmissivity;
				state.ThermalCapacityPerVolume = finalThermalCapacityPerVolume;
				state.ThermalCapacityPerArea = finalThermalCapacityPerArea;
				state.ThermalConductivity = finalThermalConductivity;

			}
		} );
	}
}
