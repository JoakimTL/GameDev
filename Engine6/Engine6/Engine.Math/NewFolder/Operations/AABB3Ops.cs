using Engine.Math.NewFolder.Operations.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewFolder.Operations;

public sealed class AABB3Ops<T> :
        IAABBOperations<AABB3<T>, Vector3<T>>,
        IAABBVolumeOperations<AABB3<T>, Rotor3<T>, Vector3<T>, T>
    where T :
        unmanaged, INumber<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB3<T> Extend(in AABB3<T> aabb, in Vector3<T> v) => new(aabb, v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(in AABB3<T> r, in AABB3<T> l)
        => r.Minima.X <= l.Maxima.X && r.Maxima.X >= l.Minima.X
        && r.Minima.Y <= l.Maxima.Y && r.Maxima.Y >= l.Minima.Y
        && r.Minima.Z <= l.Maxima.Z && r.Maxima.Z >= l.Minima.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB3<T> GetLargestBounds(in AABB3<T> r, in AABB3<T> l) => new(r.Minima.Min(l.Minima), r.Maxima.Max(l.Maxima));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AABB3<T> GetSmallestBounds(in AABB3<T> r, in AABB3<T> l) => new(r.Minima.Max(l.Minima), r.Maxima.Min(l.Maxima));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetSurfaceArea(in AABB3<T> aabb)
    {
        Vector3<T> d = aabb.Maxima - aabb.Minima;
        T two = T.One + T.One;
        return
              (d.X * d.Y * two)
            + (d.X * d.Z * two)
            + (d.Y * d.Z * two);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetVolume(in AABB3<T> aabb) => (aabb.Maxima - aabb.Minima).ProductOfParts();

    public static IEnumerable<Vector3<T>> GetPointsInAreaExclusive(AABB3<T> aabb, T increment, Rotor3<T> planeRotor, Vector3<T> basisFromMinima)
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<Vector3<T>> GetPointsInAreaInclusive(AABB3<T> aabb, T increment, Rotor3<T> planeRotor, Vector3<T> basisFromMinima)
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<Vector3<T>> GetPointsInAreaExclusiveExcept(AABB3<T> aabb, AABB3<T> other, T increment, Rotor3<T> planeRotor, Vector3<T> basisFromMinima)
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<Vector3<T>> GetPointsInAreaIncusiveExcept(AABB3<T> aabb, AABB3<T> other, T increment, Rotor3<T> planeRotor, Vector3<T> basisFromMinima)
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<Vector3<T>> GetPointsInVolumeExclusive(AABB3<T> aabb, T increment)
    {
        for (T z = aabb.Minima.Z; z < aabb.Maxima.Z; z += increment)
            for (T y = aabb.Minima.Y; y < aabb.Maxima.Y; y += increment)
                for (T x = aabb.Minima.X; x < aabb.Maxima.X; x += increment)
                    yield return new(x, y, z);
    }

    public static IEnumerable<Vector3<T>> GetPointsInVolumeInclusive(AABB3<T> aabb, T increment)
    {
        for (T z = aabb.Minima.Z; z <= aabb.Maxima.Z; z += increment)
            for (T y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += increment)
                for (T x = aabb.Minima.X; x <= aabb.Maxima.X; x += increment)
                    yield return new(x, y, z);
    }

    public static IEnumerable<Vector3<T>> GetPointsInVolumeExclusiveExcept(AABB3<T> aabb, AABB3<T> other, T increment)
    {
        for (T z = aabb.Minima.Z; z < aabb.Maxima.Z; z += increment)
            for (T y = aabb.Minima.Y; y < aabb.Maxima.Y; y += increment)
                for (T x = aabb.Minima.X; x < aabb.Maxima.X; x += increment)
                {
                    Vector3<T> v = new(x, y, z);
                    if (v.Inside(other))
                    {
                        x += other.Maxima.X - x;
                        continue;
                    }
                    yield return v;
                }
    }

    public static IEnumerable<Vector3<T>> GetPointsInVolumeIncusiveExcept(AABB3<T> aabb, AABB3<T> other, T increment)
    {
        for (T z = aabb.Minima.Z; z <= aabb.Maxima.Z; z += increment)
            for (T y = aabb.Minima.Y; y <= aabb.Maxima.Y; y += increment)
                for (T x = aabb.Minima.X; x <= aabb.Maxima.X; x += increment)
                {
                    Vector3<T> v = new(x, y, z);
                    if (v.Inside(other))
                    {
                        x += other.Maxima.X - x;
                        continue;
                    }
                    yield return v;
                }
    }
}

