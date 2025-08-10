using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class TectonicPlateGenerator {

	private readonly TectonicPlate[] _plates;

	public TectonicPlateGenerator( Random r, int voronoiCenterPoints, float minDistance, float minHeight, float maxHeight, float plateHeightNoiseScale ) {
		if (minDistance <= 0)
			throw new ArgumentOutOfRangeException( nameof( minDistance ), "Minimum distance must be greater than 0." );
		this._plates = new TectonicPlate[ voronoiCenterPoints ];

		Noise3 heightNoise = new( r.Next(), plateHeightNoiseScale );

		for (int i = 0; i < voronoiCenterPoints; i++) {
			bool foundOverlap;
			Vector3<float> vector;
			Vector3<float> movementVector;
			do {
				foundOverlap = false;
				float yaw = (r.NextSingle() * 2 - 1) * float.Pi;
				float pitch = (r.NextSingle() * 2 - 1) * float.Pi;
				Vector2<float> sphericalCoordinates = (yaw, pitch);
				vector = sphericalCoordinates.ToCartesianFromPolar( 1 );
				for (int j = 0; j < i; j++) {
					Vector3<float> diff = vector - this._plates[ j ].Position;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance > minDistance)
						continue;
					foundOverlap = true;
					break;
				}
			} while (foundOverlap);
			{
				float yawMovement = (r.NextSingle() * 2 - 1) * float.Pi;
				float pitchMovement = (r.NextSingle() * 2 - 1) * float.Pi;
				Vector2<float> movementSpherical = (yawMovement, pitchMovement);
				movementVector = movementSpherical.ToCartesianFromPolar( 1 );
			}
			this._plates[ i ] = new TectonicPlate( i, vector, movementVector - movementVector.Dot( vector ) * vector, minHeight + (maxHeight - minHeight) * heightNoise.Noise( vector ) );
		}
	}

	public TectonicPlate Get( Vector3<float> xyz, List<(TectonicPlate, float)> neighbours, float borderGradientFactor, float lowerBounds ) {
		neighbours.Clear();
		TectonicPlate? currentPlate = null;
		float currentMinDistance = float.MaxValue;
		for (int i = 0; i < this._plates.Length; i++) {
			TectonicPlate plate = this._plates[ i ];
			Vector3<float> vec = plate.Position;
			Vector3<float> diff = xyz - vec;
			float distance = diff.MagnitudeSquared();
			if (distance < currentMinDistance) {
				if (currentPlate is not null)
					neighbours.Add( (currentPlate, currentMinDistance) );
				currentMinDistance = distance;
				currentPlate = plate;
				continue;
			} else
				neighbours.Add( (plate, distance) );
		}
		if (currentPlate is null)
			throw new InvalidOperationException( "No plate found." );

		for (int i = neighbours.Count - 1; i >= 0; i--) {
			float distance = neighbours[ i ].Item2;
			Vector3<float> pointsDiff = currentPlate.Position - neighbours[ i ].Item1.Position;
			float pointsDistance = pointsDiff.Magnitude<Vector3<float>, float>();
			float gradient = float.Exp( -(distance - currentMinDistance) / pointsDistance * borderGradientFactor );
			if (gradient < lowerBounds) {
				neighbours.RemoveAt( i );
				continue;
			}
			neighbours[ i ] = (neighbours[ i ].Item1, gradient);
		}

		return currentPlate;
	}
}
