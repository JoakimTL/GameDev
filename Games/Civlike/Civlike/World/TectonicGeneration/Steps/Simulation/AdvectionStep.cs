using Civlike.World.GenerationState;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

public sealed class AdvectionStep : ISimulationStep {
	private float[]? _newQ;
	private float[]? _newT;
	private readonly SphericalTriangleLocator _sphericalTriangleLocator = new();

	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {

		float dt = (float) secondsToSimulate;
		float area = (float) globe.TileArea;
		float dtArea = dt / area;
		float edgeLen = (float) globe.TileLength * 2;
		float maxWindMagnitude = float.Sqrt( globe.TectonicFaces.Max( p => p.State.Wind.MagnitudeSquared() ) );
		float dtLoop = dt / float.Max( 1, maxWindMagnitude * dt / edgeLen );

		if (_newQ is null || _newQ.Length != globe.TectonicFaces.Count)
			_newQ = new float[ globe.TectonicFaces.Count ];
		if (_newT is null || _newT.Length != globe.TectonicFaces.Count)
			_newT = new float[ globe.TectonicFaces.Count ];

		float accumulatedTime = 0;

		while (accumulatedTime < dt) {
			ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
				for (int i = start; i < end; i++) {
					Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
					TectonicFaceState state = face.State;

					Vector3<float> u = state.Wind;

					float divQ = 0;
					float divT = 0;

					foreach (NeighbouringFace neighbour in face.Neighbours) {
						Face<TectonicFaceState> nbrFace = neighbour.Face as Face<TectonicFaceState> ?? throw new InvalidOperationException( "Neighbour face is not of type TectonicFaceState." );
						TectonicFaceState nbrState = nbrFace.State;

						Vector3<float> dir = neighbour.NormalizedDirection;
						float un = u.Dot( dir );

						(float qUp, float tUp) = un >= 0 ? (state.SpecificHumidity, state.AirTemperature) : (nbrState.SpecificHumidity, nbrState.AirTemperature);

						float fluxQ = un * qUp;
						float fluxT = un * tUp;

						float Fq = fluxQ * edgeLen;
						float Ft = fluxT * edgeLen;

						divQ += Fq;
						divT += Ft;
					}

					_newQ[ i ] -= dtArea * divQ * dtLoop;
					_newT[ i ] -= dtArea * divT * dtLoop;
				}
			} );

			ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
				for (int i = start; i < end; i++) {
					Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
					TectonicFaceState state = face.State;

					state.SpecificHumidity = _newQ[ i ];
					state.AirTemperature = _newT[ i ];
				}
			} );

			accumulatedTime += dtLoop;
		}
	}
}

public sealed class SphericalTriangleLocator {

	private List<Face<TectonicFaceState>> _faceList = [];

	public Face<TectonicFaceState> LocateFace( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, Vector3<float> point ) {
		Vector3<float> normalizedPoint = point.Normalize<Vector3<float>, float>();

		float L = 1.17557f / (1 << (int) parameters.Subdivisions);

		_faceList.Clear();
		globe.FaceTree.Get( _faceList, normalizedPoint.CreateBounds( L * 3 ) );

		foreach (Face<TectonicFaceState> face in _faceList) {
			if (RayIntersectsTriangle( 0, normalizedPoint, face.VectorA, face.VectorB, face.VectorC, out _ ))
				return face;
		}

		throw new InvalidOperationException( "No face found for the given point." );
	}

	public static bool RayIntersectsTriangle( Vector3<float> rayOrigin, Vector3<float> rayDirection, Vector3<float> v0, Vector3<float> v1, Vector3<float> v2, out float t ) {
		const float EPSILON = 1e-8f;
		t = 0;

		Vector3<float> edge1 = v1 - v0;
		Vector3<float> edge2 = v2 - v0;

		Vector3<float> h = rayDirection.Cross( edge2 );
		float a = edge1.Dot( h );

		if (float.Abs( a ) < EPSILON)
			return false; // Ray is parallel to triangle.

		float f = 1.0f / a;
		Vector3<float> s = rayOrigin - v0;
		float u = f * s.Dot( h );

		if (u < 0.0f || u > 1.0f)
			return false;

		Vector3<float> q = s.Cross( edge1 );
		float v = f * rayDirection.Dot( q );

		if (v < 0.0f || u + v > 1.0f)
			return false;

		t = f * edge2.Dot( q );

		if (t > EPSILON)
			return true; // Intersection detected.

		return false; // Intersection is behind the ray.
	}
}