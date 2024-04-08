using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Trivector3Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Trivector3<int> vectorA = new( 1 );
		Trivector3<double> vectorB = new( 1.0 );

		Assert.That( vectorA.XYZ, Is.EqualTo( 1 ) );

		Assert.That( vectorB.XYZ, Is.EqualTo( 1.0 ) );
	}

	[Test]
	public void Identities() {
		Trivector3<int> addtitiveIdentityA = Trivector3<int>.AdditiveIdentity;
		Trivector3<int> multiplicativeIdentityA = Trivector3<int>.MultiplicativeIdentity;
		Trivector3<int> zeroA = Trivector3<int>.Zero;
		Trivector3<int> oneA = Trivector3<int>.One;
		Trivector3<double> addtitiveIdentityB = Trivector3<double>.AdditiveIdentity;
		Trivector3<double> multiplicativeIdentityB = Trivector3<double>.MultiplicativeIdentity;
		Trivector3<double> zeroB = Trivector3<double>.Zero;
		Trivector3<double> oneB = Trivector3<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.XYZ, Is.EqualTo( 0 ) );

		Assert.That( oneA.XYZ, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.XYZ, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.XYZ, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Trivector3<int>( 1 ) == new Trivector3<int>( 1 );
		bool notequal_isfalse = new Trivector3<int>( 1 ) == new Trivector3<int>( 0 );

		Assert.That( equal, Is.True );
		Assert.That( notequal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Trivector3<int>( 1 ) != new Trivector3<int>( 1 );
		bool notequal = new Trivector3<int>( 1 ) != new Trivector3<int>( 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Trivector3<double>( 1.0 ) == new Trivector3<double>( 1.0 );
		bool notequal_isfalse = new Trivector3<double>( 1.0 ) == new Trivector3<double>( 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( notequal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Trivector3<double>( 1.0 ) != new Trivector3<double>( 1.0 );
		bool notequal = new Trivector3<double>( 1.0 ) != new Trivector3<double>( 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Trivector3<int> vectorA = new( 1 );

		Trivector3<int> expected = new( -1 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Trivector3<int> vectorA = new( 1 );
		Trivector3<int> vectorB = new( 2 );

		Trivector3<int> expected = new( 3 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Trivector3<int> vectorA = new( 1 );
		Trivector3<int> vectorB = new( 2 );

		Trivector3<int> expected = new( -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Trivector3<int> vectorA = new( 1 );

		Trivector3<int> expected = new( 2 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Trivector3<int> vectorA = new( 2 );

		Trivector3<int> expected = new( 1 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Trivector3<int> vectorA = new( 2 );

		Trivector3<int> expected = new( 1 );
		Assert.That( 2 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
