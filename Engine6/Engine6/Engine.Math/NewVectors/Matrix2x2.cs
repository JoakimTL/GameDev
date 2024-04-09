using Engine.Math.NewVectors.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math.NewVectors;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Matrix2x2<TScalar>( TScalar m00, TScalar m01, TScalar m10, TScalar m11 ) :
		IVector<Matrix2x2<TScalar>, TScalar>,
		ILinearAlgebraOperators<Matrix2x2<TScalar>, TScalar>,
		IMatrix<TScalar>,
		ISquareMatrix<Matrix2x2<TScalar>>,
		IProduct<Matrix2x2<TScalar>, Matrix2x2<TScalar>, Matrix2x2<TScalar>>,
		IProduct<Matrix2x2<TScalar>, Vector2<TScalar>, Vector2<TScalar>>
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
		=> row switch {
			0 => col switch {
				0 => M00,
				1 => M01,
				_ => throw new System.ArgumentOutOfRangeException( nameof( col ) ),
			},
			1 => col switch {
				0 => M10,
				1 => M11,
				_ => throw new System.ArgumentOutOfRangeException( nameof( col ) ),
			},
			_ => throw new System.ArgumentOutOfRangeException( nameof( row ) ),
		};

	public Vector2<TScalar> Row0 => new( M00, M01 );
	public Vector2<TScalar> Row1 => new( M10, M11 );
	public Vector2<TScalar> Col0 => new( M00, M10 );
	public Vector2<TScalar> Col1 => new( M01, M11 );

	public static Matrix2x2<TScalar> AdditiveIdentity => Zero;
	public static Matrix2x2<TScalar> MultiplicativeIdentity { get; } = new( TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.One );
	public static Matrix2x2<TScalar> Zero { get; } = new( TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero );
	public static Matrix2x2<TScalar> One { get; } = new( TScalar.One, TScalar.One, TScalar.One, TScalar.One );

	public Matrix2x2<TScalar> Negate() => new( -M00, -M01, -M10, -M11 );
	public Matrix2x2<TScalar> Add( in Matrix2x2<TScalar> r ) => new( M00 + r.M00, M01 + r.M01, M10 + r.M10, M11 + r.M11 );
	public Matrix2x2<TScalar> Subtract( in Matrix2x2<TScalar> r ) => new( M00 - r.M00, M01 - r.M01, M10 - r.M10, M11 - r.M11 );
	public Matrix2x2<TScalar> ScalarMultiply( TScalar r ) => new( M00 * r, M01 * r, M10 * r, M11 * r );
	public Matrix2x2<TScalar> ScalarDivide( TScalar r ) => new( M00 / r, M01 / r, M10 / r, M11 / r );
	public static Matrix2x2<TScalar> DivideScalar( TScalar l, in Matrix2x2<TScalar> r ) => new( l / r.M00, l / r.M01, l / r.M10, l / r.M11 );
	public TScalar Dot( in Matrix2x2<TScalar> r ) => (M00 * r.M00) + (M01 * r.M01) + (M10 * r.M10) + (M11 * r.M11);

	public TScalar GetDeterminant() => (M00 * M11) - (M01 * M10);
	public Matrix2x2<TScalar> GetTransposed() => new( M00, M10, M01, M11 );
	public bool TryGetInverse( out Matrix2x2<TScalar> matrix ) {
		TScalar determinant = GetDeterminant();
		if (TScalar.IsZero( determinant )) {
			matrix = default;
			return false;
		}
		matrix = new Matrix2x2<TScalar>( M11, -M01, -M10, M00 ) / determinant;
		return true;
	}
	public bool TryGetUpperTriangular( out Matrix2x2<TScalar> upperTriangular, out bool negative ) => throw new NotImplementedException();
	public bool TryGetLowerTriangular( out Matrix2x2<TScalar> lowerTriangular, out bool negative ) => throw new NotImplementedException();

	public Matrix2x2<TScalar> Multiply( in Matrix2x2<TScalar> l )
		=> new(
			Row0.Dot( l.Col0 ), Row0.Dot( l.Col1 ),
			Row1.Dot( l.Col0 ), Row1.Dot( l.Col1 )
		);
	public Vector2<TScalar> Multiply( in Vector2<TScalar> l )
		=> new( Row0.Dot( l ), Row1.Dot( l ) );
	public static Matrix2x2<TScalar> operator *( in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r ) => l.Multiply( r );
	public static Vector2<TScalar> operator *( in Matrix2x2<TScalar> l, in Vector2<TScalar> r ) => l.Multiply( r );

	public static Matrix2x2<TScalar> operator -( in Matrix2x2<TScalar> l ) => l.Negate();
	public static Matrix2x2<TScalar> operator +( in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r ) => l.Add( r );
	public static Matrix2x2<TScalar> operator -( in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r ) => l.Subtract( r );
	public static Matrix2x2<TScalar> operator *( in Matrix2x2<TScalar> l, TScalar r ) => l.ScalarMultiply( r );
	public static Matrix2x2<TScalar> operator *( TScalar l, in Matrix2x2<TScalar> r ) => r.ScalarMultiply( l );
	public static Matrix2x2<TScalar> operator /( in Matrix2x2<TScalar> l, TScalar r ) => l.ScalarDivide( r );
	public static Matrix2x2<TScalar> operator /( TScalar l, in Matrix2x2<TScalar> r ) => DivideScalar( l, r );

	public static bool operator ==( in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r ) => l.Row0 == r.Row0 && l.Row1 == r.Row1;
	public static bool operator !=( in Matrix2x2<TScalar> l, in Matrix2x2<TScalar> r ) => !(l == r);

	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Matrix2x2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Row0, Row1 );
	public override string ToString() => $"[{Row0}, {Row1}]";
}
