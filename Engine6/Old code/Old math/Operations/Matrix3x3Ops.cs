using Engine.Math.NewFolder.Calculation;
using Engine.Math.NewFolder.Operations.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.NewFolder.Operations;

/// <summary>
/// Extension methods for <see cref="Matrix3x3{T}"/>. Return types may vary.
/// </summary>
public sealed class Matrix3x3Ops<T> :
        ILinearAlgebraOperations<Matrix3x3<T>, T>,
        IEntrywiseOperations<Matrix3x3<T>>,
        IProductOperation<Matrix3x3<T>, Matrix3x3<T>, Matrix3x3<T>>,
        IMatrixOperations<Matrix3x3<T>, T>,
        IMatrixExpansionOperation<Matrix3x3<T>, Matrix4x4<T>>,
        IMatrixDataOperations<Matrix3x3<T>>
    where T :
        unmanaged, INumber<T>
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> Negate(in Matrix3x3<T> l) => Matrix3x3Math<T>.Negate(l);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> Add(in Matrix3x3<T> l, in Matrix3x3<T> r) => Matrix3x3Math<T>.Add(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> Subtract(in Matrix3x3<T> l, in Matrix3x3<T> r) => Matrix3x3Math<T>.Subtract(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> ScalarMultiply(in Matrix3x3<T> l, T r) => Matrix3x3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> ScalarDivide(in Matrix3x3<T> l, T r) => Matrix3x3Math<T>.Divide(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> MultiplyEntrywise(in Matrix3x3<T> l, in Matrix3x3<T> r) => Matrix3x3Math<T>.MultiplyEntrywise(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> DivideEntrywise(in Matrix3x3<T> l, in Matrix3x3<T> r) => Matrix3x3Math<T>.DivideEntrywise(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> Multiply(in Matrix3x3<T> l, in Matrix3x3<T> r) => Matrix3x3Math<T>.Multiply(l, r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetDeterminant(in Matrix3x3<T> m) => Matrix3x3Math<T>.GetDeterminant(m);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> GetTransposed(in Matrix3x3<T> m)
        => new(
            m.M00, m.M10, m.M20,
            m.M01, m.M11, m.M21,
            m.M02, m.M12, m.M22
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix3x3<T> GetInverse(in Matrix3x3<T> m)
        => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Expand(in Matrix3x3<T> m, out Matrix4x4<T> result)
        => result = new(
            m.M00, m.M01, m.M02, T.Zero,
            m.M10, m.M11, m.M12, T.Zero,
            m.M20, m.M21, m.M22, T.Zero,
            T.Zero, T.Zero, T.Zero, T.One
        );

    /// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
    public static bool TryFillRowMajor<TData>(in Matrix3x3<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0) where TData : unmanaged
    {
        unsafe
        {
            uint sizeStorageBytes = (uint)(resultStorage.Length * sizeof(T));
            uint expectedSizeBytes = (uint)sizeof(Matrix3x3<T>);
            if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
                return false;
            fixed (Matrix3x3<T>* srcPtr = &m)
            fixed (TData* dstPtr = resultStorage)
                Unsafe.CopyBlock((byte*)dstPtr + destinationOffsetBytes, srcPtr, expectedSizeBytes);
            return true;
        }
    }

    /// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
    public static bool TryFillColumnMajor<TData>(in Matrix3x3<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0) where TData : unmanaged
    {
        unsafe
        {
            uint sizeStorageBytes = (uint)(resultStorage.Length * sizeof(T));
            uint expectedSizeBytes = (uint)sizeof(Matrix3x3<T>);
            if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
                return false;
            Matrix3x3<T> transposed = GetTransposed(m);
            fixed (TData* dstPtr = resultStorage)
                Unsafe.CopyBlock((byte*)dstPtr + destinationOffsetBytes, &transposed, expectedSizeBytes);
            return true;
        }
    }
}
