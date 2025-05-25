using Civlike.World.GenerationState;
using System.Numerics;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

public sealed class RadiationStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
		globe.InsolationProvider.Preprocess( daysSimulated );

		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			float σ = (float) globe.UniversalConstants.StefanBoltzmannConstant;
			float α_sw = (float) globe.RadiativeAndCloudParameters.AtmosphericShortwaveAbsorption;
			float α_lw = (float) globe.RadiativeAndCloudParameters.AtmosphericLongwaveAbsorption;
			float f_downsw = 0.5f;
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.Faces[ i ] as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Face at index {i} is not of type TectonicFaceState." );
				TectonicFaceState state = face.State;

				float T0 = state.Temperature;
				float I = globe.InsolationProvider.GetDailyMeanInsolation( face );
				float A = state.Albedo;
				float ε = state.Emissivity;
				float C = state.ThermalCapacity * (float) globe.PlanetaryParameters.SurfaceThermalLayerDepth;

				float dt = (float) secondsToSimulate;

				float I_atm = α_sw * I;
				float I_surf = (1f - α_sw) * I;

				float Q_sw_net = I_surf * (1f - A) + f_downsw * I_atm;

				float effε = ε * (1 - α_lw);

				float T0_2 = T0 * T0;
				float T0_3 = T0_2 * T0;
				float T0_4 = T0_2 * T0_2;
				float b1 = 4 * effε * σ * T0_3;
				float b0 = -3 * effε * σ * T0_4;

				float a = (Q_sw_net - b0) / C;
				float k = b1 / C;

				float Teq = a / k;
				float expTerm = MathF.Exp( -k * dt );
				float T_new = Teq + (T0 - Teq) * expTerm;

				state.Temperature = T_new;
				state.AverageTemperature += (T_new - state.AverageTemperature) / ((float) daysSimulated + 1);
			}
		} );
	}
}
