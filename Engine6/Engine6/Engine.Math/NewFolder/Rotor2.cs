using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

public readonly struct Rotor2<T>(T scalar, in Bivector2<T> bivector) :
        ILinearOperators<Rotor2<T>, T>,
        IVectorEntrywiseOperators<Rotor2<T>, T>,
        IMultivectorPart<Multivector2<T>>,
        IProductOperator<Rotor2<T>, Vector2<T>, Vector2<T>>,
        IProductOperator<Rotor2<T>, Multivector2<T>, Multivector2<T>>,
        IAdditiveIdentity<Rotor2<T>, Rotor2<T>>,
        IMultiplicativeIdentity<Rotor2<T>, Rotor2<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T Scalar = scalar;
    public readonly Bivector2<T> Bivector = bivector;

    public Multivector2<T> GetMultivector() => new(Scalar, Vector2<T>.Zero, Bivector);

    public static Rotor2<T> AdditiveIdentity => new(T.Zero, Bivector2<T>.Zero);
    public static Rotor2<T> MultiplicativeIdentity => new(T.Zero, Bivector2<T>.One);

    public static Rotor2<T> operator -(in Rotor2<T> l) => l.Negate();
    public static Rotor2<T> operator +(in Rotor2<T> l, in Rotor2<T> r) => l.Add(r);
    public static Rotor2<T> operator -(in Rotor2<T> l, in Rotor2<T> r) => l.Subtract(r);
    public static Rotor2<T> operator *(in Rotor2<T> l, T r) => l.ScalarMultiply(r);
    public static Rotor2<T> operator *(T l, in Rotor2<T> r) => r.ScalarMultiply(l);
    public static Rotor2<T> operator /(in Rotor2<T> l, T r) => l.ScalarDivide(r);
    public static Rotor2<T> operator *(in Rotor2<T> l, in Rotor2<T> r) => l.MultiplyEntrywise(r);
    public static Rotor2<T> operator /(in Rotor2<T> l, in Rotor2<T> r) => l.DivideEntrywise(r);
    public static Vector2<T> operator *(in Rotor2<T> l, in Vector2<T> r) => l.Multiply(r);
    public static Multivector2<T> operator *(in Rotor2<T> l, in Multivector2<T> r) => l.Multiply(r);

    public static bool operator ==(in Rotor2<T> l, in Rotor2<T> r) => l.Scalar == r.Scalar && l.Bivector == r.Bivector;
    public static bool operator !=(in Rotor2<T> l, in Rotor2<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Rotor2<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(Scalar, Bivector);
    public override string ToString() => $"[{Scalar:N3} + {Bivector}]";
}
