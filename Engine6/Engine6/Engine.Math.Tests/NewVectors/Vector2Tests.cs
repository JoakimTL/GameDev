using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

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
	#endregion
}
