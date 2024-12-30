using System.Numerics;

namespace Engine.Physics;

public sealed class Simplex2<TScalar>()
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	private readonly MinkowskiDifference<Vector2<TScalar>>[] _minkowskiSums = new MinkowskiDifference<Vector2<TScalar>>[ 3 ];
	private int _count = 0;

	public IReadOnlyList<MinkowskiDifference<Vector2<TScalar>>> MinkowskiSums => _minkowskiSums;
	public int Count => _count;

	public MinkowskiDifference<Vector2<TScalar>> A => _minkowskiSums[ 0 ];
	public MinkowskiDifference<Vector2<TScalar>> B => _minkowskiSums[ 1 ];
	public MinkowskiDifference<Vector2<TScalar>> C => _minkowskiSums[ 2 ];

	internal bool HasIndexPair( int indexA, int indexB ) {
		for (int i = 0; i < _count; i++) {
			MinkowskiDifference<Vector2<TScalar>> minkowskiSum = _minkowskiSums[ i ];
			if (minkowskiSum.IndexShapeA == indexA && minkowskiSum.IndexShapeB == indexB || minkowskiSum.IndexShapeA == indexB && minkowskiSum.IndexShapeB == indexA)
				return true;
		}
		return false;
	}

	internal void Refresh( ConvexShapeBase<Vector2<TScalar>, TScalar> shapeA, ConvexShapeBase<Vector2<TScalar>, TScalar> shapeB ) {
		//TODO: this somehow fails? It might resolve non intersecting shapes as intersecting.
		for (int i = 0; i < _count; i++) {
			MinkowskiDifference<Vector2<TScalar>> minkowskiSum = _minkowskiSums[ i ];
			_minkowskiSums[ i ] = new MinkowskiDifference<Vector2<TScalar>>( shapeA.GetVertices()[ minkowskiSum.IndexShapeA ] - shapeB.GetVertices()[ minkowskiSum.IndexShapeB ], minkowskiSum.IndexShapeA, minkowskiSum.IndexShapeB );
		}
	}

	internal void Support( int indexA, Vector2<TScalar> a, int indexB, Vector2<TScalar> b ) {
		for (int i = _minkowskiSums.Length - 1; i >= 1; i--)
			_minkowskiSums[ i ] = _minkowskiSums[ i - 1 ];
		_minkowskiSums[ 0 ] = new MinkowskiDifference<Vector2<TScalar>>( a - b, indexA, indexB ); //needs to shift the others. Point A should always be last in the list, but point A is also the first one added.
		_count++;
	}

	internal void Remove( int index ) {
		for (int i = index; i < _minkowskiSums.Length - 1; i++)
			_minkowskiSums[ i ] = _minkowskiSums[ i + 1 ];
		_count--;
	}

	internal void Clear() {
		for (int i = 0; i < _count; i++)
			_minkowskiSums[ i ] = default;
		_count = 0;
	}
}
