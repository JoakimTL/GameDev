using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class SphericalVoronoiGenerator {

	private readonly SphericalVoronoiRegion[] _regions;

	public SphericalVoronoiGenerator( Random r, int voronoiCenterPointCount, float minDistance ) {
		if (minDistance <= 0)
			throw new ArgumentOutOfRangeException( nameof( minDistance ), "Minimum distance must be greater than 0." );
		this._regions = new SphericalVoronoiRegion[ voronoiCenterPointCount ];

		for (int i = 0; i < voronoiCenterPointCount; i++) {
			bool foundOverlap;
			Vector3<float> vector;
			do {
				foundOverlap = false;
				float yaw = (r.NextSingle() * 2 - 1) * float.Pi;
				float pitch = (r.NextSingle() * 2 - 1) * float.Pi;
				Vector2<float> sphericalCoordinates = (yaw, pitch);
				vector = sphericalCoordinates.ToCartesianFromPolar( 1 );
				for (int j = 0; j < i; j++) {
					Vector3<float> diff = vector - this._regions[ j ].Position;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance > minDistance)
						continue;
					foundOverlap = true;
					break;
				}
			} while (foundOverlap);

			this._regions[ i ] = new SphericalVoronoiRegion( vector );
		}
	}

	public IReadOnlyList<SphericalVoronoiRegion> Regions => this._regions;

	public SphericalVoronoiRegion Get( Vector3<float> xyz ) {
		SphericalVoronoiRegion? currentRegion = null;
		float currentMinDistance = float.MaxValue;
		for (int i = 0; i < this._regions.Length; i++) {
			SphericalVoronoiRegion region = this._regions[ i ];
			Vector3<float> vec = region.Position;
			Vector3<float> diff = xyz - vec;
			float distance = diff.MagnitudeSquared();
			if (distance < currentMinDistance) {
				currentMinDistance = distance;
				currentRegion = region;
				continue;
			}
		}
		if (currentRegion is null)
			throw new InvalidOperationException( "No plate found." );

		return currentRegion;
	}

	public SphericalVoronoiRegion GetWithGradients( Vector3<float> xyz, List<(SphericalVoronoiRegion region, float gradient)> neighbours, float borderGradientFactor, float lowerBounds ) {
		neighbours.Clear();
		SphericalVoronoiRegion? currentRegion = null;
		float currentMinDistance = float.MaxValue;
		for (int i = 0; i < this._regions.Length; i++) {
			SphericalVoronoiRegion region = this._regions[ i ];
			Vector3<float> vec = region.Position;
			Vector3<float> diff = xyz - vec;
			float distance = diff.MagnitudeSquared();
			if (distance < currentMinDistance) {
				if (currentRegion is not null)
					neighbours.Add( (currentRegion, currentMinDistance) );
				currentMinDistance = distance;
				currentRegion = region;
				continue;
			} else
				neighbours.Add( (region, distance) );
		}
		if (currentRegion is null)
			throw new InvalidOperationException( "No plate found." );

		for (int i = neighbours.Count - 1; i >= 0; i--) {
			float distance = neighbours[ i ].Item2;
			Vector3<float> pointsDiff = currentRegion.Position - neighbours[ i ].Item1.Position;
			float pointsDistance = pointsDiff.Magnitude<Vector3<float>, float>();
			float gradient = float.Exp( -(distance - currentMinDistance) / pointsDistance * borderGradientFactor );
			if (gradient < lowerBounds) {
				neighbours.RemoveAt( i );
				continue;
			}
			neighbours[ i ] = (neighbours[ i ].region, gradient);
		}

		return currentRegion;
	}
}
