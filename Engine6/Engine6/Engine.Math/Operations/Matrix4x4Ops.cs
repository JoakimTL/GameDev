using Engine.Math.Calculation;
using Engine.Math.Operations.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Matrix4x4{T}"/>. Return types may vary.
/// </summary>
public sealed class Matrix4x4Ops<T> :
		ILinearAlgebraOperations<Matrix4x4<T>, T>,
		IEntrywiseOperations<Matrix4x4<T>>,
		IProductOperation<Matrix4x4<T>, Matrix4x4<T>, Matrix4x4<T>>,
		IMatrixOperations<Matrix4x4<T>, T>,
		IMatrixDataOperations<Matrix4x4<T>>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Negate( in Matrix4x4<T> l ) => Matrix4x4Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Add( in Matrix4x4<T> l, in Matrix4x4<T> r ) => Matrix4x4Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Subtract( in Matrix4x4<T> l, in Matrix4x4<T> r ) => Matrix4x4Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> ScalarMultiply( in Matrix4x4<T> l, T r ) => Matrix4x4Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> ScalarDivide( in Matrix4x4<T> l, T r ) => Matrix4x4Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> MultiplyEntrywise( in Matrix4x4<T> l, in Matrix4x4<T> r ) => Matrix4x4Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> DivideEntrywise( in Matrix4x4<T> l, in Matrix4x4<T> r ) => Matrix4x4Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> Multiply( in Matrix4x4<T> l, in Matrix4x4<T> r ) => Matrix4x4Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetDeterminant( in Matrix4x4<T> m ) => Matrix4x4Math<T>.GetDeterminant( m );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> GetTransposed( in Matrix4x4<T> m )
		=> new(
			m.M00, m.M10, m.M20, m.M30,
			m.M01, m.M11, m.M21, m.M31,
			m.M02, m.M12, m.M22, m.M32,
			m.M03, m.M13, m.M23, m.M33
		);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Matrix4x4<T> GetInverse( in Matrix4x4<T> m )
		=> throw new System.NotImplementedException();

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFillRowMajor<TData>( in Matrix4x4<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where TData : unmanaged {
		unsafe {
			uint sizeStorageBytes = (uint) (resultStorage.Length * sizeof( T ));
			uint expectedSizeBytes = (uint) sizeof( Matrix4x4<T> );
			if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
				return false;
			fixed (Matrix4x4<T>* srcPtr = &m)
			fixed (TData* dstPtr = resultStorage)
				Unsafe.CopyBlock( (byte*) dstPtr + destinationOffsetBytes, srcPtr, expectedSizeBytes );
			return true;
		}
	}

	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFillColumnMajor<TData>( in Matrix4x4<T> m, Span<TData> resultStorage, uint destinationOffsetBytes = 0 ) where TData : unmanaged {
		unsafe {
			uint sizeStorageBytes = (uint) (resultStorage.Length * sizeof( T ));
			uint expectedSizeBytes = (uint) sizeof( Matrix4x4<T> );
			if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
				return false;
			Matrix4x4<T> transposed = GetTransposed( m );
			fixed (TData* dstPtr = resultStorage)
				Unsafe.CopyBlock( (byte*) dstPtr + destinationOffsetBytes, &transposed, expectedSizeBytes );
			return true;
		}
	}
}