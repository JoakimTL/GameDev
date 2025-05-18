using Engine;

namespace Civlike.World.Generation;

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
		_detailLevel = detailLevel;
		_cbrtDetailLevel = _detailLevel / cbrt2;
		int cellNumbers = (int) MathF.Ceiling( cubicSize / detailLevel );
		_bounds = AABB.Create( [
			new Vector3<int>( -cellNumbers, -cellNumbers, -cellNumbers ),
			new Vector3<int>( cellNumbers, cellNumbers, cellNumbers )
		] );
		_points = new Vector3<float>[ (_bounds.Maxima.X - _bounds.Minima.X + 1) * (_bounds.Maxima.Y - _bounds.Minima.Y + 1) * (_bounds.Maxima.Z - _bounds.Minima.Z + 1) ];
		for (int i = _bounds.Minima.X; i <= _bounds.Maxima.X; i++) 			for (int j = _bounds.Minima.Y; j <= _bounds.Maxima.Y; j++) 				for (int k = _bounds.Minima.Z; k <= _bounds.Maxima.Z; k++) 					_points[ Index( (i, j, k) ) ] = new Vector3<float>( r.NextSingle() * detailLevel, r.NextSingle() * detailLevel, r.NextSingle() * detailLevel );
		_lengthZ = _bounds.Maxima.Z - _bounds.Minima.Z + 1;
		_areaYZ = (_bounds.Maxima.Y - _bounds.Minima.Y + 1) * _lengthZ;
	}

	private int Index( Vector3<int> vector ) {
		if (!_bounds.Contains( vector ))
			return -1;
		vector = vector - _bounds.Minima;
		return vector.X * _areaYZ + vector.Y * _lengthZ + vector.Z;
	}

	public float Noise( Vector3<float> xyz ) {
		Vector3<int> low = (xyz / _detailLevel).Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> lowF = low.CastSaturating<int, float>() * _detailLevel;
		float minDistance = _detailLevel;
		for (int i = -1; i <= 1; i++) 			for (int j = -1; j <= 1; j++) 				for (int k = -1; k <= 1; k++) {
					int index = Index( low + (i, j, k) );
					if (index == -1)
						continue;
					Vector3<float> vec = lowF + new Vector3<float>( i, j, k ) * _detailLevel + _points[ index ];
					Vector3<float> diff = xyz - vec;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance < minDistance)
						minDistance = distance;
				}
		return 1 - minDistance / _cbrtDetailLevel;
	}

	public Vector3<float> NoiseVector( Vector3<float> xyz ) {
		Vector3<int> low = (xyz / _detailLevel).Floor<Vector3<float>, float>().CastTruncating<float, int>();
		Vector3<float> lowF = low.CastSaturating<int, float>() * _detailLevel;
		float minDistance = _detailLevel;
		Vector3<float> output = 0;
		for (int i = -1; i <= 1; i++) 			for (int j = -1; j <= 1; j++) 				for (int k = -1; k <= 1; k++) {
					int index = Index( low + (i, j, k) );
					if (index == -1)
						continue;
					Vector3<float> vec = lowF + new Vector3<float>( i, j, k ) * _detailLevel + _points[ index ];
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
