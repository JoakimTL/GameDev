using Engine.Math.Calculation;
using Engine.Math.Old;
using Engine.Math.Operations.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Matrix2x2{T}"/>. Return types may vary.
/// </summary>
public sealed class Matrix2x2Ops<T> :
		ILinearAlgebraOperations<Matrix2x2<T>, T>,
		IEntrywiseOperations<Matrix2x2<T>>,
		IProductOperation<Matrix2x2<T>, Matrix2x2<T>, Matrix2x2<T>>,
		IMatrixOperations<Matrix2x2<T>, T>,
		IMatrixExpansionOperation<Matrix2x2<T>, Matrix3x3<T>>,
		IMatrixExpansionOperation<Matrix2x2<T>, Matrix4x4<T>>,
		IMatrixDataOperations<Matrix2x2<T>>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Negate( in Matrix2x2<T> l )
		=> Matrix2x2Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Add( in Matrix2x2<T> l, in Matrix2x2<T> r )
		=> Matrix2x2Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Subtract( in Matrix2x2<T> l, in Matrix2x2<T> r )
		=> Matrix2x2Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> ScalarMultiply( in Matrix2x2<T> l, T r )
		=> Matrix2x2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> ScalarDivide( in Matrix2x2<T> l, T r )
		=> Matrix2x2Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> MultiplyEntrywise( in Matrix2x2<T> l, in Matrix2x2<T> r )
		=> Matrix2x2Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> DivideEntrywise( in Matrix2x2<T> l, in Matrix2x2<T> r )
		=> Matrix2x2Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> Multiply( in Matrix2x2<T> l, in Matrix2x2<T> r )
		=> Matrix2x2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetDeterminant( in Matrix2x2<T> m )
		=> Matrix2x2Math<T>.GetDeterminant( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> GetTransposed( in Matrix2x2<T> m )
		=> new(
			m.M00, m.M10,
			m.M01, m.M11
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix2x2<T> GetInverse( in Matrix2x2<T> m )
		=> throw new NotImplementedException();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Expand( in Matrix2x2<T> m, out Matrix3x3<T> result )
		=> result = new(
			m.M00, m.M01, T.Zero,
			m.M10, m.M11, T.Zero,
			T.Zero, T.Zero, T.One
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Expand( in Matrix2x2<T> m, out Matrix4x4<T> result )
		=> result = new(
			m.M00, m.M01, T.Zero, T.Zero,
			m.M10, m.M11, T.Zero, T.Zero,
			T.Zero, T.Zero, T.One, T.Zero,
			T.Zero, T.Zero, T.Zero, T.One
		);

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFillRowMajor<TData>( in Matrix2x2<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where TData : unmanaged {
		unsafe {
			uint sizeStorageBytes = (uint) (resultStorage.Length * sizeof( T ));
			uint expectedSizeBytes = (uint) sizeof( Matrix2x2<T> );
			if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
				return false;
			fixed (Matrix2x2<T>* srcPtr = &m)
			fixed (TData* dstPtr = resultStorage)
				Unsafe.CopyBlock( (byte*) dstPtr + destinationOffsetBytes, srcPtr, expectedSizeBytes );
			return true;
		}
	}

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFillColumnMajor<TData>( in Matrix2x2<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where TData : unmanaged {
		unsafe {
			uint sizeStorageBytes = (uint) (resultStorage.Length * sizeof( T ));
			uint expectedSizeBytes = (uint) sizeof( Matrix2x2<T> );
			if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
				return false;
			Matrix2x2<T> transposed = GetTransposed( m );
			fixed (TData* dstPtr = resultStorage)
				Unsafe.CopyBlock( (byte*) dstPtr + destinationOffsetBytes, &transposed, expectedSizeBytes );
			return true;
		}
	}
}
