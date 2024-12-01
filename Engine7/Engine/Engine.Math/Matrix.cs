using System.Numerics;
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

	public static Matrix2x2<TNewScalar> CastChecked<TOriginalScalar, TNewScalar>( in this Matrix2x2<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateChecked( matrix.M00 ), TNewScalar.CreateChecked( matrix.M01 ),
			TNewScalar.CreateChecked( matrix.M10 ), TNewScalar.CreateChecked( matrix.M11 )
		);
	public static Matrix2x2<TNewScalar> CastTruncating<TOriginalScalar, TNewScalar>( in this Matrix2x2<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateTruncating( matrix.M00 ), TNewScalar.CreateTruncating( matrix.M01 ),
			TNewScalar.CreateTruncating( matrix.M10 ), TNewScalar.CreateTruncating( matrix.M11 )
		);
	public static Matrix2x2<TNewScalar> CastSaturating<TOriginalScalar, TNewScalar>( in this Matrix2x2<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateSaturating( matrix.M00 ), TNewScalar.CreateSaturating( matrix.M01 ),
			TNewScalar.CreateSaturating( matrix.M10 ), TNewScalar.CreateSaturating( matrix.M11 )
		);

	public static Matrix3x3<TNewScalar> CastChecked<TOriginalScalar, TNewScalar>( in this Matrix3x3<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateChecked( matrix.M00 ), TNewScalar.CreateChecked( matrix.M01 ), TNewScalar.CreateChecked( matrix.M02 ),
			TNewScalar.CreateChecked( matrix.M10 ), TNewScalar.CreateChecked( matrix.M11 ), TNewScalar.CreateChecked( matrix.M12 ),
			TNewScalar.CreateChecked( matrix.M20 ), TNewScalar.CreateChecked( matrix.M21 ), TNewScalar.CreateChecked( matrix.M22 )
		);
	public static Matrix3x3<TNewScalar> CastTruncating<TOriginalScalar, TNewScalar>( in this Matrix3x3<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateTruncating( matrix.M00 ), TNewScalar.CreateTruncating( matrix.M01 ), TNewScalar.CreateTruncating( matrix.M02 ),
			TNewScalar.CreateTruncating( matrix.M10 ), TNewScalar.CreateTruncating( matrix.M11 ), TNewScalar.CreateTruncating( matrix.M12 ),
			TNewScalar.CreateTruncating( matrix.M20 ), TNewScalar.CreateTruncating( matrix.M21 ), TNewScalar.CreateTruncating( matrix.M22 )
		);
	public static Matrix3x3<TNewScalar> CastSaturating<TOriginalScalar, TNewScalar>( in this Matrix3x3<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateSaturating( matrix.M00 ), TNewScalar.CreateSaturating( matrix.M01 ), TNewScalar.CreateSaturating( matrix.M02 ),
			TNewScalar.CreateSaturating( matrix.M10 ), TNewScalar.CreateSaturating( matrix.M11 ), TNewScalar.CreateSaturating( matrix.M12 ),
			TNewScalar.CreateSaturating( matrix.M20 ), TNewScalar.CreateSaturating( matrix.M21 ), TNewScalar.CreateSaturating( matrix.M22 )
		);

	public static Matrix4x4<TNewScalar> CastChecked<TOriginalScalar, TNewScalar>( in this Matrix4x4<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateChecked( matrix.M00 ), TNewScalar.CreateChecked( matrix.M01 ), TNewScalar.CreateChecked( matrix.M02 ), TNewScalar.CreateChecked( matrix.M03 ),
			TNewScalar.CreateChecked( matrix.M10 ), TNewScalar.CreateChecked( matrix.M11 ), TNewScalar.CreateChecked( matrix.M12 ), TNewScalar.CreateChecked( matrix.M13 ),
			TNewScalar.CreateChecked( matrix.M20 ), TNewScalar.CreateChecked( matrix.M21 ), TNewScalar.CreateChecked( matrix.M22 ), TNewScalar.CreateChecked( matrix.M23 ),
			TNewScalar.CreateChecked( matrix.M30 ), TNewScalar.CreateChecked( matrix.M31 ), TNewScalar.CreateChecked( matrix.M32 ), TNewScalar.CreateChecked( matrix.M33 )
		);
	public static Matrix4x4<TNewScalar> CastTruncating<TOriginalScalar, TNewScalar>( in this Matrix4x4<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateTruncating( matrix.M00 ), TNewScalar.CreateTruncating( matrix.M01 ), TNewScalar.CreateTruncating( matrix.M02 ), TNewScalar.CreateTruncating( matrix.M03 ),
			TNewScalar.CreateTruncating( matrix.M10 ), TNewScalar.CreateTruncating( matrix.M11 ), TNewScalar.CreateTruncating( matrix.M12 ), TNewScalar.CreateTruncating( matrix.M13 ),
			TNewScalar.CreateTruncating( matrix.M20 ), TNewScalar.CreateTruncating( matrix.M21 ), TNewScalar.CreateTruncating( matrix.M22 ), TNewScalar.CreateTruncating( matrix.M23 ),
			TNewScalar.CreateTruncating( matrix.M30 ), TNewScalar.CreateTruncating( matrix.M31 ), TNewScalar.CreateTruncating( matrix.M32 ), TNewScalar.CreateTruncating( matrix.M33 )
		);
	public static Matrix4x4<TNewScalar> CastSaturating<TOriginalScalar, TNewScalar>( in this Matrix4x4<TOriginalScalar> matrix )
		where TOriginalScalar : unmanaged, INumber<TOriginalScalar>
		where TNewScalar : unmanaged, INumber<TNewScalar>
		=> new(
			TNewScalar.CreateSaturating( matrix.M00 ), TNewScalar.CreateSaturating( matrix.M01 ), TNewScalar.CreateSaturating( matrix.M02 ), TNewScalar.CreateSaturating( matrix.M03 ),
			TNewScalar.CreateSaturating( matrix.M10 ), TNewScalar.CreateSaturating( matrix.M11 ), TNewScalar.CreateSaturating( matrix.M12 ), TNewScalar.CreateSaturating( matrix.M13 ),
			TNewScalar.CreateSaturating( matrix.M20 ), TNewScalar.CreateSaturating( matrix.M21 ), TNewScalar.CreateSaturating( matrix.M22 ), TNewScalar.CreateSaturating( matrix.M23 ),
			TNewScalar.CreateSaturating( matrix.M30 ), TNewScalar.CreateSaturating( matrix.M31 ), TNewScalar.CreateSaturating( matrix.M32 ), TNewScalar.CreateSaturating( matrix.M33 )
		);
}
