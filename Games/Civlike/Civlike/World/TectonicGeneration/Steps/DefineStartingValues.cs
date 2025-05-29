using Civlike.World.GenerationState;
using Civlike.World.NoiseProviders;
using Civlike.World.TectonicGeneration.Parameters.Old;
using Engine;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<DefineGradientStep>]
[Engine.Processing.Do<IGenerationStep>.After<SetDistancesFromLand>]
[Engine.Processing.Do<IGenerationStep>.After<SetHadleyCells>]
[Engine.Processing.Do<IGenerationStep>.Before<SimulationStep>]
public sealed class DefineStartingValues : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Defining starting values per tile";

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		var adp = globe.AtmosphericDynamicsParameters;

		float Ω = 2f * float.Pi / (float) globe.PlanetaryConstants.RotationPeriod;
		float dt = (float) parameters.GenerationParameters.SimulationTimeStepSeconds;

		float linearDragCoefficient = (float) adp.LinearFrictionCoefficient;
		float quadraticDragCoefficient = (float) adp.QuadraticFrictionCoefficient;
		float coriolisStrength = (float) adp.CoriolisStrength;
		float pressureGradientCoefficient = -(float) adp.PressureGradientCoefficient;
		Vector3 upAxis = adp.UpAxis;

		Noise3 soilCoarseNoise = new( globe.SeedProvider.Next(), 18 );
		Noise3 soilFineNoise = new( globe.SeedProvider.Next(), 79 );
		Noise3 porosityCoarseNoise = new( globe.SeedProvider.Next(), 12 );
		Noise3 porosityFineNoise = new( globe.SeedProvider.Next(), 88 );

		List<TectonicFaceState> states = globe.Faces.OfType<Face<TectonicFaceState>>().Select( f => f.State ).ToList();
		double sigmaMin = states.Min( f => f.BaselineValues.ElevationStandardDeviation );
		double sigmaMax = states.Max( f => f.BaselineValues.ElevationStandardDeviation );
		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;
				Vector3<float> center = face.Center.FromNumerics<float>();

				double latitude = double.RadiansToDegrees( Math.Asin( center.Y ) );
				double latitudeFraction = latitude / 90;
				float soilNoise = soilCoarseNoise.Noise( center ) * 0.65f + soilFineNoise.Noise( center ) * 0.35f;
				state.SoilDepth = PhysicsHelpers.GetSoilDepth( globe, state, latitude, soilNoise );
				float porosityNoise = porosityCoarseNoise.Noise( center ) * 0.65f + porosityFineNoise.Noise( center ) * 0.35f;
				state.SoilMoistureCapacity = PhysicsHelpers.GetSoilMoistureCapacity( globe, state, porosityNoise );

				state.SurfaceRoughness = PhysicsHelpers.GetSurfaceRoughness( globe, state, sigmaMin, sigmaMax );

				state.AirTemperature = PhysicsHelpers.GetTemperatureFromLatitude( globe, state, center );
				state.SurfaceTemperature = PhysicsHelpers.GetSurfaceTemperature( globe, center );
				state.SpecificHumidity = PhysicsHelpers.GetSpecificHumidity( globe, state );

				state.VegetationFraction = 0;
				state.SnowFraction = 0;

				state.RunoffAccumulation = 0;
				state.WaterDepth = face.IsOcean ? -face.State.BaselineValues.ElevationMean : 0f;
				state.RiverDischarge = 0;

				float f = 2 * Ω * center.Y;
				state.CoriolisFactor = coriolisStrength / float.CopySign( float.Max( float.Abs( f ), 1e-6f ), f );

				state.PressureElevationMean = state.BaselineValues.ElevationMean + state.WaterDepth;
				state.CachedLapseRate = PhysicsHelpers.GetLapseRate( globe, state );
				state.Pressure = PhysicsHelpers.GetPressure( globe, state );
				state.CombinedPressure = state.Pressure + state.SeaPressure;

				if (!face.IsOcean)
					continue;

				state.SeaSalinity = (float) globe.SeaLandAirConstants.OceanSalinityReference;
				state.OceanCurrent = Vector3.Zero;
			}
		} );

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;
				state.Wind = PhysicsHelpers.GetWindVector( face, state, pressureGradientCoefficient, upAxis, linearDragCoefficient, quadraticDragCoefficient );
			}
		} );
	}
}
