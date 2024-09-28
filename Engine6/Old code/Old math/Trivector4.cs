using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math.NewFolder;

public readonly struct Trivector4<T>(T xyz, T xyw, T xzw, T yzw) :
        IAdditiveIdentity<Trivector4<T>, Trivector4<T>>,
        IMultiplicativeIdentity<Trivector4<T>, Trivector4<T>>
    where T :
        unmanaged, INumber<T>
{

    public readonly T XYZ = xyz;
    public readonly T XYW = xyw;
    public readonly T XZW = xzw;
    public readonly T YZW = yzw;

    public static Trivector4<T> AdditiveIdentity => Zero;
    public static Trivector4<T> MultiplicativeIdentity => One;

    public static readonly Trivector4<T> Zero = new(T.Zero, T.Zero, T.Zero, T.Zero);
    public static readonly Trivector4<T> One = new(T.One, T.One, T.One, T.One);

    public static bool operator ==(in Trivector4<T> l, in Trivector4<T> r) => l.XYZ == r.XYZ && l.XYW == r.XYW && l.XZW == r.XZW && l.YZW == r.YZW;
    public static bool operator !=(in Trivector4<T> l, in Trivector4<T> r) => !(l == r);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Trivector4<T> v && this == v;
    public override int GetHashCode() => HashCode.Combine(XYZ, XYW, XZW, YZW);
    public override string ToString() => $"[{XYZ:#,##0.###}XYZ, {XYW:#,##0.###}XYW, {XZW:#,##0.###}XZW, {YZW:#,##0.###}YZW]";
}
