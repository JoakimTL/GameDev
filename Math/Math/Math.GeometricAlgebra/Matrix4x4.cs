﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Math.GeometricAlgebra;

/// <summary>
/// Matrices are stored in row-major order.
/// </summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Matrix4x4<TScalar>(TScalar m00, TScalar m01, TScalar m02, TScalar m03, TScalar m10, TScalar m11, TScalar m12, TScalar m13, TScalar m20, TScalar m21, TScalar m22, TScalar m23, TScalar m30, TScalar m31, TScalar m32, TScalar m33) :
		IVector<Matrix4x4<TScalar>, TScalar>,
		ILinearAlgebraVectorOperators<Matrix4x4<TScalar>>,
		ILinearAlgebraScalarOperators<Matrix4x4<TScalar>, TScalar>,
		IMatrix<TScalar>,
		ISquareMatrix<Matrix4x4<TScalar>>,
		IProduct<Matrix4x4<TScalar>, Matrix4x4<TScalar>, Matrix4x4<TScalar>>,
		IProduct<Matrix4x4<TScalar>, Vector4<TScalar>, Vector4<TScalar>>,
		IMatrixVectorTransformation<Matrix4x4<TScalar>, Vector2<TScalar>>,
		IMatrixVectorTransformation<Matrix4x4<TScalar>, Vector3<TScalar>>
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
	/// First row, fourth column
	/// </summary>
	public readonly TScalar M03 = m03;
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
	/// Second row, fourth column
	/// </summary>
	public readonly TScalar M13 = m13;
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
	/// <summary>
	/// Third row, fourth column
	/// </summary>
	public readonly TScalar M23 = m23;
	/// <summary>
	/// Fourth row, first column
	/// </summary>
	public readonly TScalar M30 = m30;
	/// <summary>
	/// Fourth row, second column
	/// </summary>
	public readonly TScalar M31 = m31;
	/// <summary>
	/// Fourth row, third column
	/// </summary>
	public readonly TScalar M32 = m32;
	/// <summary>
	/// Fourth row, fourth column
	/// </summary>
	public readonly TScalar M33 = m33;

	public uint Rows => 4;
	public uint Columns => 4;
	public TScalar this[uint row, uint col]
		=> row switch
		{
			0 => col switch
			{
				0 => M00,
				1 => M01,
				2 => M02,
				3 => M03,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			1 => col switch
			{
				0 => M10,
				1 => M11,
				2 => M12,
				3 => M13,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			2 => col switch
			{
				0 => M20,
				1 => M21,
				2 => M22,
				3 => M23,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			3 => col switch
			{
				0 => M30,
				1 => M31,
				2 => M32,
				3 => M33,
				_ => throw new ArgumentOutOfRangeException(nameof(col))
			},
			_ => throw new ArgumentOutOfRangeException(nameof(row))
		};

	public Vector4<TScalar> Row0 => new(M00, M01, M02, M03);
	public Vector4<TScalar> Row1 => new(M10, M11, M12, M13);
	public Vector4<TScalar> Row2 => new(M20, M21, M22, M23);
	public Vector4<TScalar> Row3 => new(M30, M31, M32, M33);
	public Vector4<TScalar> Col0 => new(M00, M10, M20, M30);
	public Vector4<TScalar> Col1 => new(M01, M11, M21, M31);
	public Vector4<TScalar> Col2 => new(M02, M12, M22, M32);
	public Vector4<TScalar> Col3 => new(M03, M13, M23, M33);

	public Matrix3x3<TScalar> Excluding00 => new(M11, M12, M13, M21, M22, M23, M31, M32, M33);
	public Matrix3x3<TScalar> Excluding01 => new(M10, M12, M13, M20, M22, M23, M30, M32, M33);
	public Matrix3x3<TScalar> Excluding02 => new(M10, M11, M13, M20, M21, M23, M30, M31, M33);
	public Matrix3x3<TScalar> Excluding03 => new(M10, M11, M12, M20, M21, M22, M30, M31, M32);
	public Matrix3x3<TScalar> Excluding10 => new(M01, M02, M03, M21, M22, M23, M31, M32, M33);
	public Matrix3x3<TScalar> Excluding11 => new(M00, M02, M03, M20, M22, M23, M30, M32, M33);
	public Matrix3x3<TScalar> Excluding12 => new(M00, M01, M03, M20, M21, M23, M30, M31, M33);
	public Matrix3x3<TScalar> Excluding13 => new(M00, M01, M02, M20, M21, M22, M30, M31, M32);
	public Matrix3x3<TScalar> Excluding20 => new(M01, M02, M03, M11, M12, M13, M31, M32, M33);
	public Matrix3x3<TScalar> Excluding21 => new(M00, M02, M03, M10, M12, M13, M30, M32, M33);
	public Matrix3x3<TScalar> Excluding22 => new(M00, M01, M03, M10, M11, M13, M30, M31, M33);
	public Matrix3x3<TScalar> Excluding23 => new(M00, M01, M02, M10, M11, M12, M30, M31, M32);
	public Matrix3x3<TScalar> Excluding30 => new(M01, M02, M03, M11, M12, M13, M21, M22, M23);
	public Matrix3x3<TScalar> Excluding31 => new(M00, M02, M03, M10, M12, M13, M20, M22, M23);
	public Matrix3x3<TScalar> Excluding32 => new(M00, M01, M03, M10, M11, M13, M20, M21, M23);
	public Matrix3x3<TScalar> Excluding33 => new(M00, M01, M02, M10, M11, M12, M20, M21, M22);

	public static Matrix4x4<TScalar> AdditiveIdentity => Zero;
	public static Matrix4x4<TScalar> MultiplicativeIdentity { get; } =
		new(
			TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero,
			TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero,
			TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero,
			TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One
		);
	public static Matrix4x4<TScalar> Zero { get; } =
		new(
			TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero,
			TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero,
			TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero,
			TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero
		);
	public static Matrix4x4<TScalar> One { get; } =
		new(
			TScalar.One, TScalar.One, TScalar.One, TScalar.One,
			TScalar.One, TScalar.One, TScalar.One, TScalar.One,
			TScalar.One, TScalar.One, TScalar.One, TScalar.One,
			TScalar.One, TScalar.One, TScalar.One, TScalar.One
		);
	/// <summary>
	/// 2x Multiplicative Identity
	/// </summary>
	public static Matrix4x4<TScalar> Two { get; } = MultiplicativeIdentity + MultiplicativeIdentity;

	/// <summary>
	/// Creates a 4x4 multiplicative identity matrix overlaid by a 3x3 matrix.
	/// </summary>
	public Matrix4x4(TScalar m00, TScalar m01, TScalar m02, TScalar m10, TScalar m11, TScalar m12, TScalar m20, TScalar m21, TScalar m22) : this(
		m00, m01, m02, TScalar.Zero,
		m10, m11, m12, TScalar.Zero,
		m20, m21, m22, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One
	)
	{ }

	/// <summary>
	/// Creates a 4x4 multiplicative identity matrix overlaid by a 2x2 matrix.
	/// </summary>
	public Matrix4x4(TScalar m00, TScalar m01, TScalar m10, TScalar m11) : this(
		m00, m01, TScalar.Zero, TScalar.Zero,
		m10, m11, TScalar.Zero, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One
	)
	{ }

	public Matrix4x4<TScalar> Negate()
		=> new(
			-M00, -M01, -M02, -M03,
			-M10, -M11, -M12, -M13,
			-M20, -M21, -M22, -M23,
			-M30, -M31, -M32, -M33
		);
	public Matrix4x4<TScalar> Add(in Matrix4x4<TScalar> r)
		=> new(
			M00 + r.M00, M01 + r.M01, M02 + r.M02, M03 + r.M03,
			M10 + r.M10, M11 + r.M11, M12 + r.M12, M13 + r.M13,
			M20 + r.M20, M21 + r.M21, M22 + r.M22, M23 + r.M23,
			M30 + r.M30, M31 + r.M31, M32 + r.M32, M33 + r.M33
		);
	public Matrix4x4<TScalar> Subtract(in Matrix4x4<TScalar> r)
		=> new(
			M00 - r.M00, M01 - r.M01, M02 - r.M02, M03 - r.M03,
			M10 - r.M10, M11 - r.M11, M12 - r.M12, M13 - r.M13,
			M20 - r.M20, M21 - r.M21, M22 - r.M22, M23 - r.M23,
			M30 - r.M30, M31 - r.M31, M32 - r.M32, M33 - r.M33
		);
	public Matrix4x4<TScalar> ScalarMultiply(TScalar r)
		=> new(
			M00 * r, M01 * r, M02 * r, M03 * r,
			M10 * r, M11 * r, M12 * r, M13 * r,
			M20 * r, M21 * r, M22 * r, M23 * r,
			M30 * r, M31 * r, M32 * r, M33 * r
		);
	public Matrix4x4<TScalar> ScalarDivide(TScalar r)
		=> new(
			M00 / r, M01 / r, M02 / r, M03 / r,
			M10 / r, M11 / r, M12 / r, M13 / r,
			M20 / r, M21 / r, M22 / r, M23 / r,
			M30 / r, M31 / r, M32 / r, M33 / r
		);
	public static Matrix4x4<TScalar> DivideScalar(TScalar l, in Matrix4x4<TScalar> r)
		=> new(
			l / r.M00, l / r.M01, l / r.M02, l / r.M03,
			l / r.M10, l / r.M11, l / r.M12, l / r.M13,
			l / r.M20, l / r.M21, l / r.M22, l / r.M23,
			l / r.M30, l / r.M31, l / r.M32, l / r.M33
		);
	public TScalar Dot(in Matrix4x4<TScalar> r) =>
		M00 * r.M00 + M01 * r.M01 + M02 * r.M02 + M03 * r.M03 +
		M10 * r.M10 + M11 * r.M11 + M12 * r.M12 + M13 * r.M13 +
		M20 * r.M20 + M21 * r.M21 + M22 * r.M22 + M23 * r.M23 +
		M30 * r.M30 + M31 * r.M31 + M32 * r.M32 + M33 * r.M33;
	public TScalar MagnitudeSquared() => Dot(this);

	public TScalar GetDeterminant()
		=> M00 * Excluding00.GetDeterminant() - M01 * Excluding01.GetDeterminant() + M02 * Excluding02.GetDeterminant() - M03 * Excluding03.GetDeterminant();
	public Matrix4x4<TScalar> GetTransposed()
		=> new(
			M00, M10, M20, M30,
			M01, M11, M21, M31,
			M02, M12, M22, M32,
			M03, M13, M23, M33
		);
	public bool TryGetInverse(out Matrix4x4<TScalar> matrix)
	{
		TScalar determinant = GetDeterminant();
		if (TScalar.IsZero(determinant))
		{
			matrix = default;
			return false;
		}
		matrix = new Matrix4x4<TScalar>(
			Excluding00.GetDeterminant(), -Excluding10.GetDeterminant(), Excluding20.GetDeterminant(), -Excluding30.GetDeterminant(),
			-Excluding01.GetDeterminant(), Excluding11.GetDeterminant(), -Excluding21.GetDeterminant(), Excluding31.GetDeterminant(),
			Excluding02.GetDeterminant(), -Excluding12.GetDeterminant(), Excluding22.GetDeterminant(), -Excluding32.GetDeterminant(),
			-Excluding03.GetDeterminant(), Excluding13.GetDeterminant(), -Excluding23.GetDeterminant(), Excluding33.GetDeterminant()
		) / determinant;
		return true;
	}

	public Vector2<TScalar>? TransformWorld(in Vector2<TScalar> vector)
	{
		Vector4<TScalar> transformVector = new(vector.X, vector.Y, TScalar.Zero, TScalar.One);
		Vector4<TScalar> result = this * transformVector;
		if (TScalar.IsZero(result.W))
			return null;
		return new Vector2<TScalar>(result.X, result.Y) / result.W;
	}
	public Vector2<TScalar> TransformNormal(in Vector2<TScalar> vector)
	{
		Vector4<TScalar> transformVector = new(vector.X, vector.Y, TScalar.Zero, TScalar.Zero);
		Vector4<TScalar> result = this * transformVector;
		return new Vector2<TScalar>(result.X, result.Y);
	}
	public Vector3<TScalar>? TransformWorld(in Vector3<TScalar> vector)
	{
		Vector4<TScalar> transformVector = new(vector.X, vector.Y, vector.Z, TScalar.One);
		Vector4<TScalar> result = this * transformVector;
		if (TScalar.IsZero(result.W))
			return null;
		return new Vector3<TScalar>(result.X, result.Y, result.Z) / result.W;
	}
	public Vector3<TScalar> TransformNormal(in Vector3<TScalar> vector)
	{
		Vector4<TScalar> transformVector = new(vector.X, vector.Y, vector.Z, TScalar.Zero);
		Vector4<TScalar> result = this * transformVector;
		return new Vector3<TScalar>(result.X, result.Y, result.Z);
	}

	public Matrix4x4<TScalar> Multiply(in Matrix4x4<TScalar> r)
		=> new(
			Row0.Dot(r.Col0), Row0.Dot(r.Col1), Row0.Dot(r.Col2), Row0.Dot(r.Col3),
			Row1.Dot(r.Col0), Row1.Dot(r.Col1), Row1.Dot(r.Col2), Row1.Dot(r.Col3),
			Row2.Dot(r.Col0), Row2.Dot(r.Col1), Row2.Dot(r.Col2), Row2.Dot(r.Col3),
			Row3.Dot(r.Col0), Row3.Dot(r.Col1), Row3.Dot(r.Col2), Row3.Dot(r.Col3)
		);
	public Vector4<TScalar> Multiply(in Vector4<TScalar> r)
		=> new(Row0.Dot(r), Row1.Dot(r), Row2.Dot(r), Row3.Dot(r));
	public static Matrix4x4<TScalar> operator *(in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r) => l.Multiply(r);
	public static Vector4<TScalar> operator *(in Matrix4x4<TScalar> l, in Vector4<TScalar> r) => l.Multiply(r);

	public static Matrix4x4<TScalar> operator -(in Matrix4x4<TScalar> l) => l.Negate();
	public static Matrix4x4<TScalar> operator +(in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r) => l.Add(r);
	public static Matrix4x4<TScalar> operator -(in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r) => l.Subtract(r);
	public static Matrix4x4<TScalar> operator *(in Matrix4x4<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Matrix4x4<TScalar> operator *(TScalar l, in Matrix4x4<TScalar> r) => r.ScalarMultiply(l);
	public static Matrix4x4<TScalar> operator /(in Matrix4x4<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Matrix4x4<TScalar> operator /(TScalar l, in Matrix4x4<TScalar> r) => DivideScalar(l, r);

	public static Matrix4x4<TScalar> ConstructFromRows(Vector4<TScalar> row0, Vector4<TScalar> row1, Vector4<TScalar> row2, Vector4<TScalar> row3)
		=> new(row0.X, row0.Y, row0.Z, row0.W, row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W);
	public static Matrix4x4<TScalar> ConstructFromColumns(Vector4<TScalar> col0, Vector4<TScalar> col1, Vector4<TScalar> col2, Vector4<TScalar> col3)
		=> new(col0.X, col1.X, col2.X, col3.X, col0.Y, col1.Y, col2.Y, col3.Y, col0.Z, col1.Z, col2.Z, col3.Z, col0.W, col1.W, col2.W, col3.W);

	public static bool operator ==(in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r) => l.Row0 == r.Row0 && l.Row1 == r.Row1 && l.Row2 == r.Row2 && l.Row3 == r.Row3;
	public static bool operator !=(in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r) => !(l == r);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix4x4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(Row0, Row1, Row2, Row3);
	public override string ToString()
		=> string.Create(CultureInfo.InvariantCulture,
			$"[{M00:#,##0.###} {M01:#,##0.###} {M02:#,##0.###} {M03:#,##0.###}|{M10:#,##0.###} {M11:#,##0.###} {M12:#,##0.###} {M13:#,##0.###}|{M20:#,##0.###} {M21:#,##0.###} {M22:#,##0.###} {M23:#,##0.###}|{M30:#,##0.###} {M31:#,##0.###} {M32:#,##0.###} {M33:#,##0.###}]");
}
