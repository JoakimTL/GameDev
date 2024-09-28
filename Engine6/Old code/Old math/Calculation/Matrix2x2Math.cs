using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.NewFolder.Calculation.Interfaces;
using Engine.Math.NewFolder.Operations;

namespace Engine.Math.NewFolder.Calculation;

public sealed class Matrix2x2Math<T> :
        ILinearMath<Matrix2x2<T>, T>,
        IEntrywiseProduct<Matrix2x2<T>>,
        IMatrixMultiplicationProduct<Matrix2x2<T>, Matrix2x2<T>, Matrix2x2<T>>,
        IMatrixDeterminant<Matrix2x2<T>, T>
    where T :
        unmanaged, INumber<T>
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Negate(in Matrix2x2<T> l)
        => new(
            -l.M00, -l.M01,
            -l.M10, -l.M11
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Add(in Matrix2x2<T> l, in Matrix2x2<T> r)
        => new(
            l.M00 + r.M00, l.M01 + r.M01,
            l.M10 + r.M10, l.M11 + r.M11
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Subtract(in Matrix2x2<T> l, in Matrix2x2<T> r)
        => new(
            l.M00 - r.M00, l.M01 - r.M01,
            l.M10 - r.M10, l.M11 - r.M11
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Divide(in Matrix2x2<T> l, T r)
        => new(
            l.M00 / r, l.M01 / r,
            l.M10 / r, l.M11 / r
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Multiply(in Matrix2x2<T> l, T r)
        => new(
            l.M00 * r, l.M01 * r,
            l.M10 * r, l.M11 * r
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> MultiplyEntrywise(in Matrix2x2<T> l, in Matrix2x2<T> r)
        => new(
            l.M00 * r.M00, l.M01 * r.M01,
            l.M10 * r.M10, l.M11 * r.M11
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> DivideEntrywise(in Matrix2x2<T> l, in Matrix2x2<T> r)
        => new(
            l.M00 / r.M00, l.M01 / r.M01,
            l.M10 / r.M10, l.M11 / r.M11
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2<T> Multiply(in Matrix2x2<T> l, in Matrix2x2<T> r)
        => new(
            l.Row0.Dot(r.Col0), l.Row0.Dot(r.Col1),
            l.Row1.Dot(r.Col0), l.Row1.Dot(r.Col1)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix2x2<T> m)
        => (m.M00 * m.M11) - (m.M01 * m.M10);
}
