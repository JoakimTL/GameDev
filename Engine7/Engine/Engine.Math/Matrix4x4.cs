using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Engine;

/// <summary>
/// Matrices are stored in row-major order.
/// </summary>
[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Matrix4x4<TScalar>( TScalar m00, TScalar m01, TScalar m02, TScalar m03, TScalar m10, TScalar m11, TScalar m12, TScalar m13, TScalar m20, TScalar m21, TScalar m22, TScalar m23, TScalar m30, TScalar m31, TScalar m32, TScalar m33 ) :
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
		: unmanaged, INumber<TScalar> {

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
	public TScalar this[ uint row, uint col ]
		=> row switch {
			0 => col switch {
				0 => this.M00,
				1 => this.M01,
				2 => this.M02,
				3 => this.M03,
				_ => throw new ArgumentOutOfRangeException( nameof( col ) )
			},
			1 => col switch {
				0 => this.M10,
				1 => this.M11,
				2 => this.M12,
				3 => this.M13,
				_ => throw new ArgumentOutOfRangeException( nameof( col ) )
			},
			2 => col switch {
				0 => this.M20,
				1 => this.M21,
				2 => this.M22,
				3 => this.M23,
				_ => throw new ArgumentOutOfRangeException( nameof( col ) )
			},
			3 => col switch {
				0 => this.M30,
				1 => this.M31,
				2 => this.M32,
				3 => this.M33,
				_ => throw new ArgumentOutOfRangeException( nameof( col ) )
			},
			_ => throw new ArgumentOutOfRangeException( nameof( row ) )
		};

	public Vector4<TScalar> Row0 => new( this.M00, this.M01, this.M02, this.M03 );
	public Vector4<TScalar> Row1 => new( this.M10, this.M11, this.M12, this.M13 );
	public Vector4<TScalar> Row2 => new( this.M20, this.M21, this.M22, this.M23 );
	public Vector4<TScalar> Row3 => new( this.M30, this.M31, this.M32, this.M33 );
	public Vector4<TScalar> Col0 => new( this.M00, this.M10, this.M20, this.M30 );
	public Vector4<TScalar> Col1 => new( this.M01, this.M11, this.M21, this.M31 );
	public Vector4<TScalar> Col2 => new( this.M02, this.M12, this.M22, this.M32 );
	public Vector4<TScalar> Col3 => new( this.M03, this.M13, this.M23, this.M33 );

	public Matrix3x3<TScalar> Excluding00 => new( this.M11, this.M12, this.M13, this.M21, this.M22, this.M23, this.M31, this.M32, this.M33 );
	public Matrix3x3<TScalar> Excluding01 => new( this.M10, this.M12, this.M13, this.M20, this.M22, this.M23, this.M30, this.M32, this.M33 );
	public Matrix3x3<TScalar> Excluding02 => new( this.M10, this.M11, this.M13, this.M20, this.M21, this.M23, this.M30, this.M31, this.M33 );
	public Matrix3x3<TScalar> Excluding03 => new( this.M10, this.M11, this.M12, this.M20, this.M21, this.M22, this.M30, this.M31, this.M32 );
	public Matrix3x3<TScalar> Excluding10 => new( this.M01, this.M02, this.M03, this.M21, this.M22, this.M23, this.M31, this.M32, this.M33 );
	public Matrix3x3<TScalar> Excluding11 => new( this.M00, this.M02, this.M03, this.M20, this.M22, this.M23, this.M30, this.M32, this.M33 );
	public Matrix3x3<TScalar> Excluding12 => new( this.M00, this.M01, this.M03, this.M20, this.M21, this.M23, this.M30, this.M31, this.M33 );
	public Matrix3x3<TScalar> Excluding13 => new( this.M00, this.M01, this.M02, this.M20, this.M21, this.M22, this.M30, this.M31, this.M32 );
	public Matrix3x3<TScalar> Excluding20 => new( this.M01, this.M02, this.M03, this.M11, this.M12, this.M13, this.M31, this.M32, this.M33 );
	public Matrix3x3<TScalar> Excluding21 => new( this.M00, this.M02, this.M03, this.M10, this.M12, this.M13, this.M30, this.M32, this.M33 );
	public Matrix3x3<TScalar> Excluding22 => new( this.M00, this.M01, this.M03, this.M10, this.M11, this.M13, this.M30, this.M31, this.M33 );
	public Matrix3x3<TScalar> Excluding23 => new( this.M00, this.M01, this.M02, this.M10, this.M11, this.M12, this.M30, this.M31, this.M32 );
	public Matrix3x3<TScalar> Excluding30 => new( this.M01, this.M02, this.M03, this.M11, this.M12, this.M13, this.M21, this.M22, this.M23 );
	public Matrix3x3<TScalar> Excluding31 => new( this.M00, this.M02, this.M03, this.M10, this.M12, this.M13, this.M20, this.M22, this.M23 );
	public Matrix3x3<TScalar> Excluding32 => new( this.M00, this.M01, this.M03, this.M10, this.M11, this.M13, this.M20, this.M21, this.M23 );
	public Matrix3x3<TScalar> Excluding33 => new( this.M00, this.M01, this.M02, this.M10, this.M11, this.M12, this.M20, this.M21, this.M22 );

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
	public Matrix4x4( TScalar m00, TScalar m01, TScalar m02, TScalar m10, TScalar m11, TScalar m12, TScalar m20, TScalar m21, TScalar m22 ) : this(
		m00, m01, m02, TScalar.Zero,
		m10, m11, m12, TScalar.Zero,
		m20, m21, m22, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One
	) { }

	/// <summary>
	/// Creates a 4x4 multiplicative identity matrix overlaid by a 2x2 matrix.
	/// </summary>
	public Matrix4x4( TScalar m00, TScalar m01, TScalar m10, TScalar m11 ) : this(
		m00, m01, TScalar.Zero, TScalar.Zero,
		m10, m11, TScalar.Zero, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero,
		TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One
	) { }

	public Matrix4x4<TScalar> Negate()
		=> new(
			-this.M00, -this.M01, -this.M02, -this.M03,
			-this.M10, -this.M11, -this.M12, -this.M13,
			-this.M20, -this.M21, -this.M22, -this.M23,
			-this.M30, -this.M31, -this.M32, -this.M33
		);
	public Matrix4x4<TScalar> Add( in Matrix4x4<TScalar> r )
		=> new(
			this.M00 + r.M00, this.M01 + r.M01, this.M02 + r.M02, this.M03 + r.M03,
			this.M10 + r.M10, this.M11 + r.M11, this.M12 + r.M12, this.M13 + r.M13,
			this.M20 + r.M20, this.M21 + r.M21, this.M22 + r.M22, this.M23 + r.M23,
			this.M30 + r.M30, this.M31 + r.M31, this.M32 + r.M32, this.M33 + r.M33
		);
	public Matrix4x4<TScalar> Subtract( in Matrix4x4<TScalar> r )
		=> new(
			this.M00 - r.M00, this.M01 - r.M01, this.M02 - r.M02, this.M03 - r.M03,
			this.M10 - r.M10, this.M11 - r.M11, this.M12 - r.M12, this.M13 - r.M13,
			this.M20 - r.M20, this.M21 - r.M21, this.M22 - r.M22, this.M23 - r.M23,
			this.M30 - r.M30, this.M31 - r.M31, this.M32 - r.M32, this.M33 - r.M33
		);
	public Matrix4x4<TScalar> ScalarMultiply( TScalar r )
		=> new(
			this.M00 * r, this.M01 * r, this.M02 * r, this.M03 * r,
			this.M10 * r, this.M11 * r, this.M12 * r, this.M13 * r,
			this.M20 * r, this.M21 * r, this.M22 * r, this.M23 * r,
			this.M30 * r, this.M31 * r, this.M32 * r, this.M33 * r
		);
	public Matrix4x4<TScalar> ScalarDivide( TScalar r )
		=> new(
			this.M00 / r, this.M01 / r, this.M02 / r, this.M03 / r,
			this.M10 / r, this.M11 / r, this.M12 / r, this.M13 / r,
			this.M20 / r, this.M21 / r, this.M22 / r, this.M23 / r,
			this.M30 / r, this.M31 / r, this.M32 / r, this.M33 / r
		);
	public static Matrix4x4<TScalar> DivideScalar( TScalar l, in Matrix4x4<TScalar> r )
		=> new(
			l / r.M00, l / r.M01, l / r.M02, l / r.M03,
			l / r.M10, l / r.M11, l / r.M12, l / r.M13,
			l / r.M20, l / r.M21, l / r.M22, l / r.M23,
			l / r.M30, l / r.M31, l / r.M32, l / r.M33
		);
	public TScalar Dot( in Matrix4x4<TScalar> r ) =>
		this.M00 * r.M00 + this.M01 * r.M01 + this.M02 * r.M02 + this.M03 * r.M03 +
		this.M10 * r.M10 + this.M11 * r.M11 + this.M12 * r.M12 + this.M13 * r.M13 +
		this.M20 * r.M20 + this.M21 * r.M21 + this.M22 * r.M22 + this.M23 * r.M23 +
		this.M30 * r.M30 + this.M31 * r.M31 + this.M32 * r.M32 + this.M33 * r.M33;
	public TScalar MagnitudeSquared() => Dot( this );

	public TScalar GetDeterminant()
		=> this.M00 * this.Excluding00.GetDeterminant() - this.M01 * this.Excluding01.GetDeterminant() + this.M02 * this.Excluding02.GetDeterminant() - this.M03 * this.Excluding03.GetDeterminant();
	public Matrix4x4<TScalar> GetTransposed()
		=> new(
			this.M00, this.M10, this.M20, this.M30,
			this.M01, this.M11, this.M21, this.M31,
			this.M02, this.M12, this.M22, this.M32,
			this.M03, this.M13, this.M23, this.M33
		);
	public bool TryGetInverse( out Matrix4x4<TScalar> matrix ) {
		TScalar determinant = GetDeterminant();
		if (TScalar.IsZero( determinant )) {
			matrix = default;
			return false;
		}
		matrix = new Matrix4x4<TScalar>(
			this.Excluding00.GetDeterminant(), -this.Excluding10.GetDeterminant(), this.Excluding20.GetDeterminant(), -this.Excluding30.GetDeterminant(),
			-this.Excluding01.GetDeterminant(), this.Excluding11.GetDeterminant(), -this.Excluding21.GetDeterminant(), this.Excluding31.GetDeterminant(),
			this.Excluding02.GetDeterminant(), -this.Excluding12.GetDeterminant(), this.Excluding22.GetDeterminant(), -this.Excluding32.GetDeterminant(),
			-this.Excluding03.GetDeterminant(), this.Excluding13.GetDeterminant(), -this.Excluding23.GetDeterminant(), this.Excluding33.GetDeterminant()
		) / determinant;
		return true;
	}

	public Vector2<TScalar>? TransformWorld( in Vector2<TScalar> vector ) {
		Vector4<TScalar> transformVector = new( vector.X, vector.Y, TScalar.Zero, TScalar.One );
		Vector4<TScalar> result = this * transformVector;
		if (TScalar.IsZero( result.W ))
			return null;
		return new Vector2<TScalar>( result.X, result.Y ) / result.W;
	}
	public Vector2<TScalar> TransformNormal( in Vector2<TScalar> vector ) {
		Vector4<TScalar> transformVector = new( vector.X, vector.Y, TScalar.Zero, TScalar.Zero );
		Vector4<TScalar> result = this * transformVector;
		return new Vector2<TScalar>( result.X, result.Y );
	}
	public Vector3<TScalar>? TransformWorld( in Vector3<TScalar> vector ) {
		Vector4<TScalar> transformVector = new( vector.X, vector.Y, vector.Z, TScalar.One );
		Vector4<TScalar> result = this * transformVector;
		if (TScalar.IsZero( result.W ))
			return null;
		return new Vector3<TScalar>( result.X, result.Y, result.Z ) / result.W;
	}
	public Vector3<TScalar> TransformNormal( in Vector3<TScalar> vector ) {
		Vector4<TScalar> transformVector = new( vector.X, vector.Y, vector.Z, TScalar.Zero );
		Vector4<TScalar> result = this * transformVector;
		return new Vector3<TScalar>( result.X, result.Y, result.Z );
	}

	public Matrix4x4<TScalar> Multiply( in Matrix4x4<TScalar> r )
		=> new(
			this.Row0.Dot( r.Col0 ), this.Row0.Dot( r.Col1 ), this.Row0.Dot( r.Col2 ), this.Row0.Dot( r.Col3 ),
			this.Row1.Dot( r.Col0 ), this.Row1.Dot( r.Col1 ), this.Row1.Dot( r.Col2 ), this.Row1.Dot( r.Col3 ),
			this.Row2.Dot( r.Col0 ), this.Row2.Dot( r.Col1 ), this.Row2.Dot( r.Col2 ), this.Row2.Dot( r.Col3 ),
			this.Row3.Dot( r.Col0 ), this.Row3.Dot( r.Col1 ), this.Row3.Dot( r.Col2 ), this.Row3.Dot( r.Col3 )
		);
	public Vector4<TScalar> Multiply( in Vector4<TScalar> r )
		=> new( this.Row0.Dot( r ), this.Row1.Dot( r ), this.Row2.Dot( r ), this.Row3.Dot( r ) );
	public static Matrix4x4<TScalar> operator *( in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r ) => l.Multiply( r );
	public static Vector4<TScalar> operator *( in Matrix4x4<TScalar> l, in Vector4<TScalar> r ) => l.Multiply( r );

	public static Matrix4x4<TScalar> operator -( in Matrix4x4<TScalar> l ) => l.Negate();
	public static Matrix4x4<TScalar> operator +( in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r ) => l.Add( r );
	public static Matrix4x4<TScalar> operator -( in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r ) => l.Subtract( r );
	public static Matrix4x4<TScalar> operator *( in Matrix4x4<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Matrix4x4<TScalar> operator *( TScalar l, in Matrix4x4<TScalar> r ) => r.ScalarMultiply( l );
	public static Matrix4x4<TScalar> operator /( in Matrix4x4<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Matrix4x4<TScalar> operator /( TScalar l, in Matrix4x4<TScalar> r ) => DivideScalar( l, r );

	public static Matrix4x4<TScalar> ConstructFromRows( Vector4<TScalar> row0, Vector4<TScalar> row1, Vector4<TScalar> row2, Vector4<TScalar> row3 )
		=> new( row0.X, row0.Y, row0.Z, row0.W, row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W );
	public static Matrix4x4<TScalar> ConstructFromColumns( Vector4<TScalar> col0, Vector4<TScalar> col1, Vector4<TScalar> col2, Vector4<TScalar> col3 )
		=> new( col0.X, col1.X, col2.X, col3.X, col0.Y, col1.Y, col2.Y, col3.Y, col0.Z, col1.Z, col2.Z, col3.Z, col0.W, col1.W, col2.W, col3.W );

	public static bool operator ==( in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r ) => l.Row0 == r.Row0 && l.Row1 == r.Row1 && l.Row2 == r.Row2 && l.Row3 == r.Row3;
	public static bool operator !=( in Matrix4x4<TScalar> l, in Matrix4x4<TScalar> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Matrix4x4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( this.Row0, this.Row1, this.Row2, this.Row3 );
	public override string ToString()
		=> string.Create( CultureInfo.InvariantCulture,
			$"[{this.M00:#,##0.###} {this.M01:#,##0.###} {this.M02:#,##0.###} {this.M03:#,##0.###}|{this.M10:#,##0.###} {this.M11:#,##0.###} {this.M12:#,##0.###} {this.M13:#,##0.###}|{this.M20:#,##0.###} {this.M21:#,##0.###} {this.M22:#,##0.###} {this.M23:#,##0.###}|{this.M30:#,##0.###} {this.M31:#,##0.###} {this.M32:#,##0.###} {this.M33:#,##0.###}]" );
}
