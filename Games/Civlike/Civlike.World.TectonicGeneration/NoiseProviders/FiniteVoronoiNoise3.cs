using Engine;
using System;
using System.Numerics;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class FiniteVoronoiNoise3 {

	private readonly Vector3<float>[] _points;

	private readonly AABB<Vector3<int>> _bounds;

	private readonly float _detailLevel;
	private readonly float _cbrtDetailLevel;

	private readonly int _lengthZ;
	private readonly int _areaYZ;

	private const float _cbrt2 = 1.2599210498948732f; // 2^(1/3)

	/// <summary>
	/// 
	/// </summary>
	/// <param name="r"></param>
	/// <param name="detailLevel">Distance between each cell</param>
	/// <param name="cubicSize">Size of the finite span in one direction. All direction on the cube are equally long.</param>
	public FiniteVoronoiNoise3( Random r, float detailLevel, float cubicSize ) {
		this._detailLevel = detailLevel;
		this._cbrtDetailLevel = this._detailLevel / _cbrt2;
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
		return vector.X * this._areaYZ + vector.Y * this._lengthZ + vector.Z;
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
					Vector3<float> vec = lowF + new Vector3<float>( i, j, k ) * this._detailLevel + this._points[ index ];
					Vector3<float> diff = xyz - vec;
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance < minDistance)
						minDistance = distance;
				}
		return 1 - minDistance / this._cbrtDetailLevel;
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
					Vector3<float> vec = lowF + new Vector3<float>( i, j, k ) * this._detailLevel + this._points[ index ];
					Vector3<float> diff = xyz - vec;
					float distance = diff.Magnitude<Vector3<float>, float>();
					distances[ distIndex ] = distance;
					distIndex++;
					if (distance < minDistance)
						minDistance = distance;
				}

		float gradient = 0;
		int numGradientContrbuters = 0;
		for (int i = 0; i < distIndex; i++) {
			if (distances[ i ] == minDistance)
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
					Vector3<float> vec = lowF + new Vector3<float>( i, j, k ) * this._detailLevel + this._points[ index ];
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
public sealed class FiniteSphericalVoronoiNoise3 {

	private readonly Vector3<float>[] _points;

	public FiniteSphericalVoronoiNoise3( Random r, int pointCount, float minDistanceBetweenPoints ) {
		this._points = new Vector3<float>[ pointCount ];

		for (int i = 0; i < pointCount; i++) {
			bool foundOverlap;
			Vector3<float> vector;
			do {
				foundOverlap = false;
				float yaw = (r.NextSingle() * 2 - 1) * float.Pi;
				float pitch = (r.NextSingle() * 2 - 1) * float.Pi;
				Vector2<float> sphericalCoordinates = (yaw, pitch);
				vector = sphericalCoordinates.ToCartesianFromPolar( 1 );
				for (int j = 0; j < i; j++) {
					Vector3<float> diff = vector - this._points[ j ];
					float distance = diff.Magnitude<Vector3<float>, float>();
					if (distance > minDistanceBetweenPoints)
						continue;
					foundOverlap = true;
					break;
				}
			} while (foundOverlap);

			this._points[ i ] = vector;
		}
	}

	public float Noise( Vector3<float> p ) {
		int iBest = -1, iSecond = -1;
		float bestDot = -2f, secondDot = -2f;

		// Find nearest and 2nd-nearest by ANGLE using only dot products (fast).
		for (int i = 0; i < _points.Length; i++) {
			float dot = p.Dot( _points[ i ] ); // both unit => dot = cos(theta)
			if (dot > bestDot) {
				secondDot = bestDot;
				iSecond = iBest;
				bestDot = dot;
				iBest = i;
			} else if (
				dot > secondDot) {
				secondDot = dot;
				iSecond = i;
			}
		}

		if (iSecond < 0)
			return 0f;

		// Convert only twice to geodesic angles (radians). Clamp guards float error.
		float d0 = float.Clamp( bestDot, -1.0f, 1.0f );
		float d1 = float.Clamp( secondDot, -1.0f, 1.0f );

		// More stable than acos: atan2(|a×b|, a·b)
		Vector3<float> s0 = _points[ iBest ];
		Vector3<float> s1 = _points[ iSecond ];

		float ang1 = float.Atan2( p.Cross( s0 ).Magnitude<Vector3<float>, float>(), d0 );
		float ang2 = float.Atan2( p.Cross( s1 ).Magnitude<Vector3<float>, float>(), d1 );

		// If you need actual meters: dist = radius * ang.
		// For a 0..1 noise value use the angle ratio (scale-free):
		return float.Clamp( ang1 / ang2, 0f, 1f );
	}

	public Vector3<float> NoiseVector( Vector3<float> xyz ) {
		Vector3<float> output = 0;
		float minDistance = float.PositiveInfinity;
		for (int i = 0; i < this._points.Length; i++) {
			Vector3<float> vec = this._points[ i ];
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
