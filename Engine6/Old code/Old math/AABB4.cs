using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct AABB4<T>
    where T :
        unmanaged, INumber<T>
{
    public readonly Vector4<T> Minima;
    public readonly Vector4<T> Maxima;

    public AABB4(Vector4<T> a, Vector4<T> b)
    {
        Minima = a.Min(b);
        Maxima = a.Max(b);
    }
    public AABB4(AABB4<T> aabb, Vector4<T> v)
    {
        Minima = aabb.Minima.Min(v);
        Maxima = aabb.Maxima.Max(v);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AABB4<T> aabb && Equals(aabb);
    public override int GetHashCode() => HashCode.Combine(Minima, Maxima);
    public bool Equals(AABB4<T> other) => other.Minima == Minima && other.Maxima == Maxima;
    public static bool operator ==(AABB4<T> left, AABB4<T> right) => left.Equals(right);
    public static bool operator !=(AABB4<T> left, AABB4<T> right) => !left.Equals(right);
    public override string ToString() => $"{Minima} -> {Maxima}";
}

