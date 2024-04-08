using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

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
	#endregion
}
