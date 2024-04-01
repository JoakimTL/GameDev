using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewVectors;

public static class AABBExtensions {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool Intersects<TScalar>( in this AABB<Vector2<TScalar>, TScalar> r, in AABB<Vector2<TScalar>, TScalar> l ) where TScalar : unmanaged, INumber<TScalar>
		=> r.Minima.X <= l.Maxima.X && r.Maxima.X >= l.Minima.X
		&& r.Minima.Y <= l.Maxima.Y && r.Maxima.Y >= l.Minima.Y;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool Intersects<TScalar>( in this AABB<Vector3<TScalar>, TScalar> r, in AABB<Vector3<TScalar>, TScalar> l ) where TScalar : unmanaged, INumber<TScalar>
		=> r.Minima.X <= l.Maxima.X && r.Maxima.X >= l.Minima.X
		&& r.Minima.Y <= l.Maxima.Y && r.Maxima.Y >= l.Minima.Y
		&& r.Minima.Z <= l.Maxima.Z && r.Maxima.Z >= l.Minima.Z;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool Intersects<TScalar>( in this AABB<Vector4<TScalar>, TScalar> r, in AABB<Vector4<TScalar>, TScalar> l ) where TScalar : unmanaged, INumber<TScalar>
		=> r.Minima.X <= l.Maxima.X && r.Maxima.X >= l.Minima.X
		&& r.Minima.Y <= l.Maxima.Y && r.Maxima.Y >= l.Minima.Y
		&& r.Minima.Z <= l.Maxima.Z && r.Maxima.Z >= l.Minima.Z
		&& r.Minima.W <= l.Maxima.W && r.Maxima.W >= l.Minima.W;


	public static TScalar GetArea<TScalar>( in this AABB<Vector2<TScalar>, TScalar> aabb ) where TScalar : unmanaged, INumber<TScalar>
		=> (aabb.Maxima - aabb.Minima).ProductOfParts();

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