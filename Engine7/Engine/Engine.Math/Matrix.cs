using System.Runtime.CompilerServices;

namespace Engine;

public static partial class Matrix {
	/// <returns>False if there is not enough space in the span for the copy of the matrix</returns>
	public static bool TryFill<TMatrix, TData>( this TMatrix m, Span<TData> resultStorage, bool columnMajor = false, uint destinationOffsetBytes = 0 )
		where TMatrix :
			unmanaged, ISquareMatrix<TMatrix>
		where TData :
			unmanaged {
		unsafe {
			uint sizeStorageBytes = (uint) (resultStorage.Length * sizeof( TData ));
			uint expectedSizeBytes = (uint) sizeof( TMatrix );
			if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
				return false;
			TMatrix matrix = columnMajor ? m.GetTransposed() : m;
			fixed (TData* dstPtr = resultStorage)
				Unsafe.CopyBlock( (byte*) dstPtr + destinationOffsetBytes, &matrix, expectedSizeBytes );
			return true;
		}
	}
}