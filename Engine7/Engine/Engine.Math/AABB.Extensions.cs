using System.Numerics;

namespace Engine;

public static partial class AABB {
	public static bool Intersects<TVector>( in this AABB<TVector> r, in AABB<TVector> l )
		where TVector :
			unmanaged, IEntrywiseMinMaxOperations<TVector>, IInEqualityOperators<TVector, TVector, bool>, ILinearAlgebraVectorOperations<TVector>, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> r.Maxima.Subtract( l.Minima ) >= TVector.Zero && l.Maxima.Subtract( r.Minima ) >= TVector.Zero;

	public static bool Contains<TVector>( in this AABB<TVector> r, in TVector l )
		where TVector :
			unmanaged, IEntrywiseMinMaxOperations<TVector>, IInEqualityOperators<TVector, TVector, bool>, ILinearAlgebraVectorOperations<TVector>, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> l >= r.Minima && l <= r.Maxima;

	public static TScalar GetArea<TScalar>( in this AABB<Vector2<TScalar>> aabb ) where TScalar : unmanaged, INumber<TScalar>
		=> (aabb.Maxima - aabb.Minima).ProductOfParts();
	public static TScalar GetSurfaceArea<TScalar>( in this AABB<Vector3<TScalar>> aabb ) where TScalar : unmanaged, INumber<TScalar>
		=> (aabb.Maxima - aabb.Minima).SumOfUnitBasisAreas() * (TScalar.One + TScalar.One);
	public static TScalar GetVolume<TScalar>( in this AABB<Vector3<TScalar>> aabb ) where TScalar : unmanaged, INumber<TScalar>
		=> (aabb.Maxima - aabb.Minima).ProductOfParts();
	public static TVector GetCenter<TVector>( in this AABB<TVector> aabb )
		where TVector :
			unmanaged, IEntrywiseMinMaxOperations<TVector>, IInEqualityOperators<TVector, TVector, bool>, ILinearAlgebraVectorOperations<TVector>, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>, IEntrywiseProductOperations<TVector>
		=> aabb.Minima.Add( aabb.Maxima.Subtract( aabb.Minima ).DivideEntrywise( TVector.Two ) );
	public static TVector GetLengths<TVector>( in this AABB<TVector> aabb )
		where TVector :
			unmanaged, ILinearAlgebraVectorOperations<TVector>, IEntrywiseMinMaxOperations<TVector>, IInEqualityOperators<TVector, TVector, bool>
		=> aabb.Maxima.Subtract( aabb.Minima );

	///// <param name="aabb">The bounds in which will contain all points on the plane will</param>
	///// <param name="stepSize">The step size between each point on the grid. This will be the length on a basis matrix formed by the plane rotation. X is right-left and Y is up-down in this basis.</param>
	///// <param name="planeOffset">Offset from the center of the bounds.</param>
	///// <param name="planeRotation">Rotation of the plane</param>
	///// <returns>A set of points which lie on a plane formed by the offset from the bound's center and the supplied rotation. All points lie within or on the border of the bounds and have a distance from eachother based on the provided stepsize.</returns>
	//public static IEnumerable<Vector3<TScalar>> GetPointsInBoundsOnPlaneInclusive<TScalar>( in this AABB<Vector3<TScalar>> aabb, Vector2<TScalar> stepSize, in Vector3<TScalar> planeOffset, in Rotor3<TScalar> planeRotation )
	//	where TScalar :
	//		unmanaged, INumber<TScalar> {
	//	//We need to find out how many steps we need to take in both the x and y direction.
	//	//Matrix.Create3x3.Basis( planeRotation, out Matrix3x3<TScalar> basis );
	//	for (TScalar y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += stepSize.Y)
	//		for (TScalar x = aabb.Minima.X; x <= aabb.Maxima.X; x += stepSize.X) {
	//		}
	//	return Enumerable.Empty<Vector3<TScalar>>();
	//}

	//public static IEnumerable<Vector2<T>> GetPointsInAreaExclusive( AABB2<T> aabb, T increment ) {
	//	for (T y = aabb.Minima.Y; y < aabb.Maxima.Y; y += increment)
	//		for (T x = aabb.Minima.X; x < aabb.Maxima.X; x += increment)
	//			yield return new( x, y );
	//}

	//public static IEnumerable<Vector2<T>> GetPointsInAreaInclusive( AABB2<T> aabb, T increment ) {

	//	for (T y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += increment)
	//		for (T x = aabb.Minima.X; x <= aabb.Maxima.X; x += increment)
	//			yield return new( x, y );
	//}

	//public static IEnumerable<Vector2<T>> GetPointsInAreaExclusiveExcept( AABB2<T> aabb, AABB2<T> other, T increment ) {
	//	for (T y = aabb.Minima.Y; y < aabb.Maxima.Y; y += increment)
	//		for (T x = aabb.Minima.X; x < aabb.Maxima.X; x += increment) {
	//			Vector2<T> v = new( x, y );
	//			if (v.Inside( other )) {
	//				x += other.Maxima.X - x;
	//				continue;
	//			}
	//			yield return v;
	//		}
	//}

	//public static IEnumerable<Vector2<T>> GetPointsInAreaIncusiveExcept( AABB2<T> aabb, AABB2<T> other, T increment ) {
	//	for (T y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += increment)
	//		for (T x = aabb.Minima.X; x <= aabb.Maxima.X; x += increment) {
	//			Vector2<T> v = new( x, y );
	//			if (v.Inside( other )) {
	//				x += other.Maxima.X - x;
	//				continue;
	//			}
	//			yield return v;
	//		}
	//}
}
