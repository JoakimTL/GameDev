using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math.NewFolder;

public readonly struct Multivector4<T>(T scalar, in Vector4<T> vector, in Bivector4<T> bivector, in Trivector4<T> trivector, in Quadvector4<T> quadvector)
    where T :
        unmanaged, INumber<T>
{

    public readonly T Scalar = scalar;
    public readonly Vector4<T> Vector = vector;
    public readonly Bivector4<T> Bivector = bivector;
    public readonly Trivector4<T> Trivector = trivector;
    public readonly Quadvector4<T> Quadvector = quadvector;

    public static bool operator ==(in Multivector4<T> l, in Multivector4<T> r) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector && l.Trivector == r.Trivector && l.Quadvector == r.Quadvector;
    public static bool operator !=(in Multivector4<T> l, in Multivector4<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Multivector4<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(Scalar, Vector, Bivector, Trivector, Quadvector);
    public override string ToString() => $"[{Scalar:N3}+{Vector}+{Bivector}+{Trivector}+{Quadvector}]";
}
