using Civlike.World.GenerationState;
using Engine;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Civlike.World.TectonicGeneration;

public unsafe class DefaultInsolationProvider( TectonicGeneratingGlobe globe ) : DisposableIdentifiable, IInsolationProvider {
	public TectonicGeneratingGlobe Globe { get; } = globe;

	private bool _pointerInitialized = false;
	private float* _insolationAtLatId = null!;

	private float _rRel;
	private float _rRelSq;
	private float _invRRelSq;
	private float _delta;
	private float _sinDelta;
	private float _cosDelta;
	private float _tanDelta;
	private float _dailyMeanPreprocessConstant;

	public void Preprocess( double daysSimulated ) {
		if (!_pointerInitialized) {
			_insolationAtLatId = (float*) NativeMemory.Alloc( (uint) (Globe.Latitudes.Count * sizeof( float )) );
			_pointerInitialized = true;
		}

		float orbitDays = (float) (Globe.PlanetaryConstants.OrbitPeriod / Globe.PlanetaryConstants.RotationPeriod);
		float obliquityRad = float.DegreesToRadians( (float) Globe.PlanetaryConstants.Obliquity );
		float dayOfYear = 1 + (int) double.Floor( daysSimulated % orbitDays );

		float gamma = 2 * float.Pi * (dayOfYear - 3) / (float) orbitDays;
		float theta = 2 * float.Pi * (dayOfYear - 81) / (float) orbitDays;
		_rRel = 1 + (float) Globe.PlanetaryConstants.Eccentricity * MathF.Cos( gamma );
		_rRelSq = _rRel * _rRel;
		_invRRelSq = 1 / _rRelSq;

		_delta = MathF.Asin( MathF.Sin( obliquityRad ) * MathF.Sin( theta ) );
		_sinDelta = MathF.Sin( _delta );
		_cosDelta = MathF.Cos( _delta );
		_tanDelta = MathF.Tan( _delta );
		_dailyMeanPreprocessConstant = ((float) Globe.PlanetaryConstants.MeanSolarConstant / float.Pi) * _invRRelSq;

		ParallelProcessing.Range( Globe.Latitudes.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Latitude latitude = Globe.Latitudes[ i ];
				_insolationAtLatId[ i ] = GetDailyMeanInsolation( latitude );
			}
		} );
	}

	public float GetDailyMeanInsolation( Face<TectonicFaceState> face ) 
		=> _insolationAtLatId[ face.LatitudeId ];

	private float GetDailyMeanInsolation( Latitude latitude ) {
		float sinLat = latitude.Sin;
		float cosLat = latitude.Cos;
		float tanLat = latitude.Tan;
		float cosH0 = -tanLat * _tanDelta;
		cosH0 = float.Clamp( cosH0, -1, 1 );
		float h0 = MathF.Acos( cosH0 );
		float dailyMean = _dailyMeanPreprocessConstant * (h0 * sinLat * _sinDelta + cosLat * _cosDelta * MathF.Sin( h0 ));
		dailyMean = MathF.Max( dailyMean, 0 );
		return dailyMean;
	}

	protected override bool InternalDispose() {
		if (_pointerInitialized)
			NativeMemory.Free( _insolationAtLatId );
		return true;
	}
}