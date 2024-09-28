using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Old.InternalMath;

namespace Engine.Math.NewFolder.Old;

public static class Vector4Operations
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Negate<T>(in this Vector4<T> v) where T : INumber<T>
        => new(-v.X, -v.Y, -v.Z, -v.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Add<T>(in this Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => MathVector4.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Subtract<T>(in this Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => MathVector4.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> ScalarMultiplication<T>(in this Vector4<T> l, T r) where T : INumber<T>
        => MathVector4.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> HadamardMultiplication<T>(in this Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => MathVector4.HadamardMultiplication(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> HadamardDivision<T>(in this Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => MathVector4.HadamardDivision(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> ScalarDivision<T>(in this Vector4<T> l, T r) where T : INumber<T>
        => l.ScalarMultiplication(T.MultiplicativeIdentity / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in this Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => MathVector4.Dot(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(in this Vector4<T> v) where T : INumber<T>
        => v.Dot(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(in this Vector4<T> v) where T : IFloatingPointIeee754<T>
        => T.Sqrt(v.MagnitudeSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Normalize<T>(in this Vector4<T> v) where T : IFloatingPointIeee754<T>
        => v.ScalarDivision(v.Magnitude());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Multiply<T>(in this Vector4<T> l, in Matrix4x4<T> r) where T : INumber<T>
        => MathMatrix4x4.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(in this Vector4<T> l, in Vector4Column<T> r) where T : INumber<T>
        => l.Dot(r.AsRow);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4<T> Multiply<T>(in this Vector4Column<T> l, in Vector4<T> r) where T : INumber<T>
        => MathMatrix4x4.Multiply(l, r);


}
