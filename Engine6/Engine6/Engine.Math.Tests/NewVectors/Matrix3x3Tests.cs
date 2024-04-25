using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Matrix3x3Tests {
	[Test]
	public void ConstructorAndProperties() {
		double dummy;
		Matrix3x3<double> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Assert.That( m.M00, Is.EqualTo( 1 ) );
		Assert.That( m.M01, Is.EqualTo( 2 ) );
		Assert.That( m.M02, Is.EqualTo( 3 ) );
		Assert.That( m.M10, Is.EqualTo( 4 ) );
		Assert.That( m.M11, Is.EqualTo( 5 ) );
		Assert.That( m.M12, Is.EqualTo( 6 ) );
		Assert.That( m.M20, Is.EqualTo( 7 ) );
		Assert.That( m.M21, Is.EqualTo( 8 ) );
		Assert.That( m.M22, Is.EqualTo( 9 ) );
		Assert.That( m.Rows, Is.EqualTo( 3 ) );
		Assert.That( m.Columns, Is.EqualTo( 3 ) );

		Assert.That( m.Row0, Is.EqualTo( new Vector3<double>( 1, 2, 3 ) ) );
		Assert.That( m.Row1, Is.EqualTo( new Vector3<double>( 4, 5, 6 ) ) );
		Assert.That( m.Row2, Is.EqualTo( new Vector3<double>( 7, 8, 9 ) ) );
		Assert.That( m.Col0, Is.EqualTo( new Vector3<double>( 1, 4, 7 ) ) );
		Assert.That( m.Col1, Is.EqualTo( new Vector3<double>( 2, 5, 8 ) ) );
		Assert.That( m.Col2, Is.EqualTo( new Vector3<double>( 3, 6, 9 ) ) );

		Assert.That( m[ 0, 0 ], Is.EqualTo( 1 ) );
		Assert.That( m[ 0, 1 ], Is.EqualTo( 2 ) );
		Assert.That( m[ 0, 2 ], Is.EqualTo( 3 ) );
		Assert.That( m[ 1, 0 ], Is.EqualTo( 4 ) );
		Assert.That( m[ 1, 1 ], Is.EqualTo( 5 ) );
		Assert.That( m[ 1, 2 ], Is.EqualTo( 6 ) );
		Assert.That( m[ 2, 0 ], Is.EqualTo( 7 ) );
		Assert.That( m[ 2, 1 ], Is.EqualTo( 8 ) );
		Assert.That( m[ 2, 2 ], Is.EqualTo( 9 ) );

		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 3, 0 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 3, 1 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 3, 2 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 0, 3 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 1, 3 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 2, 3 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 3, 3 ] );
	}

	[Test]
	public void Identities() {
		Assert.That( Matrix3x3<double>.AdditiveIdentity, Is.EqualTo( new Matrix3x3<double>( 0, 0, 0, 0, 0, 0, 0, 0, 0 ) ) );
		Assert.That( Matrix3x3<double>.MultiplicativeIdentity, Is.EqualTo( new Matrix3x3<double>( 1, 0, 0, 0, 1, 0, 0, 0, 1 ) ) );
		Assert.That( Matrix3x3<double>.Zero, Is.EqualTo( new Matrix3x3<double>( 0, 0, 0, 0, 0, 0, 0, 0, 0 ) ) );
		Assert.That( Matrix3x3<double>.One, Is.EqualTo( new Matrix3x3<double>( 1, 1, 1, 1, 1, 1, 1, 1, 1 ) ) );
		Assert.That( Matrix3x3<double>.Two, Is.EqualTo( new Matrix3x3<double>( 2, 0, 0, 0, 2, 0, 0, 0, 2 ) ) );
	}

	[Test]
	public void Operators() {
		Matrix3x3<double> m1 = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m2 = new(
			10, 11, 12,
			13, 14, 15,
			16, 17, 18
		);

		Assert.That( -m1, Is.EqualTo( new Matrix3x3<double>( -1, -2, -3, -4, -5, -6, -7, -8, -9 ) ) );
		Assert.That( m1 + m2, Is.EqualTo( new Matrix3x3<double>( 11, 13, 15, 17, 19, 21, 23, 25, 27 ) ) );
		Assert.That( m1 - m2, Is.EqualTo( new Matrix3x3<double>( -9, -9, -9, -9, -9, -9, -9, -9, -9 ) ) );
		Assert.That( m1 * 2, Is.EqualTo( new Matrix3x3<double>( 2, 4, 6, 8, 10, 12, 14, 16, 18 ) ) );
		Assert.That( 2 * m1, Is.EqualTo( new Matrix3x3<double>( 2, 4, 6, 8, 10, 12, 14, 16, 18 ) ) );
		Assert.That( m1 / 2, Is.EqualTo( new Matrix3x3<double>( 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5 ) ) );
		Assert.That( 362880 / m1, Is.EqualTo( new Matrix3x3<double>( 362880, 181440, 120960, 90720, 72576, 60480, 51840, 45360, 40320 ) ) );
		Assert.That( m1 * m2, Is.EqualTo( new Matrix3x3<double>( 84, 90, 96, 201, 216, 231, 318, 342, 366 ) ) );

		Vector3<double> v = new( 1, 2, 3 );
		Assert.That( m1 * v, Is.EqualTo( new Vector3<double>( 14, 32, 50 ) ) );
	}

	[Test]
	public void Dot() {
		Matrix3x3<double> m1 = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m2 = new(
			10, 11, 12,
			13, 14, 15,
			16, 17, 18
		);

		Assert.That( m1.Dot( m2 ), Is.EqualTo( 690 ) );
	}

	[Test]
	public void Determinant() {
		Matrix3x3<double> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Assert.That( m.GetDeterminant(), Is.EqualTo( 0 ) );
	}

	[Test]
	public void Transpose() {
		Matrix3x3<double> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Matrix3x3<double> transposed = m.GetTransposed();

		Assert.That( transposed, Is.EqualTo( new Matrix3x3<double>( 1, 4, 7, 2, 5, 8, 3, 6, 9 ) ) );
	}

	[Test]
	public void TryGetInverse() {
		//Sucess
		Matrix3x3<double> m1 = new(
			1, 2, 3,
			3, 1, 2,
			4, 3, 1
		);

		bool success1 = m1.TryGetInverse( out Matrix3x3<double> inverse1 );

		Assert.That( success1, Is.True );

		Matrix3x3<double> m1Identity = m1 * inverse1;
		Assert.That( m1Identity.M00, Is.EqualTo( 1 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M01, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M02, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M10, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M11, Is.EqualTo( 1 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M12, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M20, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M21, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M22, Is.EqualTo( 1 ).Within( 0.00001 ) );
		//Fail
		Matrix3x3<double> m2 = new(
			0, 0, 0,
			0, 0, 0,
			0, 0, 0
		);

		bool success2 = m2.TryGetInverse( out Matrix3x3<double> inverse2 );

		Assert.That( success2, Is.False );
		Assert.That( inverse2, Is.EqualTo( new Matrix3x3<double>( 0, 0, 0, 0, 0, 0, 0, 0, 0 ) ) );
	}

	[Test]
	public void AlternateConstruction() {
		Vector3<double> a = new( 1, 2, 3 );
		Vector3<double> b = new( 4, 5, 6 );
		Vector3<double> c = new( 7, 8, 9 );

		Matrix3x3<double> fromRows = Matrix3x3<double>.ConstructFromRows( a, b, c );
		Matrix3x3<double> fromCols = Matrix3x3<double>.ConstructFromColumns( a, b, c );

		Assert.That( fromRows, Is.EqualTo( new Matrix3x3<double>( 1, 2, 3, 4, 5, 6, 7, 8, 9 ) ) );
		Assert.That( fromCols, Is.EqualTo( new Matrix3x3<double>( 1, 4, 7, 2, 5, 8, 3, 6, 9 ) ) );

		Matrix3x3<double> from2x2 = new( 1, 2, 3, 4 );

		Assert.That( from2x2, Is.EqualTo( new Matrix3x3<double>( 1, 2, 0, 3, 4, 0, 0, 0, 1 ) ) );
	}

	[Test]
	public void Equality() {
		Matrix3x3<double> m1 = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m2 = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m3 = new(
			4, 5, 6,
			8, 9, 1,
			2, 3, 4
		);

		Assert.That( m1 == m2, Is.True );
		Assert.That( m1 == m3, Is.False );
		Assert.That( m1 != m2, Is.False );
		Assert.That( m1 != m3, Is.True );
		Assert.That( m1.Equals( m2 ), Is.True );
		Assert.That( m1.Equals( m3 ), Is.False );
		Assert.That( m1.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Matrix3x3<double> m1 = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m2 = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m3 = new(
			4, 5, 6,
			8, 9, 1,
			2, 3, 4
		);
		Matrix3x3<double> m4 = new(
			4, 5, 6,
			8, 9, 1,
			2, 3, 4
		);

		Assert.That( m1.GetHashCode(), Is.EqualTo( m2.GetHashCode() ) );
		Assert.That( m1.GetHashCode(), Is.Not.EqualTo( m3.GetHashCode() ) );
		Assert.That( m3.GetHashCode(), Is.EqualTo( m4.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Matrix3x3<double> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);
		Matrix3x3<double> m2 = new(
			1.1, 2.22, 3.333,
			4.4444, 5.55555, 6.666666,
			7.7777777, 8.88888888, 9.999999999
		);

		Assert.That( m.ToString(), Is.EqualTo( "[1 2 3|4 5 6|7 8 9]" ) );
		Assert.That( m2.ToString(), Is.EqualTo( "[1.1 2.22 3.333|4.444 5.556 6.667|7.778 8.889 10]" ) );
	}

	[Test]
	public void Excluding() {
		Matrix3x3<double> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Assert.That( m.Excluding00, Is.EqualTo( new Matrix2x2<double>( 5, 6, 8, 9 ) ) );
		Assert.That( m.Excluding01, Is.EqualTo( new Matrix2x2<double>( 4, 6, 7, 9 ) ) );
		Assert.That( m.Excluding02, Is.EqualTo( new Matrix2x2<double>( 4, 5, 7, 8 ) ) );
		Assert.That( m.Excluding10, Is.EqualTo( new Matrix2x2<double>( 2, 3, 8, 9 ) ) );
		Assert.That( m.Excluding11, Is.EqualTo( new Matrix2x2<double>( 1, 3, 7, 9 ) ) );
		Assert.That( m.Excluding12, Is.EqualTo( new Matrix2x2<double>( 1, 2, 7, 8 ) ) );
		Assert.That( m.Excluding20, Is.EqualTo( new Matrix2x2<double>( 2, 3, 5, 6 ) ) );
		Assert.That( m.Excluding21, Is.EqualTo( new Matrix2x2<double>( 1, 3, 4, 6 ) ) );
		Assert.That( m.Excluding22, Is.EqualTo( new Matrix2x2<double>( 1, 2, 4, 5 ) ) );
	}

	[Test]
	public void CastTo4x4() {
		Matrix3x3<double> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Matrix4x4<double> m4 = (Matrix4x4<double>) m;

		Assert.That( m4, Is.EqualTo( new Matrix4x4<double>( 1, 2, 3, 0, 4, 5, 6, 0, 7, 8, 9, 0, 0, 0, 0, 1 ) ) );
	}
}
