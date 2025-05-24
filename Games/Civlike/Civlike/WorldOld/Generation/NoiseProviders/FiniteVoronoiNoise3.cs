using Engine;

namespace Civlike.WorldOld.Generation.NoiseProviders;

public sealed class FiniteVoronoiNoise3 {

	private readonly Vector3<float>[] _points;

	private readonly AABB<Vector3<int>> _bounds;

	private readonly float _detailLevel;
	private readonly float _cbrtDetailLevel;

	private readonly int _lengthZ;
	private readonly int _areaYZ;

	private const float cbrt2 = 1.2599210498948732f; // 2^(1/3)

	/// <summary>
	/// 
	/// </summary>
	/// <param name="r"></param>
	/// <param name="detailLevel">Distance between each cell</param>
	/// <param name="cubicSize">Size of the finite span in one direction. All direction on the cube are equally long.</param>
	public FiniteVoronoiNoise3( Random r, float detailLevel, float cubicSize ) {
		this._detailLevel = detailLevel;
		this._cbrtDetailLevel = this._detailLevel / cbrt2;
		int cellNumbers = (int) MathF.Ceiling( cubicSize / detailLevel );
		this._bounds = AABB.Create( [
			new Vector3<int>( -cellNumbers, -cellNumbers, -cellNumbers ),
			new Vector3<int>( cellNumbers, cellNumbers, cellNumbers )
		] );
		this._points = new Vector3<float>[ (this._bounds.Maxima.X - this._bounds.Minima.X + 1) * (this._bounds.Maxima.Y - this._bounds.Minima.Y + 1) * (this._bounds.Maxima.Z - this._bounds.Minima.Z + 1) ];
		for (int i = this._bounds.Minima.X; i <= this._bounds.Maxima.X; i++)
			for (int j = this._bounds.Minima.Y; j <= this._bounds.Maxima.Y; j++)
				for (int k = this._bounds.Minima.Z; k <= this._bounds.Maxima.Z; k++)
					this._points[ Index( (i, j, k) ) ] = new Vector3<float>( r.NextSingle() * detailLevel, r.NextSingle() * detailLevel, r.NextSingle() * detailLevel );
		this._lengthZ = this._bounds.Maxima.Z - this._bounds.Minima.Z + 1;
		this._areaYZ = (this._bounds.Maxima.Y - this._bounds.Minima.Y + 1) * this._lengthZ;
	}

	private int Index( Vector3<int> vector ) {
		if (!this._bounds.Contains( vector ))
			return -1;
		vector = vector - this._bounds.Minima;
		return (vector.X * this._areaYZ) + (vector.Y * this._lengthZ) + vector.Z;
	}

	public float Noise( Vector3<float> xyz ) {
		Vector3<int> low = (xyz / this._detailLevel).Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> lowF = low.CastSaturating<int, float>() * this._detailLevel;
		float minDistance = this._detailLevel;
		for (int i = -1; i <= 1; i++)
			for (int j = -1; j <= 1; j++)
				for (int k = -1; k <= 1; k++) {
					int index = Index( low + (i, j, k) );
					if (index == -1)
						continue;
					Vector3<float> vec = lowF + (new Vector3<float>( i, j, k ) * this._detailLevel) + this._points[ index ];
					Vector3<float> diff = xyz - vec;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance < minDistance)
						minDistance = distance;
				}
		return 1 - (minDistance / this._cbrtDetailLevel);
	}

	public float BorderNoise( Vector3<float> xyz, float gradientFactor ) {
		Vector3<int> low = (xyz / this._detailLevel).Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> lowF = low.CastSaturating<int, float>() * this._detailLevel;
		float minDistance = this._detailLevel;
		Span<float> distances = stackalloc float[ 27 ];
		int distIndex = 0;
		for (int i = -1; i <= 1; i++)
			for (int j = -1; j <= 1; j++)
				for (int k = -1; k <= 1; k++) {
					int index = Index( low + (i, j, k) );
					if (index == -1)
						continue;
					Vector3<float> vec = lowF + (new Vector3<float>( i, j, k ) * this._detailLevel) + this._points[ index ];
					Vector3<float> diff = xyz - vec;
					float distance = diff.Magnitude<Vector3<float>, float>();
					distances[distIndex] = distance;
					distIndex++;
					if (distance < minDistance)
						minDistance = distance;
				}

		float gradient = 0;
		int numGradientContrbuters = 0;
		for (int i = 0; i < distIndex; i++) {
			if (distances[i] == minDistance)
				continue;
			float currentGradient = float.Exp( -(distances[ i ] - minDistance) * gradientFactor );
			if (currentGradient < 0.001f)
				continue;
			gradient += currentGradient;
			numGradientContrbuters++;
		}
		return numGradientContrbuters > 0 ? gradient / numGradientContrbuters : 0;
	}

	public Vector3<float> NoiseVector( Vector3<float> xyz ) {
		Vector3<int> low = (xyz / this._detailLevel).Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> lowF = low.CastSaturating<int, float>() * this._detailLevel;
		float minDistance = this._detailLevel;
		Vector3<float> output = 0;
		for (int i = -1; i <= 1; i++)
			for (int j = -1; j <= 1; j++)
				for (int k = -1; k <= 1; k++) {
					int index = Index( low + (i, j, k) );
					if (index == -1)
						continue;
					Vector3<float> vec = lowF + (new Vector3<float>( i, j, k ) * this._detailLevel) + this._points[ index ];
					Vector3<float> diff = xyz - vec;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance < minDistance) {
						minDistance = distance;
						output = diff;
					}
				}
		return output;
	}
}


public sealed class TectonicPlate( int id, Vector3<float> position, Vector3<float> movement, float height ) {
	public readonly int Id = id;
	public readonly Vector3<float> Position = position;
	public readonly Vector3<float> Movement = movement;
	public readonly float Height = height;

	//Negative number means these plates are moving away from each other.
	//Positive number means these plates are moving towards each other.
	public float GetFaultMovementDifference( TectonicPlate other ) {
		Vector3<float> positionDiff = (this.Position - other.Position).Normalize<Vector3<float>, float>();
		Vector3<float> movementDiff = (this.Movement - other.Movement) * 0.5f;
		return -positionDiff.Dot( movementDiff );
	}

	public float GetFaultReactionIntensity( TectonicPlate other ) {
		return float.Abs( this.Movement.Dot( other.Movement ) - 1 );
	}
}

public sealed class TectonicPlateGenerator {

	private readonly TectonicPlate[] _plates;

	public TectonicPlateGenerator( Random r, int voronoiCenterPoints, float minDistance, float minHeight, float maxHeight ) {
		if (minDistance <= 0)
			throw new ArgumentOutOfRangeException( nameof( minDistance ), "Minimum distance must be greater than 0." );
		this._plates = new TectonicPlate[ voronoiCenterPoints ];

		Noise3 heightNoise = new( r.Next(), 2.4f );

		for (int i = 0; i < voronoiCenterPoints; i++) {
			bool foundOverlap;
			Vector3<float> vector;
			Vector3<float> movementVector;
			do {
				foundOverlap = false;
				float yaw = ((r.NextSingle() * 2) - 1) * float.Pi;
				float pitch = ((r.NextSingle() * 2) - 1) * float.Pi;
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
				float yawMovement = ((r.NextSingle() * 2) - 1) * float.Pi;
				float pitchMovement = ((r.NextSingle() * 2) - 1) * float.Pi;
				Vector2<float> movementSpherical = (yawMovement, pitchMovement);
				movementVector = movementSpherical.ToCartesianFromPolar( 1 );
			}
			this._plates[ i ] = new TectonicPlate( i, vector, movementVector - (movementVector.Dot( vector ) * vector), minHeight + ((maxHeight - minHeight) * heightNoise.Noise( vector )) );
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
			} else 				neighbours.Add( (plate, distance) );
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