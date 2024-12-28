namespace Engine.Physics;

public readonly struct MinkowskiDifference<TVector>( TVector sum, int indexShapeA, int indexShapeB ) {
	public TVector Sum { get; } = sum;
	public int IndexShapeA { get; } = indexShapeA;
	public int IndexShapeB { get; } = indexShapeB;
}
