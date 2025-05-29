using Civlike.World.GameplayState;
using Civlike.World.GenerationState;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Civlike.World.TectonicGeneration.Steps;
public static class PhysicsHelpers {

	public static Pressure GetPressure( TectonicGeneratingGlobe globe, TectonicFaceState state ) {
		Parameters.SeaLandAirConstants slaC = globe.SeaLandAirConstants;

		return slaC.SeaLevelPressure * float.Exp( -state.CachedLapseRate / state.AirTemperature.Kelvin );
	}
	public static float GetLapseRate( TectonicGeneratingGlobe globe, TectonicFaceState state ) {
		Parameters.UniversalConstants uC = globe.UniversalConstants;
		Parameters.PlanetaryConstants pC = globe.PlanetaryConstants;
		Parameters.SeaLandAirConstants slaC = globe.SeaLandAirConstants;

		return (float) (pC.Gravity * state.PressureElevationMean * slaC.DryAirMolarMass / uC.UniversalGasConstant);
	}

	public static Vector3 GetPressureGradient( in Face<TectonicFaceState> face, in TectonicFaceState state ) {
		float pressure = state.CombinedPressure.Pascal;
		IReadOnlyList<NeighbouringFace<TectonicFaceState>> neighbours = face.Neighbours;
		NeighbouringFace<TectonicFaceState> nbrA = neighbours[ 0 ];
		NeighbouringFace<TectonicFaceState> nbrB = neighbours[ 1 ];
		NeighbouringFace<TectonicFaceState> nbrC = neighbours[ 2 ];
		float pA = nbrA.Face.State.CombinedPressure.Pascal;
		float pB = nbrB.Face.State.CombinedPressure.Pascal;
		float pC = nbrC.Face.State.CombinedPressure.Pascal;
		float deltaA = pA - pressure;
		float deltaB = pB - pressure;
		float deltaC = pC - pressure;
		Vector3 deltas = new( deltaA, deltaB, deltaC );
		Vector3 ildX = face.InverseLengthNeighbourDirectionX;
		Vector3 ildY = face.InverseLengthNeighbourDirectionY;
		Vector3 ildZ = face.InverseLengthNeighbourDirectionZ;
		return new( Vector3.Dot( deltas, ildX ), Vector3.Dot( deltas, ildY ), Vector3.Dot( deltas, ildZ ) );
	}

	public static Vector3 ApplyCoriolis( in Vector3 velocity, in Vector3 upAxis, float coriolisFactor )
		=> Vector3.Cross(upAxis, velocity ) * coriolisFactor;

	public static Vector3 ApplyDrag( in Vector3 velocity, float linCoefficient, float quadCoefficient )
		=> velocity / (1f + linCoefficient + quadCoefficient * velocity.Length());

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3 GetWindVector(Face<TectonicFaceState> face, TectonicFaceState state, float pressureGradientCoefficient, Vector3 upAxis, float linearDragCoefficient, float quadraticDragCoefficient) {
		Vector3 n = face.CenterNormalized;

		Vector3 pressureGradient = GetPressureGradient( face, state );
		Vector3 pressureWind = pressureGradientCoefficient * pressureGradient;
		Vector3 coriolis = ApplyCoriolis( pressureWind, upAxis, state.CoriolisFactor );
		Vector3 windBeforeDrag = pressureWind + coriolis + state.HadleyWinds;

		return ApplyDrag( windBeforeDrag, linearDragCoefficient, quadraticDragCoefficient );
	}

	public static float GetSurfaceTemperature( TectonicGeneratingGlobe globe, in Vector3<float> center ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		double absoluteLatitudeFraction = Math.Abs( center.Y );

		return (float) (initializationParameters.EquatorialSeaSurfaceTemperature - initializationParameters.PolarSeaSurfaceTemperatureReduction * absoluteLatitudeFraction);
	}

	public static float GetSpecificHumidity( TectonicGeneratingGlobe globe, TectonicFaceState state ) {
		Parameters.UniversalConstants universalConstants = globe.UniversalConstants;
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		Parameters.SeaLandAirConstants seaLandAirConstants = globe.SeaLandAirConstants;

		double invT0 = 1.0 / initializationParameters.EquatorialAirTemperature;
		double invT = 1.0 / state.AirTemperature;
		double esat = seaLandAirConstants.ReferenceSaturationVaporPressure * Math.Exp( universalConstants.ClausiusClapeyronExponent * (invT0 - invT) );
		return (float) (initializationParameters.InitialRelativeHumidity * seaLandAirConstants.MolecularWeightRatioVaporDryAir * esat / (state.Pressure - esat));
	}

	public static Temperature GetTemperatureFromLatitude( TectonicGeneratingGlobe globe, TectonicFaceState state, in Vector3<float> center ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		double absoluteLatitudeFraction = Math.Abs( center.Y );

		return initializationParameters.EquatorialAirTemperature
			- initializationParameters.PolarAirTemperatureReduction * absoluteLatitudeFraction
			- initializationParameters.LapseRate * state.PressureElevationMean;
	}

	public static float GetSurfaceRoughness( TectonicGeneratingGlobe globe, TectonicFaceState state, double sigmaMin, double sigmaMax ) {
		double bareGroundMinimum = globe.EndMemberProperties.DrySoil.MinimumRoughnessLength;
		double bareGroundMaximum = globe.EndMemberProperties.DrySoil.MaximumRoughnessLength;

		double σ = state.BaselineValues.ElevationStandardDeviation;
		double normSigma = (σ - sigmaMin) / (sigmaMax - sigmaMin);
		normSigma = Math.Clamp( normSigma, 0.0, 1.0 );
		return (float) (bareGroundMinimum + (bareGroundMaximum - bareGroundMinimum) * normSigma);
	}

	public static float GetSoilDepth( TectonicGeneratingGlobe globe, TectonicFaceState state, double latitude, float soilNoise ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;
		Parameters.SeaLandAirConstants seaLandAirConstants = globe.SeaLandAirConstants;

		double S = state.BaselineValues.Gradient.Length();
		double baseDepth = initializationParameters.SoilDepthBase * Math.Exp( -seaLandAirConstants.SlopeDecayConstant * S );
		double latNorm = Math.Abs( latitude ) / 90.0;
		double climateFactor = 1.0 - 0.5 * latNorm;

		soilNoise = soilNoise * 2 - 1;
		baseDepth *= 1 + initializationParameters.SoilNoiseAmplitude * soilNoise;

		return (float) (baseDepth * climateFactor);
	}

	public static float GetSoilMoistureCapacity( TectonicGeneratingGlobe globe, TectonicFaceState state, float porosityNoise ) {
		Parameters.InitializationParameters initializationParameters = globe.InitializationParameters;

		porosityNoise = porosityNoise * 2 - 1;

		float porosity = (float) (initializationParameters.SoilPorosityBase * (1 + initializationParameters.SoilPorosityNoiseAmplitude * porosityNoise));

		return state.SoilDepth * porosity * 1000.0f;
	}
}
