using Civlike.World.GenerationState;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

public sealed class EvaporationStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.Faces[ i ] as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Face at index {i} is not of type TectonicFaceState." );
				TectonicFaceState state = face.State;

				float T = state.Temperature;
				float P = state.Pressure;

				//Pa = kg*m⁻¹*s⁻²
				//J = kg*m²*s⁻²
				//Pa/(J/(kg*K)*K) -> Pa / (J/kg) -> kg/(m*s²)/(J/kg) -> (kg²)/(m*s²*J) -> kg²/(m*s²*kg*m²*s⁻²) -> kg/m³
				//Air density at pressure and temperature
				float rho = P / ((float) globe.UniversalConstants.SpecificGasConstant * T);

				float e_sat = float.Min( ComputeEsat( globe, T ), 0.999f * P );
				float q_sat = (float) globe.UniversalConstants.MolecularWeightRatioVaporDryAir * e_sat / (P - e_sat);

				float windSpeed = state.Wind.Magnitude<Vector3<float>, float>();

				float bulkTransferCoefficient = (float) (face.IsLand ? globe.EvaporationParameters.LandBulkTransferCoefficient : globe.EvaporationParameters.WaterBulkTransferCoefficient);

				//kg/m³ * m/s -> kg/(m²*s) -> kg/(m²*s)
				float evaporationRate = bulkTransferCoefficient * rho * windSpeed * (q_sat - state.SpecificHumidity);
				evaporationRate = float.Max( 0, evaporationRate ); // Ensure non-negative evaporation rate

				//Since we're talking about evaporating water, then 1kg water = 1L.
				//1L / 1 m² = 0.001m, so we're talking in terms of mm.
				//mm/s * s -> mm
				float evaporation = evaporationRate * (float) secondsToSimulate;

				if (face.IsLand) {
					evaporation = float.Min( evaporation, state.SoilMoisture );
					state.SoilMoisture = float.Max( 0, state.SoilMoisture - evaporation );
				}

				float layerHeight = (float) globe.EvaporationParameters.BoundaryLayerHeight;
				// (kg/(m²s) * s) / (kg/m³ * m) -> (kg/m²)/(kg/m²)
				float dq = evaporation / (rho * layerHeight);
				state.SpecificHumidity = float.Min( q_sat, state.SpecificHumidity + dq );
			}
		} );
	}

	private float ComputeEsat( TectonicGeneratingGlobe globe, float t ) {
		const float invT0 = 1f / Temperature.ZeroCelsius;
		float invT = 1f / t;
		float exponent = (float) globe.UniversalConstants.ClausiusClapeyronExponent * (invT0 - invT);
		// Pa * e^(K/K) -> Pa * e^(0) -> Pa -> kg/(m*s²)
		return (float) globe.DynamicInitializationConstants.ReferenceSaturationVaporPressure * float.Exp( exponent );
	}
}

public sealed class AtmosphericDynamicsStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
		float omega = 2f * float.Pi / (float) globe.PlanetaryParameters.RotationPeriod; // Angular velocity in rad/s
		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.Faces[ i ] as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Face at index {i} is not of type TectonicFaceState." );
				TectonicFaceState state = face.State;


			}
		} );
	}
}
