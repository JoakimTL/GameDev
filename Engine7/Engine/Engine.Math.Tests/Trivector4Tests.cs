namespace Engine.Math.Tests;

[TestFixture]
public sealed class Trivector4Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Trivector4<int> vectorA = new( 1, 2, 3, 4 );
		Trivector4<double> vectorB = new( 1.0, 2.0, 3, 4 );

		Assert.That( vectorA.YZW, Is.EqualTo( 1 ) );
		Assert.That( vectorA.XZW, Is.EqualTo( 2 ) );
		Assert.That( vectorA.XYW, Is.EqualTo( 3 ) );
		Assert.That( vectorA.XYZ, Is.EqualTo( 4 ) );

		Assert.That( vectorB.YZW, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.XZW, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.XYW, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.XYZ, Is.EqualTo( 4.0 ) );
	}

	[Test]
	public void Identities() {
		Trivector4<int> addtitiveIdentityA = Trivector4<int>.AdditiveIdentity;
		Trivector4<int> multiplicativeIdentityA = Trivector4<int>.MultiplicativeIdentity;
		Trivector4<int> zeroA = Trivector4<int>.Zero;
		Trivector4<int> oneA = Trivector4<int>.One;
		Trivector4<double> addtitiveIdentityB = Trivector4<double>.AdditiveIdentity;
		Trivector4<double> multiplicativeIdentityB = Trivector4<double>.MultiplicativeIdentity;
		Trivector4<double> zeroB = Trivector4<double>.Zero;
		Trivector4<double> oneB = Trivector4<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.YZW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.XZW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.XYW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.XYZ, Is.EqualTo( 0 ) );

		Assert.That( oneA.YZW, Is.EqualTo( 1 ) );
		Assert.That( oneA.XZW, Is.EqualTo( 1 ) );
		Assert.That( oneA.XYW, Is.EqualTo( 1 ) );
		Assert.That( oneA.XYZ, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.YZW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.XZW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.XYW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.XYZ, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.YZW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.XZW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.XYW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.XYZ, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Trivector4<int>( 1, 2, 3, 4 ) == new Trivector4<int>( 1, 2, 3, 4 );
		bool equalYZW_isfalse = new Trivector4<int>( 1, 2, 3, 4 ) == new Trivector4<int>( 0, 2, 3, 4 );
		bool equalXZW_isfalse = new Trivector4<int>( 1, 2, 3, 4 ) == new Trivector4<int>( 1, 0, 3, 4 );
		bool equalXYW_isfalse = new Trivector4<int>( 1, 2, 3, 4 ) == new Trivector4<int>( 1, 2, 0, 4 );
		bool equalXYZ_isfalse = new Trivector4<int>( 1, 2, 3, 4 ) == new Trivector4<int>( 1, 2, 3, 0 );

		Assert.That( equal, Is.True );
		Assert.That( equalYZW_isfalse, Is.False );
		Assert.That( equalXZW_isfalse, Is.False );
		Assert.That( equalXYW_isfalse, Is.False );
		Assert.That( equalXYZ_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Trivector4<int>( 1, 2, 3, 4 ) != new Trivector4<int>( 1, 2, 3, 4 );
		bool notequalYZW = new Trivector4<int>( 1, 2, 3, 4 ) != new Trivector4<int>( 0, 2, 3, 4 );
		bool notequalXZW = new Trivector4<int>( 1, 2, 3, 4 ) != new Trivector4<int>( 1, 0, 3, 4 );
		bool notequalXYW = new Trivector4<int>( 1, 2, 3, 4 ) != new Trivector4<int>( 1, 2, 0, 4 );
		bool notequalXYZ = new Trivector4<int>( 1, 2, 3, 4 ) != new Trivector4<int>( 1, 2, 3, 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalYZW, Is.True );
		Assert.That( notequalXZW, Is.True );
		Assert.That( notequalXYW, Is.True );
		Assert.That( notequalXYZ, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 );
		bool equalYZW_isfalse = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Trivector4<double>( 0.0, 2.0, 3.0, 4.0 );
		bool equalXZW_isfalse = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Trivector4<double>( 1.0, 0.0, 3.0, 4.0 );
		bool equalXYW_isfalse = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Trivector4<double>( 1.0, 2.0, 0.0, 4.0 );
		bool equalXYZ_isfalse = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Trivector4<double>( 1.0, 2.0, 3.0, 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( equalYZW_isfalse, Is.False );
		Assert.That( equalXZW_isfalse, Is.False );
		Assert.That( equalXYW_isfalse, Is.False );
		Assert.That( equalXYZ_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 );
		bool notequalYZW = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Trivector4<double>( 0.0, 2.0, 3.0, 4.0 );
		bool notequalXZW = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Trivector4<double>( 1.0, 0.0, 3.0, 4.0 );
		bool notequalXYW = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Trivector4<double>( 1.0, 2.0, 0.0, 4.0 );
		bool notequalXYZ = new Trivector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Trivector4<double>( 1.0, 2.0, 3.0, 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalYZW, Is.True );
		Assert.That( notequalXZW, Is.True );
		Assert.That( notequalXYW, Is.True );
		Assert.That( notequalXYZ, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Trivector4<int> vectorA = new( 1, 2, 3, 4 );

		Trivector4<int> expected = new( -1, -2, -3, -4 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Trivector4<int> vectorA = new( 1, 2, 3, 4 );
		Trivector4<int> vectorB = new( 2, 3, 4, 5 );

		Trivector4<int> expected = new( 3, 5, 7, 9 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Trivector4<int> vectorA = new( 1, 2, 3, 4 );
		Trivector4<int> vectorB = new( 2, 3, 4, 5 );

		Trivector4<int> expected = new( -1, -1, -1, -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Trivector4<int> vectorA = new( 1, 2, 3, 4 );

		Trivector4<int> expected = new( 2, 4, 6, 8 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Trivector4<int> vectorA = new( 2, 4, 6, 8 );

		Trivector4<int> expected = new( 1, 2, 3, 4 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Trivector4<int> vectorA = new( 2, 4, 8, 16 );

		Trivector4<int> expected = new( 16, 8, 4, 2 );
		Assert.That( 32 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Trivector4<int> a = new( 1, 2, 3, 4 );

		Multivector4<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.Z, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.W, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.YW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.ZW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.XW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Trivector.YZW, Is.EqualTo( 1 ) );
		Assert.That( multivector.Trivector.XZW, Is.EqualTo( 2 ) );
		Assert.That( multivector.Trivector.XYW, Is.EqualTo( 3 ) );
		Assert.That( multivector.Trivector.XYZ, Is.EqualTo( 4 ) );
		Assert.That( multivector.Quadvector.XYZW, Is.EqualTo( 0 ) );

	}

	[Test]
	public void Dot() {
		Trivector4<int> a = new( 1, 2, 3, 4 );
		Trivector4<int> b = new( 5, 6, 7, 8 );

		int expected = -70;

		Assert.That( a.Dot( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void MagnitudeSquared() {
		Trivector4<int> a = new( 1, 2, 3, 4 );

		int expected = 30;

		Assert.That( a.MagnitudeSquared(), Is.EqualTo( expected ) );
	}

	[Test]
	public void Test_Equals() {
		Trivector4<int> a = new( 1, 2, 3, 4 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Trivector4<int>( 1, 2, 3, 4 ) ), Is.True );
		Assert.That( a.Equals( new Trivector4<int>( 2, 5, 6, 2 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Trivector4<int> a = new( 1, 2, 3, 4 );
		Trivector4<int> b = new( 1, 2, 3, 4 );
		Trivector4<int> c = new( 2, 5, 6, 7 );
		Trivector4<int> d = new( 2, 5, 6, 7 );

		Assert.That( a.GetHashCode(), Is.EqualTo( b.GetHashCode() ) );
		Assert.That( a.GetHashCode(), Is.Not.EqualTo( c.GetHashCode() ) );
		Assert.That( c.GetHashCode(), Is.EqualTo( d.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Trivector4<int> a = new( 1, 2, 3, 4005 );
		Trivector4<double> c = new( -69.4201, 604023.51321, 2348, 12 );
		Trivector4<int> allNegative = new( -1, -2, -3, -4 );

		Assert.That( a.ToString(), Is.EqualTo( "[1YZW + 2XZW + 3XYW + 4,005XYZ]" ) );
		Assert.That( c.ToString(), Is.EqualTo( "[-69.42YZW + 604,023.513XZW + 2,348XYW + 12XYZ]" ) );
		Assert.That( allNegative.ToString(), Is.EqualTo( "[-1YZW - 2XZW - 3XYW - 4XYZ]" ) );
	}
	#endregion
}
