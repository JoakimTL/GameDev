using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation;

namespace Engine.Math.NewFolder.Operations;

/// <summary>
/// Extension methods for <see cref="Vector3{T}"/>. Return types may vary.
/// </summary>
public static class Vector3Ops
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Negate<T>(in this Vector3<T> l) where T : unmanaged, INumber<T> => Vector3Math<T>.Negate(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Add<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Vector3Math<T>.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Subtract<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Vector3Math<T>.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ScalarMultiply<T>(in this Vector3<T> l, T r) where T : unmanaged, INumber<T> => Vector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ScalarDivide<T>(in this Vector3<T> l, T r) where T : unmanaged, INumber<T> => Vector3Math<T>.Divide(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> MultiplyEntrywise<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Vector3Math<T>.MultiplyEntrywise(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> DivideEntrywise<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Vector3Math<T>.DivideEntrywise(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rotor3<T> Multiply<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Rotor3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Vector3<T> l, in Bivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bivector3<T> Multiply<T>(in this Vector3<T> l, in Trivector3<T> r) where T : unmanaged, INumber<T> => Bivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Vector3<T> l, in Rotor3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Multivector3<T> Multiply<T>(in this Vector3<T> l, in Multivector3<T> r) where T : unmanaged, INumber<T> => Multivector3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bivector3<T> Wedge<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => Bivector3Math<T>.Wedge(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Cross<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => l.Wedge(r) * -Trivector3<T>.One;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T> => ScalarMath<T>.Dot(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(in this Vector3<T> l) where T : unmanaged, INumber<T> => l.Dot(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(in this Vector3<T> l) where T : unmanaged, IFloatingPointIeee754<T> => T.Sqrt(l.MagnitudeSquared());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Normalize<T>(in this Vector3<T> l) where T : unmanaged, IFloatingPointIeee754<T> => l.ScalarDivide(l.Magnitude());

    public static bool TryNormalize<T>(in this Vector3<T> l, out Vector3<T> result, out T originalMagnitude) where T : unmanaged, IFloatingPointIeee754<T>
    {
        originalMagnitude = l.Magnitude();
        if (originalMagnitude == T.Zero)
        {
            result = default;
            return false;
        }
        result = l.ScalarDivide(originalMagnitude);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Inverse<T>(in this Vector3<T> l) where T : unmanaged, INumber<T> => l.ScalarDivide(l.MagnitudeSquared());

    //[MethodImpl( MethodImplOptions.AggressiveInlining )]
    //public static Vector3<T> WorldTransform<T>( in this Vector3<T> v, in Matrix4x4<T> matrix ) where T : unmanaged, INumber<T> => ( v.WorldTransformVector * matrix ).GetTransformedVector3();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Floor<T>(in this Vector3<T> l) where T : unmanaged, IFloatingPointIeee754<T>
        => new(T.Floor(l.X), T.Floor(l.Y), T.Floor(l.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Ceiling<T>(in this Vector3<T> l) where T : unmanaged, IFloatingPointIeee754<T>
        => new(T.Ceiling(l.X), T.Ceiling(l.Y), T.Ceiling(l.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Round<T>(in this Vector3<T> l, int digits, MidpointRounding roundingMode) where T : unmanaged, IFloatingPointIeee754<T>
        => new(T.Round(l.X, digits, roundingMode), T.Round(l.Y, digits, roundingMode), T.Round(l.Z, digits, roundingMode));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Max<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T>
        => new(T.Max(l.X, r.X), T.Max(l.Y, r.Y), T.Max(l.Z, r.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Min<T>(in this Vector3<T> l, in Vector3<T> r) where T : unmanaged, INumber<T>
        => new(T.Min(l.X, r.X), T.Min(l.Y, r.Y), T.Min(l.Z, r.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SumOfParts<T>(in this Vector3<T> l) where T : unmanaged, INumber<T>
        => l.X + l.Y + l.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ProductOfParts<T>(in this Vector3<T> l) where T : unmanaged, INumber<T>
        => l.X * l.Y * l.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Inside<T>(in this Vector3<T> l, in AABB3<T> r) where T : unmanaged, INumber<T>
        => r.Minima.X <= l.X && r.Maxima.X >= l.X
        && r.Minima.Y <= l.Y && r.Maxima.Y >= l.Y
        && r.Minima.Z <= l.Z && r.Maxima.Z >= l.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegativeOrZero<T>(in this Vector3<T> l) where T : unmanaged, INumber<T>
        => l.X <= T.Zero || l.Y <= T.Zero || l.Z <= T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ReflectNormal<T>(in this Vector3<T> v, in Vector3<T> normal) where T : unmanaged, INumber<T>
        => normal.Multiply(v).Multiply(normal).Vector;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ReflectMirror<T>(in this Vector3<T> v, in Vector3<T> mirrorNormal) where T : unmanaged, INumber<T>
        => -mirrorNormal.Multiply(v).Multiply(mirrorNormal).Vector;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Rotate<T>(in this Vector3<T> v, in Rotor3<T> rotor) where T : unmanaged, INumber<T>
        => rotor.Multiply(v).Multiply(rotor.GetConjugate()).Vector;

    public static Vector3<T> NormalizeWithinSpace<T>(in this Vector3<T> l, T min, T max) where T : unmanaged, INumber<T>
    {
        T spaceInverse = T.MultiplicativeIdentity / (max - min);
        if (spaceInverse == T.Zero)
            throw new ArgumentException($"{nameof(min)} - {nameof(max)} is equal to 0, meaning the space can't be normalized.");
        return new((l.X - min) * spaceInverse, (l.Y - min) * spaceInverse, (l.Z - min) * spaceInverse);
    }
}
