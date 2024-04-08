using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Rotor2Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Rotor2<int> vectorA = new( 1, new( 2 ) );
		Rotor2<double> vectorB = new( 1.0, new( 2.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 2 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 2.0 ) );
	}

	[Test]
	public void Identities() {
		Rotor2<int> addtitiveIdentityA = Rotor2<int>.AdditiveIdentity;
		Rotor2<int> multiplicativeIdentityA = Rotor2<int>.MultiplicativeIdentity;
		Rotor2<int> zeroA = Rotor2<int>.Zero;
		Rotor2<int> oneA = Rotor2<int>.One;
		Rotor2<double> addtitiveIdentityB = Rotor2<double>.AdditiveIdentity;
		Rotor2<double> multiplicativeIdentityB = Rotor2<double>.MultiplicativeIdentity;
		Rotor2<double> zeroB = Rotor2<double>.Zero;
		Rotor2<double> oneB = Rotor2<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.Scalar, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.XY, Is.EqualTo( 0 ) );

		Assert.That( oneA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.XY, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.Scalar, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.XY, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.XY, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Rotor2<int>( 1, new( 2 ) ) == new Rotor2<int>( 1, new( 2 ) );
		bool equalScalar_isfalse = new Rotor2<int>( 1, new( 2 ) ) == new Rotor2<int>( 0, new( 2 ) );
		bool equalBivector_isfalse = new Rotor2<int>( 1, new( 2 ) ) == new Rotor2<int>( 1, new( 0 ) );

		Assert.That( equal, Is.True );
		Assert.That( equalScalar_isfalse, Is.False );
		Assert.That( equalBivector_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Rotor2<int>( 1, new( 2 ) ) != new Rotor2<int>( 1, new( 2 ) );
		bool equalScalar = new Rotor2<int>( 1, new( 2 ) ) != new Rotor2<int>( 0, new( 2 ) );
		bool equalBivector = new Rotor2<int>( 1, new( 2 ) ) != new Rotor2<int>( 1, new( 0 ) );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( equalScalar, Is.True );
		Assert.That( equalBivector, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Rotor2<double>( 1.0, new( 2.0 ) ) == new Rotor2<double>( 1.0, new( 2.0 ) );
		bool equalScalar_isfalse = new Rotor2<double>( 1.0, new( 2.0 ) ) == new Rotor2<double>( 0.0, new( 2.0 ) );
		bool equalBivector_isfalse = new Rotor2<double>( 1.0, new( 2.0 ) ) == new Rotor2<double>( 1.0, new( 0.0 ) );

		Assert.That( equal, Is.True );
		Assert.That( equalScalar_isfalse, Is.False );
		Assert.That( equalBivector_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Rotor2<double>( 1.0, new( 2.0 ) ) != new Rotor2<double>( 1.0, new( 2.0 ) );
		bool equalScalar = new Rotor2<double>( 1.0, new( 2.0 ) ) != new Rotor2<double>( 0.0, new( 2.0 ) );
		bool equalBivector = new Rotor2<double>( 1.0, new( 2.0 ) ) != new Rotor2<double>( 1.0, new( 0.0 ) );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( equalScalar, Is.True );
		Assert.That( equalBivector, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Rotor2<int> vectorA = new( 1, new( 2 ) );

		Rotor2<int> expected = new( -1, new( -2 ) );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Rotor2<int> vectorA = new( 1, new( 2 ) );
		Rotor2<int> vectorB = new( 2, new( 3 ) );

		Rotor2<int> expected = new( 3, new( 5 ) );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Rotor2<int> vectorA = new( 1, new( 2 ) );
		Rotor2<int> vectorB = new( 2, new( 3 ) );

		Rotor2<int> expected = new( -1, new( -1 ) );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Rotor2<int> vectorA = new( 1, new( 2 ) );

		Rotor2<int> expected = new( 2, new( 4 ) );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Rotor2<int> vectorA = new( 2, new( 4 ) );

		Rotor2<int> expected = new( 1, new( 2 ) );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Rotor2<int> vectorA = new( 4, new( 2 ) );

		Rotor2<int> expected = new( 2, new( 4 ) );
		Assert.That( 8 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
