using System.Diagnostics.CodeAnalysis;

namespace Engine;

/// <summary>
/// Use <see cref="AABB.Create{TScalar}(Vector2{TScalar}, Vector2{TScalar})"/>, <see cref="AABB.Create{TScalar}(Vector3{TScalar}, Vector3{TScalar})"/>, or <see cref="AABB.Create{TScalar}(Vector4{TScalar}, Vector4{TScalar})"/> to create an instance.
/// </summary>
/// <typeparam name="TVector"></typeparam>
/// <typeparam name="TScalar"></typeparam>
[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct AABB<TVector>
	where TVector :
		unmanaged, IInEqualityOperators<TVector, TVector, bool>, IEntrywiseMinMaxOperations<TVector> {
	public readonly TVector Minima;
	public readonly TVector Maxima;

	public AABB( TVector a, TVector b ) {
		this.Minima = a.Min( b );
		this.Maxima = a.Max( b );
	}

	public AABB( AABB<TVector> aabb, TVector v ) {
		this.Minima = v.Min( aabb.Minima );
		this.Maxima = v.Max( aabb.Maxima );
	}

	public AABB<TVector> Extend( in TVector v ) => new( this, v );
	public AABB<TVector> GetLargestBounds( in AABB<TVector> l ) => new( this.Minima.Min( l.Minima ), this.Maxima.Max( l.Maxima ) );
	public AABB<TVector> GetSmallestBounds( in AABB<TVector> l ) => new( this.Minima.Max( l.Minima ), this.Maxima.Min( l.Maxima ) );

	public static bool operator ==( AABB<TVector> left, AABB<TVector> right ) => left.Equals( right );
	public static bool operator !=( AABB<TVector> left, AABB<TVector> right ) => !(left == right);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is AABB<TVector> aabb && aabb.Minima == this.Minima && aabb.Maxima == this.Maxima;
	public override int GetHashCode() => HashCode.Combine( this.Minima, this.Maxima );
	public override string ToString()
		=> $"<{this.Minima} -> {this.Maxima}>";
}
