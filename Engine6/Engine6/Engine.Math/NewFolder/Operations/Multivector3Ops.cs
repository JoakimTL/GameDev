using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation;

namespace Engine.Math.NewFolder.Operations;

public static class Multivector3Ops
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Negate<T>(in this Multivector3<T> l) where T : unmanaged, INumber<T> => Multivector3Math<T>.Negate(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Add<T>(in this Multivector3<T> l, in Multivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Subtract<T>(in this Multivector3<T> l, in Multivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Multivector3<T> l, in T r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Divide<T>(in this Multivector3<T> l, in T r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Divide(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> MultiplyEntrywise<T>(in this Multivector3<T> l, in Multivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.MultiplyEntrywise(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> DivideEntrywise<T>(in this Multivector3<T> l, in Multivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.DivideEntrywise(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Multivector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Multivector3<T> l, in Bivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Multivector3<T> l, in Trivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Multivector3<T> l, in Rotor3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Multivector3<T> l, in Multivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);
}
