using Engine.Math.NewVectors.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewVectors;

public static partial class AABB {
	public static bool Intersects<TVector>( in this AABB<TVector> r, in AABB<TVector> l )
		where TVector :
			unmanaged, IEntrywiseMinMaxOperations<TVector>, IInEqualityOperators<TVector, TVector, bool>, ILinearAlgebraVectorOperations<TVector>, IEntrywiseComparisonOperators<TVector>, IVectorIdentities<TVector>
		=> r.Maxima.Subtract( l.Minima ) >= TVector.Zero && l.Maxima.Subtract( r.Minima ) >= TVector.Zero;

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

	//public static IEnumerable<TVector> GetPointsInBoundsOnPlane<TRotor, TVector, TScalar>( in this AABB<TVector, TScalar> aabb, TVector planeOrigin, TRotor planeRotation )
	//	where TRotor :
	//		unmanaged, IVector<TRotor, TScalar>
	//	where TVector :
	//		unmanaged, IVector<TVector, TScalar>, IEntrywiseMinMaxOperations<TVector>
	//	where TScalar :
	//		unmanaged, INumber<TScalar> {

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
