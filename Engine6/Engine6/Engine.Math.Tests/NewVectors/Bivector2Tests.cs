using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Bivector2Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Bivector2<int> vectorA = new( 1 );
		Bivector2<double> vectorB = new( 1.0 );

		Assert.That( vectorA.XY, Is.EqualTo( 1 ) );

		Assert.That( vectorB.XY, Is.EqualTo( 1.0 ) );
	}

	[Test]
	public void Identities() {
		Bivector2<int> addtitiveIdentityA = Bivector2<int>.AdditiveIdentity;
		Bivector2<int> multiplicativeIdentityA = Bivector2<int>.MultiplicativeIdentity;
		Bivector2<int> zeroA = Bivector2<int>.Zero;
		Bivector2<int> oneA = Bivector2<int>.One;
		Bivector2<double> addtitiveIdentityB = Bivector2<double>.AdditiveIdentity;
		Bivector2<double> multiplicativeIdentityB = Bivector2<double>.MultiplicativeIdentity;
		Bivector2<double> zeroB = Bivector2<double>.Zero;
		Bivector2<double> oneB = Bivector2<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.XY, Is.EqualTo( 0 ) );

		Assert.That( oneA.XY, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.XY, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.XY, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Bivector2<int>( 1 ) == new Bivector2<int>( 1 );
		bool notequal_isfalse = new Bivector2<int>( 1 ) == new Bivector2<int>( 0 );

		Assert.That( equal, Is.True );
		Assert.That( notequal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Bivector2<int>( 1 ) != new Bivector2<int>( 1 );
		bool notequal = new Bivector2<int>( 1 ) != new Bivector2<int>( 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Bivector2<double>( 1.0 ) == new Bivector2<double>( 1.0 );
		bool notequal_isfalse = new Bivector2<double>( 1.0 ) == new Bivector2<double>( 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( notequal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Bivector2<double>( 1.0 ) != new Bivector2<double>( 1.0 );
		bool notequal = new Bivector2<double>( 1.0 ) != new Bivector2<double>( 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Bivector2<int> vectorA = new( 1 );

		Bivector2<int> expected = new( -1 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Bivector2<int> vectorA = new( 1 );
		Bivector2<int> vectorB = new( 2 );

		Bivector2<int> expected = new( 3 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Bivector2<int> vectorA = new( 1 );
		Bivector2<int> vectorB = new( 2 );

		Bivector2<int> expected = new( -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Bivector2<int> vectorA = new( 3 );

		Bivector2<int> expected = new( 6 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Bivector2<int> vectorA = new( 6 );

		Bivector2<int> expected = new( 3 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Bivector2<int> vectorA = new( 6 );

		Bivector2<int> expected = new( 4 );
		Assert.That( 24 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
