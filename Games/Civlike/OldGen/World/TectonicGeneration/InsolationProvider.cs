//using Engine;
//using OldGen.World.GenerationState;
//using System.Runtime.InteropServices;

//namespace OldGen.World.TectonicGeneration;

//public unsafe class InsolationProvider( TectonicGeneratingGlobe globe, Func<InsolationProvider, Latitude, float> insolationFunc ) : DisposableIdentifiable {
//	public TectonicGeneratingGlobe Globe { get; } = globe;

//	private bool _pointerInitialized = false;
//	private float* _insolationAtLatId = null!;

//	private float _rRel;
//	private float _rRelSq;
//	private float _invRRelSq;
//	private float _delta;
//	private float _sinDelta;
//	private float _cosDelta;
//	private float _tanDelta;
//	private float _dailyMeanPreprocessConstant;
//	private readonly Func<InsolationProvider, Latitude, float> _insolationFunc = insolationFunc;

//	public float SinDelta => this._sinDelta;
//	public float CosDelta => this._cosDelta;
//	public float TanDelta => this._tanDelta;
//	public float DailyMeanPreprocessConstant => this._dailyMeanPreprocessConstant;

//	public void Preprocess( double daysSimulated ) {
//		if (!this._pointerInitialized) {
//			this._insolationAtLatId = (float*) NativeMemory.Alloc( (uint) (this.Globe.Latitudes.Count * sizeof( float )) );
//			this._pointerInitialized = true;
//		}

//		float orbitDays = (float) (this.Globe.PlanetaryConstants.OrbitPeriod / this.Globe.PlanetaryConstants.RotationPeriod);
//		float obliquityRad = float.DegreesToRadians( (float) this.Globe.PlanetaryConstants.Obliquity );
//		float dayOfYear = 1 + (int) double.Floor( daysSimulated % orbitDays );

//		float gamma = 2 * float.Pi * (dayOfYear - 3) / (float) orbitDays;
//		float theta = 2 * float.Pi * (dayOfYear - 81) / (float) orbitDays;
//		this._rRel = 1 + (float) this.Globe.PlanetaryConstants.Eccentricity * MathF.Cos( gamma );
//		this._rRelSq = this._rRel * this._rRel;
//		this._invRRelSq = 1 / this._rRelSq;

//		this._delta = MathF.Asin( MathF.Sin( obliquityRad ) * MathF.Sin( theta ) );
//		this._sinDelta = MathF.Sin( this._delta );
//		this._cosDelta = MathF.Cos( this._delta );
//		this._tanDelta = MathF.Tan( this._delta );
//		this._dailyMeanPreprocessConstant = (float) this.Globe.PlanetaryConstants.MeanSolarConstant / float.Pi * this._invRRelSq;

//		ParallelProcessing.Range( this.Globe.Latitudes.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Latitude latitude = this.Globe.Latitudes[ i ];
//				this._insolationAtLatId[ i ] = this._insolationFunc( this, latitude );
//			}
//		} );
//	}

//	public float GetDailyMeanInsolation( Face<TectonicFaceState> face )
//		=> this._insolationAtLatId[ face.LatitudeId ];

//	protected override bool InternalDispose() {
//		if (this._pointerInitialized)
//			NativeMemory.Free( this._insolationAtLatId );
//		return true;
//	}
//}
