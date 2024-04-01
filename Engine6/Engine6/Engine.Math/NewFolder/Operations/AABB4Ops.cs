using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewFolder.Operations;

public static class AABB4Ops
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB4<T> Extend<T>(in this AABB4<T> aabb, in Vector4<T> v) where T : unmanaged, INumber<T> => new(aabb, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects<T>(in this AABB4<T> r, in AABB4<T> l) where T : unmanaged, INumber<T>
        => r.Minima.X <= l.Maxima.X && r.Maxima.X >= l.Minima.X
        && r.Minima.Y <= l.Maxima.Y && r.Maxima.Y >= l.Minima.Y
        && r.Minima.Z <= l.Maxima.Z && r.Maxima.Z >= l.Minima.Z
        && r.Minima.W <= l.Maxima.W && r.Maxima.W >= l.Minima.W;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB4<T> GetLargestBounds<T>(in this AABB4<T> r, in AABB4<T> l) where T : unmanaged, INumber<T> => new(r.Minima.Min(l.Minima), r.Maxima.Max(l.Maxima));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB4<T> GetSmallestBounds<T>(in this AABB4<T> r, in AABB4<T> l) where T : unmanaged, INumber<T> => new(r.Minima.Max(l.Minima), r.Maxima.Min(l.Maxima));
}

