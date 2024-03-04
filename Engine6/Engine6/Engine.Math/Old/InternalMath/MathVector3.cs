using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Old;

namespace Engine.Math.Old.InternalMath;

public static class MathVector3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3<T> Add<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3<T> Subtract<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Quaternion<T> Multiply<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(Wedge(l, r), Dot(l, r));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3<T> HadamardMultiplication<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3<T> Multiply<T>(in Vector3<T> l, T r) where T : INumberBase<T>
        => new(l.X * r, l.Y * r, l.Z * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3<T> HadamardDivision<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T Dot<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => l.X * r.X + l.Y * r.Y + l.Z * r.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Bivector3<T> Wedge<T>(in Vector3<T> l, in Vector3<T> r) where T : INumberBase<T>
        => new(l.Y * r.Z - l.Z * r.Y, l.Z * r.X - l.X * r.Z, l.X * r.Y - l.Y * r.X);
}