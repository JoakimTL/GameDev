using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Math.GeometricAlgebra;

/// <summary>
/// Matrices are stored in row-major order.
/// </summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Matrix3x3<TScalar>(TScalar m00, TScalar m01, TScalar m02, TScalar m10, TScalar m11, TScalar m12, TScalar m20, TScalar m21, TScalar m22) :
		IVector<Matrix3x3<TScalar>, TScalar>,
		ILinearAlgebraVectorOperators<Matrix3x3<TScalar>>,
		ILinearAlgebraScalarOperators<Matrix3x3<TScalar>, TScalar>,
		IMatrix<TScalar>,
		ISquareMatrix<Matrix3x3<TScalar>>,
		IProduct<Matrix3x3<TScalar>, Matrix3x3<TScalar>, Matrix3x3<TScalar>>,
		IProduct<Matrix3x3<TScalar>, Vector3<TScalar>, Vector3<TScalar>>,
		IExplicitCast<Matrix3x3<TScalar>, Matrix4x4<TScalar>>
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
	/// First row, third column
	/// </summary>
	public readonly TScalar M02 = m02;
	/// <summary>
	/// Second row, first column
	/// </summary>
	public readonly TScalar M10 = m10;
	/// <summary>
	/// Second row, second column
	/// </summary>
	public readonly TScalar M11 = m11;
	/// <summary>
	/// Second row, third column
	/// </summary>
	public readonly TScalar M12 = m12;
	/// <summary>
	/// Third row, first column
	/// </summary>
	public readonly TScalar M20 = m20;
	/// <summary>
	/// Third row, second column
	/// </summary>
	public readonly TScalar M21 = m21;
	/// <summary>
	/// Third row, third column
	/// </summary>
	public readonly TScalar M22 = m22;

	public uint Rows => 3;
	public uint Columns => 3;
	public TScalar this[uint row, uint col]
		=> row switch
		{
			0 => col switch
			{
				0 => M00,
				1 => M01,
				2 => M02,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			1 => col switch
			{
				0 => M10,
				1 => M11,
				2 => M12,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			2 => col switch
			{
				0 => M20,
				1 => M21,
				2 => M22,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			_ => throw new ArgumentOutOfRangeException(nameof(row))
		};

	public Vector3<TScalar> Row0 => new(M00, M01, M02);
	public Vector3<TScalar> Row1 => new(M10, M11, M12);
	public Vector3<TScalar> Row2 => new(M20, M21, M22);
	public Vector3<TScalar> Col0 => new(M00, M10, M20);
	public Vector3<TScalar> Col1 => new(M01, M11, M21);
	public Vector3<TScalar> Col2 => new(M02, M12, M22);

	public Matrix2x2<TScalar> Excluding00 => new(M11, M12, M21, M22);
	public Matrix2x2<TScalar> Excluding01 => new(M10, M12, M20, M22);
	public Matrix2x2<TScalar> Excluding02 => new(M10, M11, M20, M21);
	public Matrix2x2<TScalar> Excluding10 => new(M01, M02, M21, M22);
	public Matrix2x2<TScalar> Excluding11 => new(M00, M02, M20, M22);
	public Matrix2x2<TScalar> Excluding12 => new(M00, M01, M20, M21);
	public Matrix2x2<TScalar> Excluding20 => new(M01, M02, M11, M12);
	public Matrix2x2<TScalar> Excluding21 => new(M00, M02, M10, M12);
	public Matrix2x2<TScalar> Excluding22 => new(M00, M01, M10, M11);

	public static Matrix3x3<TScalar> AdditiveIdentity => Zero;
	public static Matrix3x3<TScalar> MultiplicativeIdentity { get; } = new(TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One);
	public static Matrix3x3<TScalar> Zero { get; } = new(TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero);
	public static Matrix3x3<TScalar> One { get; } = new(TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One, TScalar.One);
	/// <summary>
	/// 2x Multiplicative Identity
	/// </summary>
	public static Matrix3x3<TScalar> Two { get; } = MultiplicativeIdentity + MultiplicativeIdentity;


	/// <summary>
	/// Creates a 3x3 multiplicative identity matrix overlaid by a 2x2 matrix.
	/// </summary>
	public Matrix3x3(TScalar m00, TScalar m01, TScalar m10, TScalar m11) : this(
		m00, m01, TScalar.Zero,
		m10, m11, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.One
	)
	{ }

	public Matrix3x3<TScalar> Negate() => new(-M00, -M01, -M02, -M10, -M11, -M12, -M20, -M21, -M22);
	public Matrix3x3<TScalar> Add(in Matrix3x3<TScalar> r) => new(M00 + r.M00, M01 + r.M01, M02 + r.M02, M10 + r.M10, M11 + r.M11, M12 + r.M12, M20 + r.M20, M21 + r.M21, M22 + r.M22);
	public Matrix3x3<TScalar> Subtract(in Matrix3x3<TScalar> r) => new(M00 - r.M00, M01 - r.M01, M02 - r.M02, M10 - r.M10, M11 - r.M11, M12 - r.M12, M20 - r.M20, M21 - r.M21, M22 - r.M22);
	public Matrix3x3<TScalar> ScalarMultiply(TScalar r) => new(M00 * r, M01 * r, M02 * r, M10 * r, M11 * r, M12 * r, M20 * r, M21 * r, M22 * r);
	public Matrix3x3<TScalar> ScalarDivide(TScalar r) => new(M00 / r, M01 / r, M02 / r, M10 / r, M11 / r, M12 / r, M20 / r, M21 / r, M22 / r);
	public static Matrix3x3<TScalar> DivideScalar(TScalar l, in Matrix3x3<TScalar> r) => new(l / r.M00, l / r.M01, l / r.M02, l / r.M10, l / r.M11, l / r.M12, l / r.M20, l / r.M21, l / r.M22);
	public TScalar Dot(in Matrix3x3<TScalar> r) => M00 * r.M00 + M01 * r.M01 + M02 * r.M02 + M10 * r.M10 + M11 * r.M11 + M12 * r.M12 + M20 * r.M20 + M21 * r.M21 + M22 * r.M22;
	public TScalar MagnitudeSquared() => Dot(this);

	public TScalar GetDeterminant() => M00 * Excluding00.GetDeterminant() - M01 * Excluding01.GetDeterminant() + M02 * Excluding02.GetDeterminant();
	public Matrix3x3<TScalar> GetTransposed() => new(M00, M10, M20, M01, M11, M21, M02, M12, M22);
	public bool TryGetInverse(out Matrix3x3<TScalar> matrix)
	{
		TScalar determinant = GetDeterminant();
		if (TScalar.IsZero(determinant))
		{
			matrix = default;
			return false;
		}
		matrix = new Matrix3x3<TScalar>(
			Excluding00.GetDeterminant(), -Excluding10.GetDeterminant(), Excluding20.GetDeterminant(),
			-Excluding01.GetDeterminant(), Excluding11.GetDeterminant(), -Excluding21.GetDeterminant(),
			Excluding02.GetDeterminant(), -Excluding12.GetDeterminant(), Excluding22.GetDeterminant()
		) / determinant;
		return true;
	}

	public Matrix3x3<TScalar> Multiply(in Matrix3x3<TScalar> r)
		=> new(
			Row0.Dot(r.Col0), Row0.Dot(r.Col1), Row0.Dot(r.Col2),
			Row1.Dot(r.Col0), Row1.Dot(r.Col1), Row1.Dot(r.Col2),
			Row2.Dot(r.Col0), Row2.Dot(r.Col1), Row2.Dot(r.Col2)
		);
	public Vector3<TScalar> Multiply(in Vector3<TScalar> r)
		=> new(Row0.Dot(r), Row1.Dot(r), Row2.Dot(r));
	public static Matrix3x3<TScalar> operator *(in Matrix3x3<TScalar> l, in Matrix3x3<TScalar> r) => l.Multiply(r);
	public static Vector3<TScalar> operator *(in Matrix3x3<TScalar> l, in Vector3<TScalar> r) => l.Multiply(r);

	public static Matrix3x3<TScalar> operator -(in Matrix3x3<TScalar> l) => l.Negate();
	public static Matrix3x3<TScalar> operator +(in Matrix3x3<TScalar> l, in Matrix3x3<TScalar> r) => l.Add(r);
	public static Matrix3x3<TScalar> operator -(in Matrix3x3<TScalar> l, in Matrix3x3<TScalar> r) => l.Subtract(r);
	public static Matrix3x3<TScalar> operator *(in Matrix3x3<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Matrix3x3<TScalar> operator *(TScalar l, in Matrix3x3<TScalar> r) => r.ScalarMultiply(l);
	public static Matrix3x3<TScalar> operator /(in Matrix3x3<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Matrix3x3<TScalar> operator /(TScalar l, in Matrix3x3<TScalar> r) => DivideScalar(l, r);

	public static Matrix3x3<TScalar> ConstructFromRows(Vector3<TScalar> row0, Vector3<TScalar> row1, Vector3<TScalar> row2) => new(row0.X, row0.Y, row0.Z, row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z);
	public static Matrix3x3<TScalar> ConstructFromColumns(Vector3<TScalar> col0, Vector3<TScalar> col1, Vector3<TScalar> col2) => new(col0.X, col1.X, col2.X, col0.Y, col1.Y, col2.Y, col0.Z, col1.Z, col2.Z);

	public static bool operator ==(in Matrix3x3<TScalar> l, in Matrix3x3<TScalar> r) => l.Row0 == r.Row0 && l.Row1 == r.Row1 && l.Row2 == r.Row2;
	public static bool operator !=(in Matrix3x3<TScalar> l, in Matrix3x3<TScalar> r) => !(l == r);

	public static explicit operator Matrix4x4<TScalar>(in Matrix3x3<TScalar> m)
		=> new(
			m.M00, m.M01, m.M02, TScalar.Zero,
			m.M10, m.M11, m.M12, TScalar.Zero,
			m.M20, m.M21, m.M22, TScalar.Zero,
			TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One
		);

	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix3x3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(Row0, Row1, Row2);
	public override string ToString()
		=> string.Create(CultureInfo.InvariantCulture,
			$"[{M00:#,##0.###} {M01:#,##0.###} {M02:#,##0.###}|{M10:#,##0.###} {M11:#,##0.###} {M12:#,##0.###}|{M20:#,##0.###} {M21:#,##0.###} {M22:#,##0.###}]");
}
