using Civlike.World.GenerationState;
using Civlike.World.NoiseProviders;
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

		List<TectonicFaceState> states = globe.Faces.Select( f => f.Get<TectonicFaceState>() ).ToList();
		double sigmaMin = states.Min( f => f.BaselineValues.ElevationStandardDeviation );
		double sigmaMax = states.Max( f => f.BaselineValues.ElevationStandardDeviation );
		double barometricExponent = globe.PlanetaryParameters.Gravity / (globe.DynamicInitializationConstants.LapseRate * globe.UniversalConstants.UniversalGasConstant);
		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				Vector3<float> center = face.Center;
				TectonicFaceState state = face.Get<TectonicFaceState>();

				double latitude = double.RadiansToDegrees( Math.Asin( center.Y ) );
				double latitudeFraction = latitude / 90;
				state.SoilDepth = GetSoilDepth( globe, state, center, latitude, soilCoarseNoise, soilFineNoise );
				state.SoilMoistureCapacity = GetSoilMoistureCapacity( globe, state, center, porosityCoarseNoise, porosityFineNoise );

				state.SurfaceRoughness = GetSurfaceRoughness( globe, state, center, sigmaMin, sigmaMax );
				state.Albedo = 0.2f;
				state.ThermalCapacity = (float) globe.EndMemberThermalProperties.DrySoil.HeatCapacity;
				state.ThermalConductivity = (float) globe.EndMemberThermalProperties.DrySoil.ThermalConductivity;

				state.Emissivity = (float) (face.IsOcean ? globe.RadiativeAndCloudParameters.OceanEmissivity : globe.RadiativeAndCloudParameters.LandEmissivity);

				state.Temperature = GetTemperatureFromLatitude( globe, state, center );
				state.Pressure = GetPressure( globe, state );
				state.SpecificHumidity = GetSpecificHumidity( globe, state, center, latitude );

				state.VegetationFraction = 0;
				state.SnowFraction = 0;

				state.RunoffAccumulation = 0;
				state.FreshwaterDepth = 0;
				state.RiverDischarge = 0;

				if (!face.IsOcean)
					continue;

				state.SeaSurfaceTemperature = GetSoilDepth( globe, latitudeFraction );
				state.SeaSalinity = (float) globe.OceanMixedLayerAndCurrents.OceanSalinityReference;
				state.OceanCurrent = 0;

			}
		} );
	}

	private float GetSoilDepth( TectonicGeneratingGlobe globe, double latitudeFraction ) {
		Parameters.DynamicInitializationConstants dynamicConstants = globe.DynamicInitializationConstants;
		double absoluteLatitudeFraction = Math.Abs( latitudeFraction );

		return (float) (dynamicConstants.EquatorSST - dynamicConstants.PolarSSTReduction * absoluteLatitudeFraction);
	}

	private float GetSpecificHumidity( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, double latitude ) {
		var universalConstants = globe.UniversalConstants;
		var dynamicConstants = globe.DynamicInitializationConstants;

		double invT0 = 1.0 / dynamicConstants.EquatorialAirTemperature;
		double invT = 1.0 / state.Temperature;
		double esat = dynamicConstants.ReferenceSaturationVaporPressure * Math.Exp( universalConstants.ClausiusClapeyronExponent * (invT0 - invT) );
		return (float) (dynamicConstants.InitialRelativeHumidity * universalConstants.MolecularWeightRatioVaporDryAir * esat / (state.Pressure - esat));
	}

	private Pressure GetPressure( TectonicGeneratingGlobe globe, TectonicFaceState state ) {
		Parameters.DynamicInitializationConstants dynamicConstants = globe.DynamicInitializationConstants;

		var exponent = globe.UniversalConstants.SpecificHeatCapacity * dynamicConstants.DryAirMolarMass / globe.UniversalConstants.UniversalGasConstant;
		var @exponentBase = 1 - globe.PlanetaryParameters.Gravity * state.ElevationMeanAboveSea / (globe.UniversalConstants.UniversalGasConstant * state.Temperature.Kelvin);

		var barometricExponent = Math.Pow( @exponentBase, exponent );

		return dynamicConstants.SeaLevelPressure * float.Exp( -(float) (globe.PlanetaryParameters.Gravity * state.ElevationMeanAboveSea * dynamicConstants.DryAirMolarMass / (state.Temperature.Kelvin * globe.UniversalConstants.UniversalGasConstant)) );
	}

	private Temperature GetTemperatureFromLatitude( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center ) {
		Parameters.DynamicInitializationConstants dynamicConstants = globe.DynamicInitializationConstants;
		double absoluteLatitudeFraction = Math.Abs( center.Y );

		return dynamicConstants.EquatorialAirTemperature
			- dynamicConstants.PolarAirTemperatureReduction * absoluteLatitudeFraction
			- dynamicConstants.LapseRate * state.ElevationMeanAboveSea;
	}

	private float GetSurfaceRoughness( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, double sigmaMin, double sigmaMax ) {
		var bareGroundMinimum = globe.SurfaceRoughnessLengthConstants.BareGroundMinimum;
		var bareGroundMaximum = globe.SurfaceRoughnessLengthConstants.BareGroundMaximum;

		double σ = state.BaselineValues.ElevationStandardDeviation;
		double normSigma = (σ - sigmaMin) / (sigmaMax - sigmaMin);
		normSigma = Math.Clamp( normSigma, 0.0, 1.0 );
		return (float) (bareGroundMinimum + (bareGroundMaximum - bareGroundMinimum) * normSigma);
	}

	private float GetSoilDepth( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, double latitude, Noise3 soilCoarseNoise, Noise3 soilFineNoise ) {
		double S = state.BaselineValues.Gradient.Magnitude<Vector3<float>, float>();
		double baseDepth = globe.SoilDepthGenerationConstants.MaxSoilDepth * Math.Exp( -globe.SoilDepthGenerationConstants.SlopeDecayConstant * S );
		double latNorm = Math.Abs( latitude ) / 90.0;
		double climateFactor = 1.0 - 0.5 * latNorm;

		double soilNoise = soilCoarseNoise.Noise( center ) * 0.65 + soilFineNoise.Noise( center ) * 0.35;
		soilNoise = soilNoise * 2 - 1;
		baseDepth *= 1 + globe.SoilDepthGenerationConstants.SoilNoiseAmplitude * soilNoise;

		return (float) (baseDepth * climateFactor);
	}

	private float GetSoilMoistureCapacity( TectonicGeneratingGlobe globe, TectonicFaceState state, Vector3<float> center, Noise3 porosityCoarseNoise, Noise3 porosityFineNoise ) {
		double porosityNoise = porosityCoarseNoise.Noise( center ) * 0.65 + porosityFineNoise.Noise( center ) * 0.35;
		porosityNoise = porosityNoise * 2 - 1;

		float porosity = (float) (globe.SoilDepthGenerationConstants.SoilPorosityBase * (1 + globe.SoilDepthGenerationConstants.SoilPorosityNoiseAmplitude * porosityNoise));

		return state.SoilDepth * porosity * 1000.0f;
	}
}
