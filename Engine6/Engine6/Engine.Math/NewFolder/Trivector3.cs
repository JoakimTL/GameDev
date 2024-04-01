using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

public readonly struct Trivector3<T>(T xyz) :
        ILinearOperators<Trivector3<T>, T>,
        IProductOperator<Trivector3<T>, Vector3<T>, Bivector3<T>>,
        IAdditiveIdentity<Trivector3<T>, Trivector3<T>>,
        IMultiplicativeIdentity<Trivector3<T>, Trivector3<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T XYZ = xyz;

    public Multivector3<T> GetMultivector() => new(T.AdditiveIdentity, Vector3<T>.AdditiveIdentity, Bivector3<T>.AdditiveIdentity, this);

    public static Trivector3<T> AdditiveIdentity => Zero;
    public static Trivector3<T> MultiplicativeIdentity => One;

    public static readonly Trivector3<T> Zero = new(T.Zero);
    public static readonly Trivector3<T> One = new(T.One);

    public static Trivector3<T> operator -(in Trivector3<T> l) => l.Negate();
    public static Trivector3<T> operator +(in Trivector3<T> l, in Trivector3<T> r) => l.Add(r);
    public static Trivector3<T> operator -(in Trivector3<T> l, in Trivector3<T> r) => l.Subtract(r);
    public static Trivector3<T> operator *(in Trivector3<T> l, T r) => l.ScalarMultiply(r);
    public static Trivector3<T> operator *(T l, in Trivector3<T> r) => r.ScalarMultiply(l);
    public static Trivector3<T> operator /(in Trivector3<T> l, T r) => l.ScalarDivide(r);
    public static Trivector3<T> operator *(in Trivector3<T> l, in Trivector3<T> r) => l.ScalarMultiply(r.XYZ);
    public static Trivector3<T> operator /(in Trivector3<T> l, in Trivector3<T> r) => l.ScalarDivide(r.XYZ);
    public static Bivector3<T> operator *(in Trivector3<T> l, in Vector3<T> r) => l.Multiply(r);
    public static implicit operator Trivector3<T>(T xyz) => new(xyz);

    public static bool operator ==(in Trivector3<T> l, in Trivector3<T> r) => l.XYZ == r.XYZ;
    public static bool operator !=(in Trivector3<T> l, in Trivector3<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Trivector3<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(XYZ);
    public override string ToString() => $"[{XYZ:N3}XYZ]";
}
