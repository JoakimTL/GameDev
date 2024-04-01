using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Old.InternalMath;

namespace Engine.Math.NewFolder.Old;

public static class Vector2Operations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Negate<T>(in this Vector2<T> v) where T : INumber<T>
        => new(-v.X, -v.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Add<T>(in this Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => MathVector2.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Subtract<T>(in this Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => MathVector2.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> ScalarMultiplication<T>(in this Vector2<T> l, T r) where T : INumber<T>
        => MathVector2.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> HadamardMultiplication<T>(in this Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => MathVector2.HadamardMultiplication(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> HadamardDivision<T>(in this Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => MathVector2.HadamardDivision(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> ScalarDivision<T>(in this Vector2<T> l, T r) where T : INumber<T>
        => l.ScalarMultiplication(T.MultiplicativeIdentity / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in this Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => MathVector2.Dot(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(in this Vector2<T> v) where T : INumber<T>
        => v.Dot(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(in this Vector2<T> v) where T : IFloatingPointIeee754<T>
        => T.Sqrt(v.MagnitudeSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Normalize<T>(in this Vector2<T> v) where T : IFloatingPointIeee754<T>
        => v.ScalarDivision(v.Magnitude());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> WorldTransform<T>(in this Vector2<T> v, in Matrix4x4<T> matrix) where T : INumber<T>
        => (v.WorldTransformVector * matrix).GetTransformedVector2();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> NormalTransform<T>(in this Vector2<T> v, in Matrix4x4<T> matrix) where T : INumber<T>
        => (v.NormalTransformVector * matrix).XY;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Multiply<T>(in this Vector2<T> l, in Matrix2x2<T> r) where T : INumber<T>
        => MathMatrix2x2.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(in this Vector2<T> l, in Vector2Column<T> r) where T : INumber<T>
        => l.Dot(r.AsRow);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Wedge<T>(in this Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => new Matrix2x2<T>(l, r).Determinant;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Multiply<T>(in this Vector2Column<T> l, in Vector2<T> r) where T : INumber<T>
        => MathMatrix2x2.Multiply(l, r);

}
