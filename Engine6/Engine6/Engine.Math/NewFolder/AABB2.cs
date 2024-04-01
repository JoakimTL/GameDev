using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct AABB2<T>
    where T :
        unmanaged, INumber<T>
{
    public readonly Vector2<T> Minima;
    public readonly Vector2<T> Maxima;

    public AABB2(Vector2<T> a, Vector2<T> b)
    {
        Minima = a.Min(b);
        Maxima = a.Max(b);
    }
    public AABB2(AABB2<T> aabb, Vector2<T> v)
    {
        Minima = aabb.Minima.Min(v);
        Maxima = aabb.Maxima.Max(v);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AABB2<T> aabb && Equals(aabb);
    public override int GetHashCode() => HashCode.Combine(Minima, Maxima);
    public bool Equals(AABB2<T> other) => other.Minima == Minima && other.Maxima == Maxima;
    public static bool operator ==(AABB2<T> left, AABB2<T> right) => left.Equals(right);
    public static bool operator !=(AABB2<T> left, AABB2<T> right) => !left.Equals(right);
    public override string ToString() => $"{Minima} -> {Maxima}";
}

