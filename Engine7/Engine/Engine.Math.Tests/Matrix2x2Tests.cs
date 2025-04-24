namespace Engine.Math.Tests;

[TestFixture]
public sealed class Matrix2x2Tests {
	[Test]
	public void ConstructorAndProperties() {
		double dummy;
		Matrix2x2<double> m = new( 1, 2, 3, 4 );

		Assert.That( m.M00, Is.EqualTo( 1 ) );
		Assert.That( m.M01, Is.EqualTo( 2 ) );
		Assert.That( m.M10, Is.EqualTo( 3 ) );
		Assert.That( m.M11, Is.EqualTo( 4 ) );
		Assert.That( m.Rows, Is.EqualTo( 2 ) );
		Assert.That( m.Columns, Is.EqualTo( 2 ) );

		Assert.That( m.Row0, Is.EqualTo( new Vector2<double>( 1, 2 ) ) );
		Assert.That( m.Row1, Is.EqualTo( new Vector2<double>( 3, 4 ) ) );
		Assert.That( m.Col0, Is.EqualTo( new Vector2<double>( 1, 3 ) ) );
		Assert.That( m.Col1, Is.EqualTo( new Vector2<double>( 2, 4 ) ) );

		Assert.That( m[ 0, 0 ], Is.EqualTo( 1 ) );
		Assert.That( m[ 0, 1 ], Is.EqualTo( 2 ) );
		Assert.That( m[ 1, 0 ], Is.EqualTo( 3 ) );
		Assert.That( m[ 1, 1 ], Is.EqualTo( 4 ) );

		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 2, 0 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 2, 1 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 0, 2 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 1, 2 ] );
		Assert.Throws<ArgumentOutOfRangeException>( () => dummy = m[ 2, 2 ] );
	}

	[Test]
	public void Identities() {
		Assert.That( Matrix2x2<double>.AdditiveIdentity, Is.EqualTo( new Matrix2x2<double>( 0, 0, 0, 0 ) ) );
		Assert.That( Matrix2x2<double>.MultiplicativeIdentity, Is.EqualTo( new Matrix2x2<double>( 1, 0, 0, 1 ) ) );
		Assert.That( Matrix2x2<double>.Zero, Is.EqualTo( new Matrix2x2<double>( 0, 0, 0, 0 ) ) );
		Assert.That( Matrix2x2<double>.One, Is.EqualTo( new Matrix2x2<double>( 1, 1, 1, 1 ) ) );
		Assert.That( Matrix2x2<double>.Two, Is.EqualTo( new Matrix2x2<double>( 2, 0, 0, 2 ) ) );
	}

	[Test]
	public void Operators() {
		Matrix2x2<double> m1 = new( 1, 2, 3, 4 );
		Matrix2x2<double> m2 = new( 5, 6, 7, 8 );

		Assert.That( -m1, Is.EqualTo( new Matrix2x2<double>( -1, -2, -3, -4 ) ) );
		Assert.That( m1 + m2, Is.EqualTo( new Matrix2x2<double>( 6, 8, 10, 12 ) ) );
		Assert.That( m1 - m2, Is.EqualTo( new Matrix2x2<double>( -4, -4, -4, -4 ) ) );
		Assert.That( m1 * 2, Is.EqualTo( new Matrix2x2<double>( 2, 4, 6, 8 ) ) );
		Assert.That( 2 * m1, Is.EqualTo( new Matrix2x2<double>( 2, 4, 6, 8 ) ) );
		Assert.That( m1 / 2, Is.EqualTo( new Matrix2x2<double>( 0.5, 1, 1.5, 2 ) ) );
		Assert.That( 24 / m1, Is.EqualTo( new Matrix2x2<double>( 24, 12, 8, 6 ) ) );
		Assert.That( m1 * m2, Is.EqualTo( new Matrix2x2<double>( 19, 22, 43, 50 ) ) );

		Vector2<double> v = new( 1, 2 );
		Assert.That( m1 * v, Is.EqualTo( new Vector2<double>( 5, 11 ) ) );
	}

	[Test]
	public void Dot() {
		Matrix2x2<double> m1 = new( 1, 2, 3, 4 );
		Matrix2x2<double> m2 = new( 5, 6, 7, 8 );

		Assert.That( m1.Dot( m2 ), Is.EqualTo( 70 ) );
	}

	[Test]
	public void MagnitudeSquared() {
		Matrix2x2<double> m = new( 1, 2, 3, 4 );

		Assert.That( m.MagnitudeSquared(), Is.EqualTo( 30 ) );
	}

	[Test]
	public void Determinant() {
		Matrix2x2<double> m = new( 1, 2, 3, 4 );

		Assert.That( m.GetDeterminant(), Is.EqualTo( -2 ) );
	}

	[Test]
	public void Transpose() {
		Matrix2x2<double> m = new( 1, 2, 3, 4 );

		Assert.That( m.GetTransposed(), Is.EqualTo( new Matrix2x2<double>( 1, 3, 2, 4 ) ) );
	}

	[Test]
	public void TryGetInverse() {
		//Sucess
		Matrix2x2<double> m1 = new( 1, 2, 3, 4 );

		bool success1 = m1.TryGetInverse( out Matrix2x2<double> inverse1 );

		Assert.That( success1, Is.True );

		Matrix2x2<double> m1Identity = m1 * inverse1;
		Assert.That( m1Identity.M00, Is.EqualTo( 1 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M01, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M10, Is.EqualTo( 0 ).Within( 0.00001 ) );
		Assert.That( m1Identity.M11, Is.EqualTo( 1 ).Within( 0.00001 ) );
		//Fail
		Matrix2x2<double> m2 = new( 0, 0, 0, 0 );

		bool success2 = m2.TryGetInverse( out Matrix2x2<double> inverse2 );

		Assert.That( success2, Is.False );
		Assert.That( inverse2, Is.EqualTo( new Matrix2x2<double>( 0, 0, 0, 0 ) ) );
	}

	[Test]
	public void AlternateConstruction() {
		Vector2<double> a = new( 1, 2 );
		Vector2<double> b = new( 3, 4 );

		Matrix2x2<double> fromRows = Matrix2x2<double>.ConstructFromRows( a, b );
		Matrix2x2<double> fromCols = Matrix2x2<double>.ConstructFromColumns( a, b );

		Assert.That( fromRows, Is.EqualTo( new Matrix2x2<double>( 1, 2, 3, 4 ) ) );
		Assert.That( fromCols, Is.EqualTo( new Matrix2x2<double>( 1, 3, 2, 4 ) ) );
	}

	[Test]
	public void Equality() {
		Matrix2x2<double> m1 = new( 1, 2, 3, 4 );
		Matrix2x2<double> m2 = new( 1, 2, 3, 4 );
		Matrix2x2<double> m3 = new( 5, 6, 7, 8 );

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
		Matrix2x2<double> m1 = new( 1, 2, 3, 4 );
		Matrix2x2<double> m2 = new( 1, 2, 3, 4 );
		Matrix2x2<double> m3 = new( 5, 6, 7, 8 );
		Matrix2x2<double> m4 = new( 5, 6, 7, 8 );

		Assert.That( m1.GetHashCode(), Is.EqualTo( m2.GetHashCode() ) );
		Assert.That( m1.GetHashCode(), Is.Not.EqualTo( m3.GetHashCode() ) );
		Assert.That( m3.GetHashCode(), Is.EqualTo( m4.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Matrix2x2<double> m = new( 1, 2, 3000, 4 );
		Matrix2x2<double> m1 = new( 1.3, 2.03, 3.003, 4.0003 );

		Assert.That( m.ToString(), Is.EqualTo( "[1 2|3,000 4]" ) );
		Assert.That( m1.ToString(), Is.EqualTo( "[1.3 2.03|3.003 4]" ) );
	}
}
