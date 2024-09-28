using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

public readonly struct Multivector2<T>(T scalar, in Vector2<T> vector, in Bivector2<T> bivector) :
        ILinearOperators<Multivector2<T>, T>,
        IProductOperator<Multivector2<T>, Vector2<T>, Multivector2<T>>,
        IProductOperator<Multivector2<T>, Bivector2<T>, Multivector2<T>>,
        IProductOperator<Multivector2<T>, Rotor2<T>, Multivector2<T>>,
        IProductOperator<Multivector2<T>, Multivector2<T>, Multivector2<T>>,
        IAdditiveIdentity<Multivector2<T>, Multivector2<T>>,
        IMultiplicativeIdentity<Multivector2<T>, Multivector2<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T Scalar = scalar;
    public readonly Vector2<T> Vector = vector;
    public readonly Bivector2<T> Bivector = bivector;

    public Multivector2(T scalar, T vectorX, T vectorY, T bivectorXY) : this(scalar, new(vectorX, vectorY), new(bivectorXY)) { }

    public static Multivector2<T> AdditiveIdentity => Zero;
    public static Multivector2<T> MultiplicativeIdentity => One;

    public static readonly Multivector2<T> Zero = new(T.Zero, Vector2<T>.Zero, T.Zero);
    public static readonly Multivector2<T> One = new(T.One, Vector2<T>.One, T.One);

    public static Multivector2<T> operator -(in Multivector2<T> l) => l.Negate();
    public static Multivector2<T> operator -(in Multivector2<T> l, in Multivector2<T> r) => l.Subtract(r);
    public static Multivector2<T> operator +(in Multivector2<T> l, in Multivector2<T> r) => l.Add(r);
    public static Multivector2<T> operator *(in Multivector2<T> l, T r) => l.Multiply(r);
    public static Multivector2<T> operator *(T l, in Multivector2<T> r) => r.Multiply(l);
    public static Multivector2<T> operator /(in Multivector2<T> l, T r) => l.Divide(r);
    public static Multivector2<T> operator *(in Multivector2<T> l, in Vector2<T> r) => l.Multiply(r);
    public static Multivector2<T> operator *(in Multivector2<T> l, in Bivector2<T> r) => l.Multiply(r);
    public static Multivector2<T> operator *(in Multivector2<T> l, in Rotor2<T> r) => l.Multiply(r);
    public static Multivector2<T> operator *(in Multivector2<T> l, in Multivector2<T> r) => l.Multiply(r);

    public static bool operator ==(in Multivector2<T> l, in Multivector2<T> r) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector;
    public static bool operator !=(in Multivector2<T> l, in Multivector2<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Multivector2<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(Scalar, Vector, Bivector);
    public override string ToString() => $"[{Scalar:#,##0.###}+{Vector}+{Bivector}]";
}
