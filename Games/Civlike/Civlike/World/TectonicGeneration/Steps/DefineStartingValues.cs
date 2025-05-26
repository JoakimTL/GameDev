using Civlike.World.GenerationState;
using Civlike.World.NoiseProviders;
using Civlike.World.TectonicGeneration.Parameters.Old;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<DefineGradientStep>]
public sealed class DefineStartingValues : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Defining starting values per tile";

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		Noise3 soilCoarseNoise = new( globe.SeedProvider.Next(), 18 );
		Noise3 soilFineNoise = new( globe.SeedProvider.Next(), 79 );
		Noise3 porosityCoarseNoise = new( globe.SeedProvider.Next(), 12 );
		Noise3 porosityFineNoise = new( globe.SeedProvider.Next(), 88 );

		List<TectonicFaceState> states = globe.Faces.OfType<Face<TectonicFaceState>>().Select( f => f.State ).ToList();
		double sigmaMin = states.Min( f => f.BaselineValues.ElevationStandardDeviation );
		double sigmaMax = states.Max( f => f.BaselineValues.ElevationStandardDeviation );
		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.Faces[ i ] as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Face at index {i} is not of type TectonicFaceState." );
				TectonicFaceState state = face.State;
				Vector3<float> center = face.Center;

				double latitude = double.RadiansToDegrees( Math.Asin( center.Y ) );
				double latitudeFraction = latitude / 90;
				state.SoilDepth = GetSoilDepth( globe, state, center, latitude, soilCoarseNoise, soilFineNoise );
				state.SoilMoistureCapacity = GetSoilMoistureCapacity( globe, state, center, porosityCoarseNoise, porosityFineNoise );

				state.SurfaceRoughness = GetSurfaceRoughness( globe, state, center, sigmaMin, sigmaMax );

				state.AirTemperature = GetTemperatureFromLatitude( globe, state, center );
				state.SurfaceTemperature = GetSurfaceTemperature( globe, center );
				state.Pressure = GetPressure( globe, state );
				state.SpecificHumidity = GetSpecificHumidity( globe, state, center, latitude );

				state.VegetationFraction = 0;
				state.SnowFraction = 0;

				state.RunoffAccumulation = 0;
				state.FreshwaterDepth = 0;
				state.RiverDischarge = 0;

				if (!face.IsOcean)
					continue;

				state.SeaSalinity = (float) globe.SeaLandAirConstants.OceanSalinityReference;
				state.OceanCurrent = 0;

			}
		} );
	}

	private float GetSurfaceTemperature( TectonicGeneratingGlobe globe, Vector3<float> center ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		double absoluteLatitudeFraction = Math.Abs( center.Y );

		return (float) (initializationParameters.EquatorialSeaSurfaceTemperature - initializationParameters.PolarSeaSurfaceTemperatureReduction * absoluteLatitudeFraction);
	}

	private float GetSpecificHumidity( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, double latitude ) {
		Parameters.UniversalConstants universalConstants = globe.UniversalConstants;
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		Parameters.SeaLandAirConstants seaLandAirConstants = globe.SeaLandAirConstants;

		double invT0 = 1.0 / initializationParameters.EquatorialAirTemperature;
		double invT = 1.0 / state.AirTemperature;
		double esat = seaLandAirConstants.ReferenceSaturationVaporPressure * Math.Exp( universalConstants.ClausiusClapeyronExponent * (invT0 - invT) );
		return (float) (initializationParameters.InitialRelativeHumidity * seaLandAirConstants.MolecularWeightRatioVaporDryAir * esat / (state.Pressure - esat));
	}

	private Pressure GetPressure( TectonicGeneratingGlobe globe, TectonicFaceState state ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		Parameters.SeaLandAirConstants seaLandAirConstants = globe.SeaLandAirConstants;

		double exponent = globe.UniversalConstants.SpecificHeatCapacity * seaLandAirConstants.DryAirMolarMass / globe.UniversalConstants.UniversalGasConstant;
		double @exponentBase = 1 - globe.PlanetaryConstants.Gravity * state.ElevationMeanAboveSea / (globe.UniversalConstants.UniversalGasConstant * state.AirTemperature.Kelvin);

		double barometricExponent = Math.Pow( @exponentBase, exponent );

		return seaLandAirConstants.SeaLevelPressure * float.Exp( -(float) (globe.PlanetaryConstants.Gravity * state.ElevationMeanAboveSea * seaLandAirConstants.DryAirMolarMass / (state.AirTemperature.Kelvin * globe.UniversalConstants.UniversalGasConstant)) );
	}

	private Temperature GetTemperatureFromLatitude( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		double absoluteLatitudeFraction = Math.Abs( center.Y );

		return initializationParameters.EquatorialAirTemperature
			- initializationParameters.PolarAirTemperatureReduction * absoluteLatitudeFraction
			- initializationParameters.LapseRate * state.ElevationMeanAboveSea;
	}

	private float GetSurfaceRoughness( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, double sigmaMin, double sigmaMax ) {
		double bareGroundMinimum = globe.EndMemberProperties.DrySoil.MinimumRoughnessLength;
		double bareGroundMaximum = globe.EndMemberProperties.DrySoil.MaximumRoughnessLength;

		double σ = state.BaselineValues.ElevationStandardDeviation;
		double normSigma = (σ - sigmaMin) / (sigmaMax - sigmaMin);
		normSigma = Math.Clamp( normSigma, 0.0, 1.0 );
		return (float) (bareGroundMinimum + (bareGroundMaximum - bareGroundMinimum) * normSigma);
	}

	private float GetSoilDepth( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, double latitude, Noise3 soilCoarseNoise, Noise3 soilFineNoise ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		Parameters.SeaLandAirConstants seaLandAirConstants = globe.SeaLandAirConstants;

		double S = state.BaselineValues.Gradient.Magnitude<Vector3<float>, float>();
		double baseDepth = initializationParameters.SoilDepthBase * Math.Exp( -seaLandAirConstants.SlopeDecayConstant * S );
		double latNorm = Math.Abs( latitude ) / 90.0;
		double climateFactor = 1.0 - 0.5 * latNorm;

		double soilNoise = soilCoarseNoise.Noise( center ) * 0.65 + soilFineNoise.Noise( center ) * 0.35;
		soilNoise = soilNoise * 2 - 1;
		baseDepth *= 1 + initializationParameters.SoilNoiseAmplitude * soilNoise;

		return (float) (baseDepth * climateFactor);
	}

	private float GetSoilMoistureCapacity( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, Noise3 porosityCoarseNoise, Noise3 porosityFineNoise ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;

		double porosityNoise = porosityCoarseNoise.Noise( center ) * 0.65 + porosityFineNoise.Noise( center ) * 0.35;
		porosityNoise = porosityNoise * 2 - 1;

		float porosity = (float) (initializationParameters.SoilPorosityBase * (1 + initializationParameters.SoilPorosityNoiseAmplitude * porosityNoise));

		return state.SoilDepth * porosity * 1000.0f;
	}
}
