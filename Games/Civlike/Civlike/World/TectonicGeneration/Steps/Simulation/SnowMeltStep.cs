using Civlike.World.GenerationState;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

public sealed class SnowMeltStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {

		float dt = (float) secondsToSimulate;
		float dtDays = dt / (float) globe.PlanetaryConstants.RotationPeriod;

		float snowMeltThreshold = (float) globe.SnowMeltParameters.SnowMeltTemperatureThreshold;
		float specificHeatAir = (float) globe.EndMemberProperties.Air.ThermalCapacity;
		float landBulkTransferCoefficient = (float) globe.EvaporationParameters.LandBulkTransferCoefficient;
		float snowInsulationCoefficient = (float) globe.SnowMeltParameters.SnowInsulationCoefficient;
		float latentHeatOfFusion = (float) globe.UniversalConstants.LatentHeatOfFusion;
		float thermalConductivitySnow = (float) globe.EndMemberProperties.Snow.ThermalConductivity;

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				if (state.SnowDepth <= 0)
					continue;

				float T_a = state.AirTemperature.Celsius;
				float T_s = state.SurfaceTemperature.Celsius;
				float windSpeed = state.TangentialWind.Magnitude<Vector3<float>, float>();

				float dT_a = T_a - snowMeltThreshold;
				float Q_H = state.AirDensity * specificHeatAir * landBulkTransferCoefficient * windSpeed * dT_a;

				float dT_s = T_s - snowMeltThreshold;
				float ks = thermalConductivitySnow / float.Max( state.SnowDepth, 0.01f );
				float Q_S = ks * dT_s * float.Exp( -snowInsulationCoefficient * state.SnowDepth );

				float Q_melt = float.Max(0, Q_H + Q_S);

				float meltDepthM = Q_melt * dt / (latentHeatOfFusion * 1000);

				meltDepthM = float.Min( meltDepthM, state.SnowDepth );
				state.SnowDepth -= meltDepthM;
				state.SnowMeltRate = meltDepthM * 1000 / dtDays;
			}
		} );
	}
}


