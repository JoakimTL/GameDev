using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

public readonly struct Vector4<T>(T x, T y, T z, T w) :
        ILinearOperators<Vector4<T>, T>,
        IVectorEntrywiseOperators<Vector4<T>, T>,
        IAdditiveIdentity<Vector4<T>, Vector4<T>>,
        IMultiplicativeIdentity<Vector4<T>, Vector4<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T X = x;
    public readonly T Y = y;
    public readonly T Z = z;
    public readonly T W = w;

    public Multivector4<T> GetMultivector() => new(T.Zero, this, Bivector4<T>.Zero, Trivector4<T>.Zero, T.Zero);

    public static Vector4<T> AdditiveIdentity => Zero;
    public static Vector4<T> MultiplicativeIdentity => One;

    public static readonly Vector4<T> Zero = new(T.Zero, T.Zero, T.Zero, T.Zero);
    public static readonly Vector4<T> One = new(T.One, T.One, T.One, T.One);

    public static Vector4<T> operator -(in Vector4<T> l) => l.Negate();
    public static Vector4<T> operator +(in Vector4<T> l, in Vector4<T> r) => l.Add(r);
    public static Vector4<T> operator -(in Vector4<T> l, in Vector4<T> r) => l.Subtract(r);
    public static Vector4<T> operator *(in Vector4<T> l, T r) => l.ScalarMultiply(r);
    public static Vector4<T> operator *(T l, in Vector4<T> r) => r.ScalarMultiply(l);
    public static Vector4<T> operator /(in Vector4<T> l, T r) => l.ScalarDivide(r);
    public static Vector4<T> operator *(in Vector4<T> l, in Vector4<T> r) => l.MultiplyEntrywise(r);
    public static Vector4<T> operator /(in Vector4<T> l, in Vector4<T> r) => l.DivideEntrywise(r);

    public static implicit operator Vector4<T>(T v) => new(v, v, v, v);
    public static implicit operator Vector4<T>((T x, T y, T z, T w) tuple) => new(tuple.x, tuple.y, tuple.z, tuple.w);

    public static bool operator ==(in Vector4<T> l, in Vector4<T> r) => l.X == r.X && l.Y == r.Y && l.Z == r.Z && l.W == r.W;
    public static bool operator !=(in Vector4<T> l, in Vector4<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Vector4<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
    public override string ToString() => $"[{X:0.###}X, {Y:0.###}Y, {Z:0.###}Z, {W:0.###}W]";
}
