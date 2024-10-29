using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Math.GeometricAlgebra;

/// <summary>
/// Matrices are stored in row-major order.
/// </summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Matrix2x2<TScalar>(TScalar m00, TScalar m01, TScalar m10, TScalar m11) :
		IVector<Matrix2x2<TScalar>, TScalar>,
		ILinearAlgebraVectorOperators<Matrix2x2<TScalar>>,
		ILinearAlgebraScalarOperators<Matrix2x2<TScalar>, TScalar>,
		IMatrix<TScalar>,
		ISquareMatrix<Matrix2x2<TScalar>>,
		IProduct<Matrix2x2<TScalar>, Matrix2x2<TScalar>, Matrix2x2<TScalar>>,
		IProduct<Matrix2x2<TScalar>, Vector2<TScalar>, Vector2<TScalar>>
	where TScalar
		: unmanaged, INumber<TScalar>
{

	/// <summary>
	/// First row, first column
	/// </summary>
	public readonly TScalar M00 = m00;
	/// <summary>
	/// First row, second column
	/// </summary>
	public readonly TScalar M01 = m01;
	/// <summary>
	/// Second row, first column
	/// </summary>
	public readonly TScalar M10 = m10;
	/// <summary>
	/// Second row, second column
	/// </summary>
	public readonly TScalar M11 = m11;

	public uint Rows => 2;
	public uint Columns => 2;
	public TScalar this[uint row, uint col]
		=> row switch
		{
			0 => col switch
			{
				0 => M00,
				1 => M01,
				_ => throw new ArgumentOutOfRangeException(nameof(col)),
			},
			1 => col switch
			{
				0 => M10,
				1 => M11,
				_ => throw new ArgumentOutOfRangeException(nameof(col)),
			},
			_ => throw new ArgumentOutOfRangeException(nameof(row)),
		};

	public Vector2<TScalar> Row0 => new(M00, M01);
	public Vector2<TScalar> Row1 => new(M10, M11);
	public Vector2<TScalar> Col0 => new(M00, M10);
	public Vector2<TScalar> Col1 => new(M01, M11);

	public static Matrix2x2<TScalar> AdditiveIdentity => Zero;
	public static Matrix2x2<TScalar> MultiplicativeIdentity { get; } = new(TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.One);
	public static Matrix2x2<TScalar> Zero { get; } = new(TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero);
	public static Matrix2x2<TScalar> One { get; } = new(TScalar.One, TScalar.One, TScalar.One, TScalar.One);
	/// <summary>
	/// 2x Multiplicative Identity
	/// </summary>
	public static Matrix2x2<TScalar> Two { get; } = MultiplicativeIdentity + MultiplicativeIdentity;

	public Matrix2x2<TScalar> Negate() => new(-M00, -M01, -M10, -M11);
	public Matrix2x2<TScalar> Add(in Matrix2x2<TScalar> r) => new(M00 + r.M00, M01 + r.M01, M10 + r.M10, M11 + r.M11);
	public Matrix2x2<TScalar> Subtract(in Matrix2x2<TScalar> r) => new(M00 - r.M00, M01 - r.M01, M10 - r.M10, M11 - r.M11);
	public Matrix2x2<TScalar> ScalarMultiply(TScalar r) => new(M00 * r, M01 * r, M10 * r, M11 * r);
	public Matrix2x2<TScalar> ScalarDivide(TScalar r) => new(M00 / r, M01 / r, M10 / r, M11 / r);
	public static Matrix2x2<TScalar> DivideScalar(TScalar l, in Matrix2x2<TScalar> r) => new(l / r.M00, l / r.M01, l / r.M10, l / r.M11);
	public TScalar Dot(in Matrix2x2<TScalar> r) => M00 * r.M00 + M01 * r.M01 + M10 * r.M10 + M11 * r.M11;
	public TScalar MagnitudeSquared() => Dot(this);

	public TScalar GetDeterminant() => M00 * M11 - M01 * M10;
	public Matrix2x2<TScalar> GetTransposed() => new(M00, M10, M01, M11);
	public bool TryGetInverse(out Matrix2x2<TScalar> matrix)
	{
		TScalar determinant = GetDeterminant();
		if (TScalar.IsZero(determinant))
		{
			matrix = default;
			return false;
		}
		matrix = new Matrix2x2<TScalar>(M11, -M01, -M10, M00) / determinant;
		return true;
	}
	//public bool TryGetUpperTriangular( out Matrix2x2<TScalar> upperTriangular ) {
	//	Matrix2x2<TScalar> matrix = this;
	//	if (TScalar.IsZero( M00 )) {
	//		if (TScalar.IsZero( M10 )) {
	//			upperTriangular = default;
	//			return false;
	//		}
	//		matrix = new( M10, M11, M00, M01 );
	//	}
	//	upperTriangular = new( matrix.M00, matrix.M01, TScalar.Zero, matrix.M11 - matrix.M10 * matrix.M01 / matrix.M00 );
	//	return true;
	//}
	//public bool TryGetUpperTriangularDecomposition( out Matrix2x2<TScalar> lowerDecomposition, out Matrix2x2<TScalar> upperDecomposition ) => throw new NotImplementedException();
	//public bool TryGetLowerTriangular( out Matrix2x2<TScalar> lowerTriangular ) => throw new NotImplementedException();
	//public bool TryGetLowerTriangularDecomposition( out Matrix2x2<TScalar> lowerDecomposition, out Matrix2x2<TScalar> upperDecomposition ) => throw new NotImplementedException();

	public Matrix2x2<TScalar> Multiply(in Matrix2x2<TScalar> r)
		=> new(
			Row0.Dot(r.Col0), Row0.Dot(r.Col1),
			Row1.Dot(r.Col0), Row1.Dot(r.Col1)
		);
	public Vector2<TScalar> Multiply(in Vector2<TScalar> r)
		=> new(Row0.Dot(r), Row1.Dot(r));
	public static Matrix2x2<TScalar> operator *(in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r) => l.Multiply(r);
	public static Vector2<TScalar> operator *(in Matrix2x2<TScalar> l, in Vector2<TScalar> r) => l.Multiply(r);

	public static Matrix2x2<TScalar> operator -(in Matrix2x2<TScalar> l) => l.Negate();
	public static Matrix2x2<TScalar> operator +(in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r) => l.Add(r);
	public static Matrix2x2<TScalar> operator -(in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r) => l.Subtract(r);
	public static Matrix2x2<TScalar> operator *(in Matrix2x2<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Matrix2x2<TScalar> operator *(TScalar l, in Matrix2x2<TScalar> r) => r.ScalarMultiply(l);
	public static Matrix2x2<TScalar> operator /(in Matrix2x2<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Matrix2x2<TScalar> operator /(TScalar l, in Matrix2x2<TScalar> r) => DivideScalar(l, r);

	public static Matrix2x2<TScalar> ConstructFromRows(Vector2<TScalar> row0, Vector2<TScalar> row1) => new(row0.X, row0.Y, row1.X, row1.Y);
	public static Matrix2x2<TScalar> ConstructFromColumns(Vector2<TScalar> col0, Vector2<TScalar> col1) => new(col0.X, col1.X, col0.Y, col1.Y);

	public static bool operator ==(in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r) => l.Row0 == r.Row0 && l.Row1 == r.Row1;
	public static bool operator !=(in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r) => !(l == r);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix2x2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(Row0, Row1);
	public override string ToString()
		=> string.Create(CultureInfo.InvariantCulture,
			$"[{M00:#,##0.###} {M01:#,##0.###}|{M10:#,##0.###} {M11:#,##0.###}]");
}
