using System.Diagnostics.CodeAnalysis;

namespace Math.GeometricAlgebra;

/// <summary>
/// Use <see cref="AABB.Create{TScalar}(Vector2{TScalar}, Vector2{TScalar})"/>, <see cref="AABB.Create{TScalar}(Vector3{TScalar}, Vector3{TScalar})"/>, or <see cref="AABB.Create{TScalar}(Vector4{TScalar}, Vector4{TScalar})"/> to create an instance.
/// </summary>
/// <typeparam name="TVector"></typeparam>
/// <typeparam name="TScalar"></typeparam>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct AABB<TVector>
	where TVector :
		unmanaged, IInEqualityOperators<TVector, TVector, bool>, IEntrywiseMinMaxOperations<TVector>
{
	public readonly TVector Minima;
	public readonly TVector Maxima;

	public AABB(TVector a, TVector b)
	{
		Minima = a.Min(b);
		Maxima = a.Max(b);
	}

	public AABB(AABB<TVector> aabb, TVector v)
	{
		Minima = v.Min(aabb.Minima);
		Maxima = v.Max(aabb.Maxima);
	}

	public AABB<TVector> Extend(in TVector v) => new(this, v);
	public AABB<TVector> GetLargestBounds(in AABB<TVector> l) => new(Minima.Min(l.Minima), Maxima.Max(l.Maxima));
	public AABB<TVector> GetSmallestBounds(in AABB<TVector> l) => new(Minima.Max(l.Minima), Maxima.Min(l.Maxima));

	public static bool operator ==(AABB<TVector> left, AABB<TVector> right) => left.Equals(right);
	public static bool operator !=(AABB<TVector> left, AABB<TVector> right) => !(left == right);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is AABB<TVector> aabb && aabb.Minima == Minima && aabb.Maxima == Maxima;
	public override int GetHashCode() => HashCode.Combine(Minima, Maxima);
	public override string ToString()
		=> $"<{Minima} -> {Maxima}>";
}
