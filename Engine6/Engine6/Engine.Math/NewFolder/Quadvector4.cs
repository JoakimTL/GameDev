using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math.NewFolder;

public readonly struct Quadvector4<T>(T xyzw) :
        IAdditiveIdentity<Quadvector4<T>, Quadvector4<T>>,
        IMultiplicativeIdentity<Quadvector4<T>, Quadvector4<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T XYZW = xyzw;

    public static Quadvector4<T> AdditiveIdentity => Zero;
    public static Quadvector4<T> MultiplicativeIdentity => One;

    public static readonly Quadvector4<T> Zero = new(T.Zero);
    public static readonly Quadvector4<T> One = new(T.One);

    public static implicit operator Quadvector4<T>(T xyzw) => new(xyzw);

    public static bool operator ==(in Quadvector4<T> l, in Quadvector4<T> r) => l.XYZW == r.XYZW;
    public static bool operator !=(in Quadvector4<T> l, in Quadvector4<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Quadvector4<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(XYZW);
    public override string ToString() => $"[{XYZW:N3}XYZW]";
}
