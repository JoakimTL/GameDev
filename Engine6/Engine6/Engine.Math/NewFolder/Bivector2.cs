using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Bivector2<T>(T xy) :
        ILinearOperators<Bivector2<T>, T>,
        IVectorEntrywiseOperators<Bivector2<T>, T>,
        IProductOperator<Bivector2<T>, Vector2<T>, Vector2<T>>,
        IAdditiveIdentity<Bivector2<T>, Bivector2<T>>,
        IMultiplicativeIdentity<Bivector2<T>, Bivector2<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T XY = xy;

    public Multivector2<T> GetMultivector() => new(T.Zero, Vector2<T>.Zero, this);

    public static Bivector2<T> AdditiveIdentity => Zero;
    public static Bivector2<T> MultiplicativeIdentity => One;

    public static readonly Bivector2<T> Zero = new(T.Zero);
    public static readonly Bivector2<T> One = new(T.One);

    public static Bivector2<T> operator -(in Bivector2<T> l) => l.Negate();
    public static Bivector2<T> operator +(in Bivector2<T> l, in Bivector2<T> r) => l.Add(r);
    public static Bivector2<T> operator -(in Bivector2<T> l, in Bivector2<T> r) => l.Subtract(r);
    public static Bivector2<T> operator *(in Bivector2<T> l, T r) => l.ScalarMultiply(r);
    public static Bivector2<T> operator *(T l, in Bivector2<T> r) => r.ScalarMultiply(l);
    public static Bivector2<T> operator /(in Bivector2<T> l, T r) => l.ScalarDivide(r);
    public static Bivector2<T> operator *(in Bivector2<T> l, in Bivector2<T> r) => l.ScalarMultiply(r.XY);
    public static Bivector2<T> operator /(in Bivector2<T> l, in Bivector2<T> r) => l.ScalarDivide(r.XY);
    public static Vector2<T> operator *(in Bivector2<T> l, in Vector2<T> r) => l.Multiply(r);

    public static implicit operator Bivector2<T>(T xy) => new(xy);

    public static bool operator ==(in Bivector2<T> l, in Bivector2<T> r) => l.XY == r.XY;
    public static bool operator !=(in Bivector2<T> l, in Bivector2<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Bivector2<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(XY);
    public override string ToString() => $"[{XY:#,##0.###}XY]";
}
