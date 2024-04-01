using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Operations.Interfaces;

namespace Engine.Math.NewFolder.Operations;

public class AABB2Ops<T> :
        IAABBOperations<AABB2<T>, Vector2<T>>,
        IAABBAreaOperations<AABB2<T>, Vector2<T>, T>
    where T :
        unmanaged, INumber<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB2<T> Extend(in AABB2<T> aabb, in Vector2<T> v) => new(aabb, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in AABB2<T> r, in AABB2<T> l) => r.Minima.X <= l.Maxima.X && r.Maxima.X >= l.Minima.X && r.Minima.Y <= l.Maxima.Y && r.Maxima.Y >= l.Minima.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB2<T> GetLargestBounds(in AABB2<T> r, in AABB2<T> l) => new(r.Minima.Min(l.Minima), r.Maxima.Max(l.Maxima));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB2<T> GetSmallestBounds(in AABB2<T> r, in AABB2<T> l) => new(r.Minima.Max(l.Minima), r.Maxima.Min(l.Maxima));

    public static T GetArea(in AABB2<T> aabb) => (aabb.Maxima - aabb.Minima).ProductOfParts();

    public static IEnumerable<Vector2<T>> GetPointsInAreaExclusive(AABB2<T> aabb, T increment)
    {
        for (T y = aabb.Minima.Y; y < aabb.Maxima.Y; y += increment)
            for (T x = aabb.Minima.X; x < aabb.Maxima.X; x += increment)
                yield return new(x, y);
    }

    public static IEnumerable<Vector2<T>> GetPointsInAreaInclusive(AABB2<T> aabb, T increment)
    {

        for (T y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += increment)
            for (T x = aabb.Minima.X; x <= aabb.Maxima.X; x += increment)
                yield return new(x, y);
    }

    public static IEnumerable<Vector2<T>> GetPointsInAreaExclusiveExcept(AABB2<T> aabb, AABB2<T> other, T increment)
    {
        for (T y = aabb.Minima.Y; y < aabb.Maxima.Y; y += increment)
            for (T x = aabb.Minima.X; x < aabb.Maxima.X; x += increment)
            {
                Vector2<T> v = new(x, y);
                if (v.Inside(other))
                {
                    x += other.Maxima.X - x;
                    continue;
                }
                yield return v;
            }
    }

    public static IEnumerable<Vector2<T>> GetPointsInAreaIncusiveExcept(AABB2<T> aabb, AABB2<T> other, T increment)
    {
        for (T y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += increment)
            for (T x = aabb.Minima.X; x <= aabb.Maxima.X; x += increment)
            {
                Vector2<T> v = new(x, y);
                if (v.Inside(other))
                {
                    x += other.Maxima.X - x;
                    continue;
                }
                yield return v;
            }
    }
}

