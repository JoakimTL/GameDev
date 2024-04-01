using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder.Calculation;

/// <summary>
/// All methods that return <see cref="Vector3{T}"/> are implemented here."/>
/// </summary>
public sealed class Vector3Math<T> :
        ILinearMath<Vector3<T>, T>,
        IEntrywiseProduct<Vector3<T>>,
        IGeometricProduct<Bivector3<T>, Trivector3<T>, Vector3<T>>,
        IGeometricProduct<Trivector3<T>, Bivector3<T>, Vector3<T>>,
        IMatrixMultiplicationProduct<Vector3<T>, Matrix3x3<T>, Vector3<T>>
    where T :
        unmanaged, INumber<T>
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Negate(in Vector3<T> l) => new(-l.X, -l.Y, -l.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Add(in Vector3<T> l, in Vector3<T> r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Subtract(in Vector3<T> l, in Vector3<T> r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(in Vector3<T> l, T r) => new(l.X * r, l.Y * r, l.Z * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Divide(in Vector3<T> l, T r) => new(l.X / r, l.Y / r, l.Z / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> MultiplyEntrywise(in Vector3<T> l, in Vector3<T> r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> DivideEntrywise(in Vector3<T> l, in Vector3<T> r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(in Bivector3<T> l, in Trivector3<T> r) => new(-l.YZ * r.XYZ, -l.ZX * r.XYZ, -l.XY * r.XYZ);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(in Trivector3<T> l, in Bivector3<T> r) => new(-l.XYZ * r.YZ, -l.XYZ * r.ZX, -l.XYZ * r.XY);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(in Vector3<T> l, in Matrix3x3<T> r) => new(l.Dot(r.Col0), l.Dot(r.Col1), l.Dot(r.Col2));

}
