using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder.Calculation;

/// <summary>
/// All methods that return <see cref="Vector4{T}"/> are implemented here."/>
/// </summary>
public sealed class Vector4Math<T> :
        ILinearMath<Vector4<T>, T>,
        IEntrywiseProduct<Vector4<T>>,
        IMatrixMultiplicationProduct<Vector4<T>, Matrix4x4<T>, Vector4<T>>
    where T :
        unmanaged, INumber<T>
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Negate(in Vector4<T> l) => new(-l.X, -l.Y, -l.Z, -l.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Add(in Vector4<T> l, in Vector4<T> r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Subtract(in Vector4<T> l, in Vector4<T> r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z, l.W - r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Multiply(in Vector4<T> l, T r) => new(l.X * r, l.Y * r, l.Z * r, l.W * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Divide(in Vector4<T> l, T r) => new(l.X / r, l.Y / r, l.Z / r, l.W / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> MultiplyEntrywise(in Vector4<T> l, in Vector4<T> r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z, l.W * r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> DivideEntrywise(in Vector4<T> l, in Vector4<T> r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z, l.W / r.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Multiply(in Vector4<T> l, in Matrix4x4<T> r) => new(l.Dot(r.Col0), l.Dot(r.Col1), l.Dot(r.Col2), l.Dot(r.Col3));
}
