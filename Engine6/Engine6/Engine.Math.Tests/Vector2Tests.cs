namespace Engine.Math.Tests;

[TestFixture]
public sealed class Vector2Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Vector2<int> vectorA = new( 1, 2 );
		Vector2<double> vectorB = new( 1.0, 2.0 );

		Assert.That( vectorA.X, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Y, Is.EqualTo( 2 ) );

		Assert.That( vectorB.X, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Y, Is.EqualTo( 2.0 ) );
	}

	[Test]
	public void Identities() {
		Vector2<int> addtitiveIdentityA = Vector2<int>.AdditiveIdentity;
		Vector2<int> multiplicativeIdentityA = Vector2<int>.MultiplicativeIdentity;
		Vector2<int> zeroA = Vector2<int>.Zero;
		Vector2<int> oneA = Vector2<int>.One;
		Vector2<double> addtitiveIdentityB = Vector2<double>.AdditiveIdentity;
		Vector2<double> multiplicativeIdentityB = Vector2<double>.MultiplicativeIdentity;
		Vector2<double> zeroB = Vector2<double>.Zero;
		Vector2<double> oneB = Vector2<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.X, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Y, Is.EqualTo( 0 ) );

		Assert.That( oneA.X, Is.EqualTo( 1 ) );
		Assert.That( oneA.Y, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.X, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Y, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.X, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Y, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Vector2<int>( 1, 2 ) == new Vector2<int>( 1, 2 );
		bool equalX_isfalse = new Vector2<int>( 1, 2 ) == new Vector2<int>( 0, 2 );
		bool equalY_isfalse = new Vector2<int>( 1, 2 ) == new Vector2<int>( 1, 0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Vector2<int>( 1, 2 ) != new Vector2<int>( 1, 2 );
		bool notequalX = new Vector2<int>( 1, 2 ) != new Vector2<int>( 0, 2 );
		bool notequalY = new Vector2<int>( 1, 2 ) != new Vector2<int>( 1, 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Vector2<double>( 1.0, 2.0 ) == new Vector2<double>( 1.0, 2.0 );
		bool equalX_isfalse = new Vector2<double>( 1.0, 2.0 ) == new Vector2<double>( 0.0, 2.0 );
		bool equalY_isfalse = new Vector2<double>( 1.0, 2.0 ) == new Vector2<double>( 1.0, 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Vector2<double>( 1.0, 2.0 ) != new Vector2<double>( 1.0, 2.0 );
		bool notequalX = new Vector2<double>( 1.0, 2.0 ) != new Vector2<double>( 0.0, 2.0 );
		bool notequalY = new Vector2<double>( 1.0, 2.0 ) != new Vector2<double>( 1.0, 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Vector2<int> vectorA = new( 1, 2 );

		Vector2<int> expected = new( -1, -2 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Vector2<int> vectorA = new( 1, 2 );
		Vector2<int> vectorB = new( 2, 3 );

		Vector2<int> expected = new( 3, 5 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Vector2<int> vectorA = new( 1, 2 );
		Vector2<int> vectorB = new( 2, 3 );

		Vector2<int> expected = new( -1, -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Vector2<int> vectorA = new( 1, 2 );

		Vector2<int> expected = new( 2, 4 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Vector2<int> vectorA = new( 2, 4 );

		Vector2<int> expected = new( 1, 2 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Vector2<int> vectorA = new( 2, 4 );

		Vector2<int> expected = new( 4, 2 );
		Assert.That( 8 / vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void MultiplyVector2Operator() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 3 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyBivector2Operator() {
		Vector2<int> a = new( 1, 2 );
		Bivector2<int> b = new( 2 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyRotor2Operator() {
		Vector2<int> a = new( 1, 2 );
		Rotor2<int> b = new( 2, 3 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMultivector2Operator() {
		Vector2<int> a = new( 1, 2 );
		Multivector2<int> b = new( 2, 3, 4, 5 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMatrix2x2Operator() {
		Vector2<int> a = new( 1, 2 );
		Matrix2x2<int> b = new( 2, 0, 0, 1 );
		Matrix2x2<int> c = new( 2, 1, 0, 0 );
		Matrix2x2<int> d = new( 0, 0, 2, 1 );

		Vector2<int> expected_ab = new( 2, 2 );
		Vector2<int> expected_ac = new( 2, 1 );
		Vector2<int> expected_ad = new( 4, 2 );

		Assert.That( a * b, Is.EqualTo( expected_ab ) );
		Assert.That( a * c, Is.EqualTo( expected_ac ) );
		Assert.That( a * d, Is.EqualTo( expected_ad ) );
	}

	[Test]
	public void ComparisonOperators() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 3 );
		Vector2<int> c = new( 2, 2 );
		Vector2<int> d = new( 1, 2 );

		Assert.That( a < b, Is.True );
		Assert.That( a > b, Is.False );
		Assert.That( a <= b, Is.True );
		Assert.That( a >= b, Is.False );
		Assert.That( a < c, Is.False );
		Assert.That( a > c, Is.False );
		Assert.That( a <= c, Is.True );
		Assert.That( a >= c, Is.False );
		Assert.That( a > d, Is.False );
		Assert.That( a < d, Is.False );
		Assert.That( a >= d, Is.True );
		Assert.That( a <= d, Is.True );
		Assert.That( b < c, Is.False );
		Assert.That( b > c, Is.False );
		Assert.That( b <= c, Is.False );
		Assert.That( b >= c, Is.True );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Vector2<int> a = new( 1, 2 );

		Multivector2<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 1 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 2 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 0 ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 3 );

		Vector2<int> expected = new( 2, 6 );

		Assert.That( a.MultiplyEntrywise( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideEntrywise() {
		Vector2<int> a = new( 4, 6 );
		Vector2<int> b = new( 2, 3 );

		Vector2<int> expected = new( 2, 2 );

		Assert.That( a.DivideEntrywise( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void EntrywiseOperation() {
		Vector2<int> a = new( 1, 2 );

		Vector2<int> expected = new( 2, 4 );

		Assert.That( a.EntrywiseOperation( x => x * 2 ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Dot() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 3 );

		int dot = a.Dot( b );

		Assert.That( dot, Is.EqualTo( 8 ) );
	}

	[Test]
	public void MagnitudeSquared() {
		Vector2<int> a = new( 1, 2 );

		int magnitudeSquared = a.MagnitudeSquared();

		Assert.That( magnitudeSquared, Is.EqualTo( 5 ) );
	}

	[Test]
	public void Wedge() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 3 );

		Bivector2<int> wedge = a.Wedge( b );

		Assert.That( wedge.XY, Is.EqualTo( -1 ) );
	}

	[Test]
	public void Min() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 1 );

		Vector2<int> expected = new( 1, 1 );

		Assert.That( a.Min( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Max() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 2, 1 );

		Vector2<int> expected = new( 2, 2 );

		Assert.That( a.Max( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void SumOfParts() {
		Vector2<int> a = new( 3, 5 );

		Assert.That( a.SumOfParts(), Is.EqualTo( 8 ) );
	}

	[Test]
	public void ProductOfParts() {
		Vector2<int> a = new( 3, 5 );

		Assert.That( a.ProductOfParts(), Is.EqualTo( 15 ) );
	}

	[Test]
	public void SumOfUnitBasisAreas() {
		Vector2<int> a = new( 3, 5 );

		Assert.That( a.SumOfUnitBasisAreas(), Is.EqualTo( 15 ) );
	}

	[Test]
	public void SumOfUnitBasisVolumes() {
		Vector2<int> a = new( 3, 5 );

		Assert.That( a.SumOfUnitBasisVolumes(), Is.EqualTo( 0 ) );
	}

	[Test]
	public void ReflectNormal() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> normal = new( 0, 1 );

		Vector2<int> expected = new( -1, 2 );

		Assert.That( a.ReflectNormal( normal ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Test_Equals() {
		Vector2<int> a = new( 1, 2 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Vector2<int>( 1, 2 ) ), Is.True );
		Assert.That( a.Equals( new Vector2<int>( 2, -3 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Vector2<int> a = new( 1, 2 );
		Vector2<int> b = new( 1, 2 );
		Vector2<int> c = new( 2, 5 );
		Vector2<int> d = new( 2, 5 );

		Assert.That( a.GetHashCode(), Is.EqualTo( b.GetHashCode() ) );
		Assert.That( a.GetHashCode(), Is.Not.EqualTo( c.GetHashCode() ) );
		Assert.That( c.GetHashCode(), Is.EqualTo( d.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Vector2<int> a = new( 1, -56300 );
		Vector2<double> b = new( -69.4201, 7031 );
		Vector2<int> allNegative = new( -1, -1 );

		Assert.That( a.ToString(), Is.EqualTo( "[1X - 56,300Y]" ) );
		Assert.That( b.ToString(), Is.EqualTo( "[-69.42X + 7,031Y]" ) );
		Assert.That( allNegative.ToString(), Is.EqualTo( "[-1X - 1Y]" ) );
	}
	#endregion

	#region Casts
	[Test]
	public void CastFromScalar() {
		int value = 5;
		Vector2<int> vector = value;

		Vector2<int> expected = new( 5, 5 );

		Assert.That( vector, Is.EqualTo( expected ) );
	}

	[Test]
	public void CastFromTuple() {
		(int, int) tuple = (5, 7);
		Vector2<int> vector = tuple;

		Vector2<int> expected = new( 5, 7 );

		Assert.That( vector, Is.EqualTo( expected ) );
	}
	#endregion
}