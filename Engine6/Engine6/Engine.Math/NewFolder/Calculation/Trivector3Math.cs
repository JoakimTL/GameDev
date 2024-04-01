using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation.Interfaces;

namespace Engine.Math.NewFolder.Calculation;

/// <summary>
/// All methods that return <see cref="Trivector3{T}"/> are implemented here."/>
/// </summary>
public sealed class Trivector3Math<T> :
        ILinearMath<Trivector3<T>, T>
    where T :
        unmanaged, INumber<T>
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Negate(in Trivector3<T> l) => new(-l.XYZ);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Add(in Trivector3<T> l, in Trivector3<T> r) => new(l.XYZ + r.XYZ);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Subtract(in Trivector3<T> l, in Trivector3<T> r) => new(l.XYZ - r.XYZ);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Multiply(in Trivector3<T> l, T r) => new(l.XYZ * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Divide(in Trivector3<T> l, T r) => new(l.XYZ / r);
}
