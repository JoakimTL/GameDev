using Engine.Math.NewFolder.Calculation;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewFolder.Operations;

/// <summary>
/// Extension methods for <see cref="Trivector3{T}"/>. Return types may vary.
/// </summary>
public static class Trivector3Ops
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Negate<T>(in this Trivector3<T> l) where T : unmanaged, INumber<T> => Trivector3Math<T>.Negate(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Add<T>(in this Trivector3<T> l, in Trivector3<T> r) where T : unmanaged, INumber<T> => Trivector3Math<T>.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> Subtract<T>(in this Trivector3<T> l, in Trivector3<T> r) where T : unmanaged, INumber<T> => Trivector3Math<T>.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> ScalarMultiply<T>(in this Trivector3<T> l, T r) where T : unmanaged, INumber<T> => Trivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Trivector3<T> ScalarDivide<T>(in this Trivector3<T> l, T r) where T : unmanaged, INumber<T> => Trivector3Math<T>.Divide(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bivector3<T> Multiply<T>(in this Trivector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Bivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(in this Trivector3<T> l, in Trivector3<T> r) where T : unmanaged, INumber<T> => ScalarMath<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply<T>(in this Trivector3<T> l, in Bivector3<T> r) where T : unmanaged, INumber<T> => Vector3Math<T>.Multiply(l, r);
}