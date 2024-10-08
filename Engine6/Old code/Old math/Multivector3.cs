﻿using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

public readonly struct Multivector3<T>(T scalar, in Vector3<T> vector, in Bivector3<T> bivector, in Trivector3<T> trivector) :
        ILinearOperators<Multivector3<T>, T>,
        IProductOperator<Multivector3<T>, Vector3<T>, Multivector3<T>>,
        IProductOperator<Multivector3<T>, Bivector3<T>, Multivector3<T>>,
        IProductOperator<Multivector3<T>, Trivector3<T>, Multivector3<T>>,
        IProductOperator<Multivector3<T>, Rotor3<T>, Multivector3<T>>,
        IProductOperator<Multivector3<T>, Multivector3<T>, Multivector3<T>>,
        IAdditiveIdentity<Multivector3<T>, Multivector3<T>>,
        IMultiplicativeIdentity<Multivector3<T>, Multivector3<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T Scalar = scalar;
    public readonly Vector3<T> Vector = vector;
    public readonly Bivector3<T> Bivector = bivector;
    public readonly Trivector3<T> Trivector = trivector;

    public Multivector3(T scalar, T vectorX, T vectorY, T vectorZ, T bivectorYZ, T bivectorZX, T bivectorXY, T trivectorXYZ) : this(scalar, new(vectorX, vectorY, vectorZ), new(bivectorYZ, bivectorZX, bivectorXY), new(trivectorXYZ)) { }

    public static Multivector3<T> AdditiveIdentity => Zero;
    public static Multivector3<T> MultiplicativeIdentity => One;

    public static readonly Multivector3<T> Zero = new(T.Zero, Vector3<T>.Zero, Bivector3<T>.Zero, T.Zero);
    public static readonly Multivector3<T> One = new(T.One, Vector3<T>.One, Bivector3<T>.One, T.One);

    public static Multivector3<T> operator -(in Multivector3<T> l) => l.Negate();
    public static Multivector3<T> operator -(in Multivector3<T> l, in Multivector3<T> r) => l.Subtract(r);
    public static Multivector3<T> operator +(in Multivector3<T> l, in Multivector3<T> r) => l.Add(r);
    public static Multivector3<T> operator *(in Multivector3<T> l, T r) => l.Multiply(r);
    public static Multivector3<T> operator *(T l, in Multivector3<T> r) => r.Multiply(l);
    public static Multivector3<T> operator /(in Multivector3<T> l, T r) => l.Divide(r);
    public static Multivector3<T> operator *(in Multivector3<T> l, in Vector3<T> r) => l.Multiply(r);
    public static Multivector3<T> operator *(in Multivector3<T> l, in Bivector3<T> r) => l.Multiply(r);
    public static Multivector3<T> operator *(in Multivector3<T> l, in Trivector3<T> r) => l.Multiply(r);
    public static Multivector3<T> operator *(in Multivector3<T> l, in Rotor3<T> r) => l.Multiply(r);
    public static Multivector3<T> operator *(in Multivector3<T> l, in Multivector3<T> r) => l.Multiply(r);

    public static bool operator ==(in Multivector3<T> l, in Multivector3<T> r) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector && l.Trivector == r.Trivector;
    public static bool operator !=(in Multivector3<T> l, in Multivector3<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Multivector3<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(Scalar, Vector, Bivector, Trivector);
    public override string ToString() => $"[{Scalar:#,##0.###}+{Vector}+{Bivector}+{Trivector}]";
}
