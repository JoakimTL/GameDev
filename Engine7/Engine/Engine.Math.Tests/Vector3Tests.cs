namespace Engine.Math.Tests;

[TestFixture]
public sealed class Vector3Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Vector3<int> vectorA = new( 1, 2, 3 );
		Vector3<double> vectorB = new( 1.0, 2.0, 3.0 );

		Assert.That( vectorA.X, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Y, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Z, Is.EqualTo( 3 ) );

		Assert.That( vectorB.X, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Y, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Z, Is.EqualTo( 3.0 ) );
	}

	[Test]
	public void Identities() {
		Vector3<int> addtitiveIdentityA = Vector3<int>.AdditiveIdentity;
		Vector3<int> multiplicativeIdentityA = Vector3<int>.MultiplicativeIdentity;
		Vector3<int> zeroA = Vector3<int>.Zero;
		Vector3<int> oneA = Vector3<int>.One;
		Vector3<double> addtitiveIdentityB = Vector3<double>.AdditiveIdentity;
		Vector3<double> multiplicativeIdentityB = Vector3<double>.MultiplicativeIdentity;
		Vector3<double> zeroB = Vector3<double>.Zero;
		Vector3<double> oneB = Vector3<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.X, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Y, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Z, Is.EqualTo( 0 ) );

		Assert.That( oneA.X, Is.EqualTo( 1 ) );
		Assert.That( oneA.Y, Is.EqualTo( 1 ) );
		Assert.That( oneA.Z, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.X, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Y, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Z, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.X, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Y, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Z, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Vector3<int>( 1, 2, 3 ) == new Vector3<int>( 1, 2, 3 );
		bool equalX_isfalse = new Vector3<int>( 1, 2, 3 ) == new Vector3<int>( 0, 2, 3 );
		bool equalY_isfalse = new Vector3<int>( 1, 2, 3 ) == new Vector3<int>( 1, 0, 3 );
		bool equalZ_isfalse = new Vector3<int>( 1, 2, 3 ) == new Vector3<int>( 1, 2, 0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
		Assert.That( equalZ_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Vector3<int>( 1, 2, 3 ) != new Vector3<int>( 1, 2, 3 );
		bool notequalX = new Vector3<int>( 1, 2, 3 ) != new Vector3<int>( 0, 2, 3 );
		bool notequalY = new Vector3<int>( 1, 2, 3 ) != new Vector3<int>( 1, 0, 3 );
		bool notequalZ = new Vector3<int>( 1, 2, 3 ) != new Vector3<int>( 1, 2, 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
		Assert.That( notequalZ, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Vector3<double>( 1.0, 2.0, 3.0 ) == new Vector3<double>( 1.0, 2.0, 3.0 );
		bool equalX_isfalse = new Vector3<double>( 1.0, 2.0, 3.0 ) == new Vector3<double>( 0.0, 2.0, 3.0 );
		bool equalY_isfalse = new Vector3<double>( 1.0, 2.0, 3.0 ) == new Vector3<double>( 1.0, 0.0, 3.0 );
		bool equalZ_isfalse = new Vector3<double>( 1.0, 2.0, 3.0 ) == new Vector3<double>( 1.0, 2.0, 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
		Assert.That( equalZ_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Vector3<double>( 1.0, 2.0, 3.0 ) != new Vector3<double>( 1.0, 2.0, 3.0 );
		bool notequalX = new Vector3<double>( 1.0, 2.0, 3.0 ) != new Vector3<double>( 0.0, 2.0, 3.0 );
		bool notequalY = new Vector3<double>( 1.0, 2.0, 3.0 ) != new Vector3<double>( 1.0, 0.0, 3.0 );
		bool notequalZ = new Vector3<double>( 1.0, 2.0, 3.0 ) != new Vector3<double>( 1.0, 2.0, 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
		Assert.That( notequalZ, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Vector3<int> vectorA = new( 1, 2, 3 );

		Vector3<int> expected = new( -1, -2, -3 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Vector3<int> vectorA = new( 1, 2, 3 );
		Vector3<int> vectorB = new( 2, 3, 4 );

		Vector3<int> expected = new( 3, 5, 7 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Vector3<int> vectorA = new( 1, 2, 3 );
		Vector3<int> vectorB = new( 2, 3, 4 );

		Vector3<int> expected = new( -1, -1, -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Vector3<int> vectorA = new( 1, 2, 3 );

		Vector3<int> expected = new( 2, 4, 6 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Vector3<int> vectorA = new( 2, 4, 6 );

		Vector3<int> expected = new( 1, 2, 3 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Vector3<int> vectorA = new( 2, 4, 8 );

		Vector3<int> expected = new( 16, 8, 4 );
		Assert.That( 32 / vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void MultiplyVector3Operator() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 2, 3, 4 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyBivector3Operator() {
		Vector3<int> a = new( 1, 2, 3 );
		Bivector3<int> b = new( 4, 5, 6 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyTrivector3Operator() {
		Vector3<int> a = new( 1, 2, 3 );
		Trivector3<int> b = new( 4 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyRotor3Operator() {
		Vector3<int> a = new( 1, 2, 3 );
		Rotor3<int> b = new( 4, 5, 6, 7 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMultivector3Operator() {
		Vector3<int> a = new( 1, 2, 3 );
		Multivector3<int> b = new( 4, 5, 6, 7, 8, 9, 10, 11 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMatrix3x3Operator() {
		Vector3<int> a = new( 1, 2, 3 );
		Matrix3x3<int> b = new(
			1, 0, 0,
			0, 1, 0,
			0, 0, 1
		);
		Matrix3x3<int> c = new(
			0, 0, 1,
			0, 1, 0,
			1, 0, 0
		);
		Matrix3x3<int> d = new(
			1, 3, 2,
			2, 1, 3,
			3, 2, 1
		);

		Vector3<int> expected_ab = new( 1, 2, 3 );
		Vector3<int> expected_ac = new( 3, 2, 1 );
		Vector3<int> expected_ad = new( 14, 11, 11 );

		Assert.That( a * b, Is.EqualTo( expected_ab ) );
		Assert.That( a * c, Is.EqualTo( expected_ac ) );
		Assert.That( a * d, Is.EqualTo( expected_ad ) );
	}

	[Test]
	public void ComparisonOperators() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 2, 3, 4 );
		Vector3<int> c = new( 2, 2, 3 );
		Vector3<int> d = new( 1, 2, 3 );

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
		Vector3<int> a = new( 1, 2, 3 );

		Multivector3<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 1 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 2 ) );
		Assert.That( multivector.Vector.Z, Is.EqualTo( 3 ) );
		Assert.That( multivector.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( multivector.Trivector.XYZ, Is.EqualTo( 0 ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 2, 3, 4 );

		Vector3<int> expected = new( 2, 6, 12 );

		Assert.That( a.MultiplyEntrywise( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideEntrywise() {
		Vector3<int> a = new( 4, 6, 8 );
		Vector3<int> b = new( 2, 3, 4 );

		Vector3<int> expected = new( 2, 2, 2 );

		Assert.That( a.DivideEntrywise( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void EntrywiseOperation() {
		Vector3<int> a = new( 1, 2, 8 );

		Vector3<int> expected = new( 2, 4, 16 );

		Assert.That( a.EntrywiseOperation( x => x * 2 ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Dot() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 2, 3, 4 );

		int dot = a.Dot( b );

		Assert.That( dot, Is.EqualTo( 20 ) );
	}

	[Test]
	public void MagnitudeSquared() {
		Vector3<int> a = new( 1, 2, 3 );

		int magnitudeSquared = a.MagnitudeSquared();

		Assert.That( magnitudeSquared, Is.EqualTo( 14 ) );
	}

	[Test]
	public void Wedge() {
		Vector3<int> a = new( 1, 0, 0 );
		Vector3<int> b = new( 0, 0, 1 );

		Bivector3<int> wedge = a.Wedge( b );

		Assert.That( wedge.YZ, Is.EqualTo( 0 ) );
		Assert.That( wedge.ZX, Is.EqualTo( -1 ) );
		Assert.That( wedge.XY, Is.EqualTo( 0 ) );
	}

	[Test]
	public void Min() {
		Vector3<int> a = new( 1, 2, 2 );
		Vector3<int> b = new( 2, 1, -1 );

		Vector3<int> expected = new( 1, 1, -1 );

		Assert.That( a.Min( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Max() {
		Vector3<int> a = new( 1, 2, 2 );
		Vector3<int> b = new( 2, 1, -1 );

		Vector3<int> expected = new( 2, 2, 2 );

		Assert.That( a.Max( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void SumOfParts() {
		Vector3<int> a = new( 3, 5, 7 );

		Assert.That( a.SumOfParts(), Is.EqualTo( 15 ) );
	}

	[Test]
	public void ProductOfParts() {
		Vector3<int> a = new( 3, 5, 7 );

		Assert.That( a.ProductOfParts(), Is.EqualTo( 105 ) );
	}

	[Test]
	public void SumOfUnitBasisAreas() {
		Vector3<int> a = new( 3, 5, 7 );

		Assert.That( a.SumOfUnitBasisAreas(), Is.EqualTo( 71 ) );
	}

	[Test]
	public void SumOfUnitBasisVolumes() {
		Vector3<int> a = new( 3, 5, 7 );

		Assert.That( a.SumOfUnitBasisVolumes(), Is.EqualTo( 105 ) );
	}

	[Test]
	public void ReflectNormal() {
		Vector3<int> a = new( 1, 2, 1 );
		Vector3<int> normal = new( 0, 1, 0 );

		Vector3<int> expected = new( -1, 2, -1 );

		Assert.That( a.ReflectNormal( normal ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Test_Equals() {
		Vector3<int> a = new( 1, 2, 3 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Vector3<int>( 1, 2, 3 ) ), Is.True );
		Assert.That( a.Equals( new Vector3<int>( 2, -3, -5 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 1, 2, 3 );
		Vector3<int> c = new( 2, 5, 7 );
		Vector3<int> d = new( 2, 5, 7 );

		Assert.That( a.GetHashCode(), Is.EqualTo( b.GetHashCode() ) );
		Assert.That( a.GetHashCode(), Is.Not.EqualTo( c.GetHashCode() ) );
		Assert.That( c.GetHashCode(), Is.EqualTo( d.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Vector3<int> a = new( 1, -56300, 17 );
		Vector3<double> b = new( -69.4201, 7031, 75.4 );
		Vector3<int> allNegative = new( -1, -2, -3 );

		Assert.That( a.ToString(), Is.EqualTo( "[1X - 56,300Y + 17Z]" ) );
		Assert.That( b.ToString(), Is.EqualTo( "[-69.42X + 7,031Y + 75.4Z]" ) );
		Assert.That( allNegative.ToString(), Is.EqualTo( "[-1X - 2Y - 3Z]" ) );
	}
	#endregion

	#region Casts
	[Test]
	public void CastFromScalar() {
		int value = 5;
		Vector3<int> vector = value;

		Vector3<int> expected = new( 5, 5, 5 );

		Assert.That( vector, Is.EqualTo( expected ) );
	}

	[Test]
	public void CastFromTuple() {
		(int, int, int) tuple = (5, 7, 9);
		Vector3<int> vector = tuple;

		Vector3<int> expected = new( 5, 7, 9 );

		Assert.That( vector, Is.EqualTo( expected ) );
	}
	#endregion
}
