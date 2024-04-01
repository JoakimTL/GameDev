using Engine.Math.NewFolder;
using Engine.Math.NewVectors.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewVectors;

/// <summary>
/// Use <see cref="AABB.Create{TScalar}(Vector2{TScalar}, Vector2{TScalar})"/>, <see cref="AABB.Create{TScalar}(Vector3{TScalar}, Vector3{TScalar})"/>, or <see cref="AABB.Create{TScalar}(Vector4{TScalar}, Vector4{TScalar})"/> to create an instance.
/// </summary>
/// <typeparam name="TVector"></typeparam>
/// <typeparam name="TScalar"></typeparam>
[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct AABB<TVector, TScalar>
	where TVector :
		unmanaged, IVector<TVector, TScalar>, IEntrywiseMinMaxOperations<TVector>
	where TScalar :
		unmanaged, INumber<TScalar> {
	public readonly TVector Minima;
	public readonly TVector Maxima;

	public AABB( TVector a, TVector b ) {
		Minima = TVector.Min( a, b );
		Maxima = TVector.Max( a, b );
	}

	public AABB( AABB<TVector, TScalar> aabb, TVector v ) {
		Minima = TVector.Min( aabb.Minima, v );
		Maxima = TVector.Max( aabb.Maxima, v );
	}

	public static AABB<TVector, TScalar> Extend( in AABB<TVector, TScalar> aabb, in TVector v ) => new( aabb, v );
	public static AABB<TVector, TScalar> GetLargestBounds( in AABB<TVector, TScalar> r, in AABB<TVector, TScalar> l ) => new( TVector.Min( r.Minima, l.Minima ), TVector.Max( r.Maxima, l.Maxima ) );
	public static AABB<TVector, TScalar> GetSmallestBounds( in AABB<TVector, TScalar> r, in AABB<TVector, TScalar> l ) => new( TVector.Max( r.Minima, l.Minima ), TVector.Min( r.Maxima, l.Maxima ) );

	public static bool operator ==( AABB<TVector, TScalar> left, AABB<TVector, TScalar> right ) => left.Equals( right );
	public static bool operator !=( AABB<TVector, TScalar> left, AABB<TVector, TScalar> right ) => !(left == right);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is AABB<TVector, TScalar> aabb && aabb.Minima == Minima && aabb.Maxima == Maxima;
	public override int GetHashCode() => HashCode.Combine( Minima, Maxima );
	public override string ToString() => $">{Minima} -> {Maxima}|";
}

public static class AABB {
	public static AABB<Vector2<TScalar>, TScalar> Create<TScalar>( Vector2<TScalar> v1, Vector2<TScalar> v2 ) where TScalar : unmanaged, INumber<TScalar>
		=> new( v1, v2 );

	public static AABB<Vector3<TScalar>, TScalar> Create<TScalar>( Vector3<TScalar> v1, Vector3<TScalar> v2 ) where TScalar : unmanaged, INumber<TScalar>
		=> new( v1, v2 );

	public static AABB<Vector4<TScalar>, TScalar> Create<TScalar>( Vector4<TScalar> v1, Vector4<TScalar> v2 ) where TScalar : unmanaged, INumber<TScalar>
		=> new( v1, v2 );

	public static AABB<TVector, TScalar> Create<TVector, TScalar>( TVector v1, TVector v2 )
		where TVector :
			unmanaged, IVector<TVector, TScalar>, IEntrywiseMinMaxOperations<TVector> 
		where TScalar : 
			unmanaged, INumber<TScalar>
		=> new( v1, v2 );
}

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
		=> Vector2<TScalar>.ProductOfParts( aabb.Maxima - aabb.Minima);

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