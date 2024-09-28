using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Old.InternalMath;

namespace Engine.Math.NewFolder.Old;

public static class Vector3Operations
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Negate<T>(in this Vector3<T> v) where T : INumber<T>
        => new(-v.X, -v.Y, -v.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Add<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Subtract<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ScalarMultiplication<T>(in this Vector3<T> l, T r) where T : INumber<T>
        => MathVector3.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Multiply<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> HadamardMultiplication<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.HadamardMultiplication(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> HadamardDivision<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.HadamardDivision(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ScalarDivision<T>(in this Vector3<T> l, T r) where T : INumber<T>
        => l.ScalarMultiplication(T.MultiplicativeIdentity / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.Dot(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bivector3<T> Wedge<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => MathVector3.Wedge(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> GetPerpendicular<T>(in this Vector3<T> l, in Vector3<T> r) where T : INumber<T>
        => l.Wedge(r).AsVector;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(in this Vector3<T> v) where T : INumber<T>
        => v.Dot(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(in this Vector3<T> v) where T : IFloatingPointIeee754<T>
        => T.Sqrt(v.MagnitudeSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Normalize<T>(in this Vector3<T> v) where T : IFloatingPointIeee754<T>
        => v.ScalarDivision(v.Magnitude());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> WorldTransform<T>(in this Vector3<T> v, in Matrix4x4<T> matrix) where T : INumber<T>
        => (v.WorldTransformVector * matrix).GetTransformedVector3();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> NormalTransform<T>(in this Vector3<T> v, in Matrix4x4<T> matrix) where T : INumber<T>
        => (v.NormalTransformVector * matrix).XYZ;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply<T>(in this Vector3<T> l, in Matrix3x3<T> r) where T : INumber<T>
        => MathMatrix3x3.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Multiply<T>(in this Vector3<T> l, in Vector3Column<T> r) where T : INumber<T>
        => l.Dot(r.AsRow);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> Multiply<T>(in this Vector3Column<T> l, in Vector3<T> r) where T : INumber<T>
        => MathMatrix3x3.Multiply(l, r);
}