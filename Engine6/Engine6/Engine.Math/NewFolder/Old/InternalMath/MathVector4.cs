using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewFolder.Old.InternalMath;

public static class MathVector4
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4<T> Add<T>(in Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4<T> Subtract<T>(in Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z, l.W - r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4<T> HadamardMultiplication<T>(in Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z, l.W * r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4<T> Multiply<T>(in Vector4<T> l, T r) where T : INumber<T>
        => new(l.X * r, l.Y * r, l.Z * r, l.W * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4<T> HadamardDivision<T>(in Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z, l.W / r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T Dot<T>(in Vector4<T> l, in Vector4<T> r) where T : INumber<T>
        => (l.X * r.X) + (l.Y * r.Y) + (l.Z * r.Z) + (l.W * r.W);
}
