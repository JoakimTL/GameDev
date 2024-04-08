using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Quadvector4Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Quadvector4<int> vectorA = new( 1 );
		Quadvector4<double> vectorB = new( 1.0 );

		Assert.That( vectorA.XYZW, Is.EqualTo( 1 ) );

		Assert.That( vectorB.XYZW, Is.EqualTo( 1.0 ) );
	}

	[Test]
	public void Identities() {
		Quadvector4<int> addtitiveIdentityA = Quadvector4<int>.AdditiveIdentity;
		Quadvector4<int> multiplicativeIdentityA = Quadvector4<int>.MultiplicativeIdentity;
		Quadvector4<int> zeroA = Quadvector4<int>.Zero;
		Quadvector4<int> oneA = Quadvector4<int>.One;
		Quadvector4<double> addtitiveIdentityB = Quadvector4<double>.AdditiveIdentity;
		Quadvector4<double> multiplicativeIdentityB = Quadvector4<double>.MultiplicativeIdentity;
		Quadvector4<double> zeroB = Quadvector4<double>.Zero;
		Quadvector4<double> oneB = Quadvector4<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.XYZW, Is.EqualTo( 0 ) );

		Assert.That( oneA.XYZW, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.XYZW, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.XYZW, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Quadvector4<int>( 1 ) == new Quadvector4<int>( 1 );
		bool notequal_isfalse = new Quadvector4<int>( 1 ) == new Quadvector4<int>( 0 );

		Assert.That( equal, Is.True );
		Assert.That( notequal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Quadvector4<int>( 1 ) != new Quadvector4<int>( 1 );
		bool notequal = new Quadvector4<int>( 1 ) != new Quadvector4<int>( 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Quadvector4<double>( 1.0 ) == new Quadvector4<double>( 1.0 );
		bool notequal_isfalse = new Quadvector4<double>( 1.0 ) == new Quadvector4<double>( 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( notequal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Quadvector4<double>( 1.0 ) != new Quadvector4<double>( 1.0 );
		bool notequal = new Quadvector4<double>( 1.0 ) != new Quadvector4<double>( 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Quadvector4<int> vectorA = new( 1 );

		Quadvector4<int> expected = new( -1 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Quadvector4<int> vectorA = new( 1 );
		Quadvector4<int> vectorB = new( 2 );

		Quadvector4<int> expected = new( 3 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Quadvector4<int> vectorA = new( 1 );
		Quadvector4<int> vectorB = new( 2 );

		Quadvector4<int> expected = new( -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Quadvector4<int> vectorA = new( 2 );

		Quadvector4<int> expected = new( 4 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Quadvector4<int> vectorA = new( 2 );

		Quadvector4<int> expected = new( 1 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Quadvector4<int> vectorA = new( 2 );

		Quadvector4<int> expected = new( 2 );
		Assert.That( 4 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
