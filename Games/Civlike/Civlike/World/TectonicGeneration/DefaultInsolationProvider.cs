using Civlike.World.GenerationState;
using Engine;
using System.Runtime.Intrinsics.X86;

namespace Civlike.World.TectonicGeneration;

public class DefaultInsolationProvider( TectonicGeneratingGlobe globe ) : IInsolationProvider {
	public TectonicGeneratingGlobe Globe { get; } = globe;

	private float _rRel;
	private float _rRelSq;
	private float _invRRelSq;
	private float _delta;
	private float _sinDelta;
	private float _cosDelta;
	private float _tanDelta;
	private float _dailyMeanPreprocessConstant;

	public void Preprocess( double daysSimulated ) {
		float orbitDays = (float) (Globe.PlanetaryParameters.OrbitPeriod / Globe.PlanetaryParameters.RotationPeriod);
		float dayOfYear = 1 + (int) double.Floor( daysSimulated % orbitDays );
		float obliquityRad = float.DegreesToRadians( (float) Globe.PlanetaryParameters.Obliquity );

		float gamma = 2 * float.Pi * (dayOfYear - 3) / (float) orbitDays;
		float theta = 2 * float.Pi * (dayOfYear - 81) / (float) orbitDays;
		_rRel = 1 + (float) Globe.PlanetaryParameters.Eccentricity * float.Cos( gamma );
		_rRelSq = _rRel * _rRel;
		_invRRelSq = 1 / _rRelSq;

		_delta = float.Asin( float.Sin( obliquityRad ) * float.Sin( theta ) );
		_sinDelta = float.Sin( _delta );
		_cosDelta = float.Cos( _delta );
		_tanDelta = float.Tan( _delta );
		_dailyMeanPreprocessConstant = ((float) Globe.PlanetaryParameters.MeanSolarConstant / float.Pi) * _invRRelSq;
	}

	public float GetDailyMeanInsolation( Face face ) {
		float sinLat = face.LatitudeSin;
		float cosLat = face.LatitudeCos;
		float tanLat = face.LatitudeTan;

		float cosH0 = -tanLat * _tanDelta;
		cosH0 = float.Clamp( cosH0, -1, 1 );
		float h0 = float.Acos( cosH0 );

		float dailyMean = _dailyMeanPreprocessConstant * (h0 * sinLat * _sinDelta + cosLat * _cosDelta * float.Sin( h0 ));

		dailyMean = float.Max( dailyMean, 0 );

		return dailyMean;
	}

	public float GetDailyMeanInsolation( float latRad ) {
		float sinLat = float.Sin( latRad );
		float cosLat = float.Cos( latRad );
		float tanLat = float.Tan( latRad );
		float cosH0 = -tanLat * _tanDelta;
		cosH0 = float.Clamp( cosH0, -1, 1 );
		float h0 = float.Acos( cosH0 );
		float dailyMean = _dailyMeanPreprocessConstant * (h0 * sinLat * _sinDelta + cosLat * _cosDelta * float.Sin( h0 ));
		dailyMean = float.Max( dailyMean, 0 );
		return dailyMean;
	}
}