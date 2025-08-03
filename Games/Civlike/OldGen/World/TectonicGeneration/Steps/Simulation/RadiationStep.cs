//using OldGen.World.GenerationState;
//using System.Runtime.CompilerServices;

//namespace OldGen.World.TectonicGeneration.Steps.Simulation;

//public sealed class RadiationStep : ISimulationStep {

//	[MethodImpl( MethodImplOptions.AggressiveOptimization )]
//	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
//		InsolationProvider insolationProvider = globe.InsolationProvider;
//		IReadOnlyList<Face<TectonicFaceState>> faces = globe.TectonicFaces;
//		insolationProvider.Preprocess( daysSimulated );
//		float σ = (float) globe.UniversalConstants.StefanBoltzmannConstant;
//		float α_sw = (float) globe.RadiationParameters.AtmosphericShortwaveAbsorption;
//		float α_lw = (float) globe.RadiationParameters.AtmosphericLongwaveAbsorption;
//		float f_down = 0.5f;
//		float dt = (float) secondsToSimulate;
//		float cpl = (float) globe.RadiationParameters.AirSeaCouplingCoefficient;

//		ParallelProcessing.Range( faces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = faces[ i ];
//				TectonicFaceState state = face.State;

//				float I = insolationProvider.GetDailyMeanInsolation( face );
//				float A = state.Albedo;

//				float I_atm = α_sw * I;
//				float I_surf = (1f - α_sw) * I;
//				float Q_sw = I_surf * (1f - A) + f_down * I_atm;

//				float T0 = state.SurfaceTemperature;
//				float ε = state.Emissivity;
//				float C = state.ThermalCapacityPerArea;

//				float effε = ε * (1 - α_lw);

//				float newSurfaceTemperature = LinearizedStep( T0, Q_sw, effε, σ, C, dt );

//				state.SurfaceTemperature = newSurfaceTemperature;
//				state.AverageSurfaceTemperature += newSurfaceTemperature;

//				float Ta0 = state.AirTemperature;
//				float Ta1 = Ta0 + (newSurfaceTemperature - Ta0) * cpl;
//				state.AirTemperature = Ta1;
//				state.AverageAirTemperature += Ta1;
//			}
//		} );
//	}

//	[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
//	private static float LinearizedStep( float T0, float Q_sw, float effε, float σ, float C, float dt ) {
//		float T02 = T0 * T0, T03 = T02 * T0, T04 = T02 * T02;
//		float b1 = 4f * effε * σ * T03;
//		float b0 = -3f * effε * σ * T04;
//		float a = (Q_sw - b0) / C;
//		float k = b1 / C;
//		float Teq = a / k;
//		float expTerm = MathF.Exp( -k * dt );
//		return Teq + (T0 - Teq) * expTerm;
//	}
//}