using Civlike.World.GenerationState;
using Engine;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;

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
		float calculationWaterDepth = (float) globe.SeaLandAirConstants.OceanMixedLayerDepth;

		float snowAlbedo = (float) snowProperties.Albedo;
		float vegetationAlbedo = (float) vegetationProperties.Albedo;
		float bareGroundAlbedo = (float) drySoilProperties.Albedo;
		float waterAlbedo = (float) waterProperties.Albedo;

		float drySoilDensity = (float) drySoilProperties.Density;
		float dayOfYear = (float) daysSimulated % (float) globe.PlanetaryConstants.OrbitPeriod; // Ensure dayOfYear is within 0-364 range
		float season = 2 * MathF.PI * (dayOfYear / (float) globe.PlanetaryConstants.OrbitPeriod); // Convert to radians
		float seasonCos = float.Cos( season );

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				float faceSnowEmissivity = state.SnowFraction * snowEmissivity;
				float faceSnowAlbedo = state.SnowFraction * snowAlbedo;

				float finalAlbedo;
				float finalEmissivity;
				float finalThermalCapacityPerVolume;
				float finalThermalCapacityPerArea;
				float finalThermalConductivity;

				float reverseSnowFraction = 1 - state.SnowFraction;

				if (face.IsLand) {
					//Land albedo parts
					float faceVegetationAlbedo = state.VegetationFraction * vegetationAlbedo;
					float faceBareGroundAlbedo = (1 - state.VegetationFraction) * bareGroundAlbedo;

					float moistureFrac = state.SoilMoisture / state.SoilMoistureCapacity;
					float reverseMoistureFrac = 1 - moistureFrac;

					//Set values
					finalAlbedo = faceSnowAlbedo + reverseSnowFraction * (faceVegetationAlbedo + faceBareGroundAlbedo);
					finalEmissivity = faceSnowEmissivity + reverseSnowFraction * drySoilEmissivity;
					finalThermalCapacityPerVolume = (reverseMoistureFrac * drySoilCp + moistureFrac * waterCp) * drySoilDensity;
					finalThermalCapacityPerArea = finalThermalCapacityPerVolume * landDepth;
					finalThermalConductivity = reverseMoistureFrac * drySoilK + moistureFrac * waterK;
				} else {
					//Water depth
					float depth = state.WaterDepth;
					float effectiveDepth = float.Clamp( depth, 1, calculationWaterDepth );

					//Set values
					finalAlbedo = faceSnowAlbedo + reverseSnowFraction * waterAlbedo;
					finalEmissivity = faceSnowEmissivity + reverseSnowFraction * waterEmissivity;
					finalThermalCapacityPerVolume = waterCp;
					finalThermalCapacityPerArea = finalThermalCapacityPerVolume * effectiveDepth;
					finalThermalConductivity = waterK;
				}

				state.Albedo = finalAlbedo;
				state.Emissivity = finalEmissivity;
				state.ThermalCapacityPerVolume = finalThermalCapacityPerVolume;
				state.ThermalCapacityPerArea = finalThermalCapacityPerArea;
				state.ThermalConductivity = finalThermalConductivity;

				if (face.IsOcean)
					continue;

				state.PressureElevationMean = state.BaselineValues.ElevationMean + state.WaterDepth;
				state.Pressure = PhysicsHelpers.GetPressure( globe, state );
				state.CombinedPressure = state.Pressure + state.SeaPressure;
			}
		} );

		//ParallelProcessing.Range( globe.NonOceanTectonicFacesByElevationMean.Count, ( start, end, taskId ) => {
		//	for (int i = start; i < end; i++) {
		//		Face<TectonicFaceState> face = globe.NonOceanTectonicFacesByElevationMean[ i ];
		//		TectonicFaceState state = face.State;

		//		state.PressureElevationMean = state.BaselineValues.ElevationMean + state.WaterDepth;
		//		state.Pressure = PhysicsHelpers.GetPressure( globe, state );
		//		state.CombinedPressure = state.Pressure + state.SeaPressure;
		//	}
		//} );
	}

	//private Pressure GetEffectivePressure( TectonicGeneratingGlobe globe, Face<TectonicFaceState> face, TectonicFaceState state, Vector3<float> center, float seasonCos, float pBaro ) {
	//	Parameters.UniversalConstants uC = globe.UniversalConstants;
	//	Parameters.PlanetaryConstants pC = globe.PlanetaryConstants;
	//	Parameters.SeaLandAirConstants slaC = globe.SeaLandAirConstants;
	//	Parameters.AtmosphericDynamicsParameters adp = globe.AtmosphericDynamicsParameters;

	//	float sinφ = center.Y;
	//	float lat = face.LatitudeRads;

	//	float φ0 = adp.TropicalBeltWidthRad;
	//	float φ1 = adp.SubTropicalBeltWidthRad;
	//	float φs = adp.BaseSubTropicalLatRad;
	//	float P_trop = adp.TropicalBeltAmplitude;
	//	float P_sub = adp.SubTropicalBeltAmplitude;

	//	// seasonal shift ±Δφ over the year
	//	float Δφ = adp.SeasonalBeltShiftRad;
	//	float centerN = φs + Δφ * seasonCos;
	//	float centerS = -φs - Δφ * seasonCos;

	//	float beltEq = P_trop * MathF.Exp( -(lat / φ0) * (lat / φ0) );
	//	float beltN = P_sub * MathF.Exp( -((lat - centerN) / φ1) * ((lat - centerN) / φ1) );
	//	float beltS = P_sub * MathF.Exp( -((lat - centerS) / φ1) * ((lat - centerS) / φ1) );

	//	float pBelt = beltEq - beltN - beltS;

	//	// 3) thermal land-sea anomaly (daily-mean)
	//	float pTherm = 0f;
	//	if (face.IsLand) {
	//		// ΔT_landSea, layerHeight from your parameters
	//		float dT = adp.MeanLandSeaDeltaT;
	//		float h = adp.LayerHeight;
	//		float T0 = state.AirTemperature.Kelvin;
	//		float g = (float) pC.Gravity;
	//		float M = (float) slaC.DryAirMolarMass; 
	//		float R = (float) uC.UniversalGasConstant;

	//		pTherm = pBaro * M * g * h * dT / (R * T0 * T0);
	//	}

	//	// 4) seasonal (annual) pressure tide
	//	float pSeason = adp.SeasonalAmplitude * seasonCos;// e.g. ±5 Pa

	//	float ΔpBg = adp.BackgroundAmplitude;    // e.g. 800 Pa
	//	float pLegendre = 0.5f * (3f * sinφ * sinφ - 1f);
	//	float pBg = ΔpBg * pLegendre;

	//	// 5) sum up & store
	//	float pEffDelta = pBelt + pTherm + pSeason + pBg;

	//	return pBaro + pEffDelta;
	//}
}
