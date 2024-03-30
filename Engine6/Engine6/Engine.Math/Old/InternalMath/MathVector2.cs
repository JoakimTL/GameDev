using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Old.InternalMath;

internal static class MathVector2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2<T> Add<T>(in Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => new(l.X + r.X, l.Y + r.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2<T> Subtract<T>(in Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => new(l.X - r.X, l.Y - r.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2<T> HadamardMultiplication<T>(in Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => new(l.X * r.X, l.Y * r.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2<T> Multiply<T>(in Vector2<T> l, T r) where T : INumber<T>
        => new(l.X * r, l.Y * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2<T> HadamardDivision<T>(in Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => new(l.X / r.X, l.Y / r.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T Dot<T>(in Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => (l.X * r.X) + (l.Y * r.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Bivector2<T> Wedge<T>(in Vector2<T> l, in Vector2<T> r) where T : INumber<T>
        => new((l.X * r.Y) - (l.Y * r.X));
}
