using Engine.Math.NewFolder;
using Engine.Math.NewVectors.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

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
		Minima = a.Min( b );
		Maxima = a.Max( b );
	}

	public AABB( AABB<TVector, TScalar> aabb, TVector v ) {
		Minima = v.Min( aabb.Minima );
		Maxima = v.Max( aabb.Maxima );
	}

	public static AABB<TVector, TScalar> Extend( in AABB<TVector, TScalar> aabb, in TVector v ) => new( aabb, v );
	public static AABB<TVector, TScalar> GetLargestBounds( in AABB<TVector, TScalar> r, in AABB<TVector, TScalar> l ) => new( r.Minima.Min( l.Minima ), r.Maxima.Max( l.Maxima ) );
	public static AABB<TVector, TScalar> GetSmallestBounds( in AABB<TVector, TScalar> r, in AABB<TVector, TScalar> l ) => new( r.Minima.Max( l.Minima ), r.Maxima.Min( l.Maxima ) );

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
