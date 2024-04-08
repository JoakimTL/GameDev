using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Vector4Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );
		Vector4<double> vectorB = new( 1.0, 2.0, 3.0, 4.0 );

		Assert.That( vectorA.X, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Y, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Z, Is.EqualTo( 3 ) );
		Assert.That( vectorA.W, Is.EqualTo( 4 ) );

		Assert.That( vectorB.X, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Y, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Z, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.W, Is.EqualTo( 4.0 ) );
	}

	[Test]
	public void Identities() {
		Vector4<int> addtitiveIdentityA = Vector4<int>.AdditiveIdentity;
		Vector4<int> multiplicativeIdentityA = Vector4<int>.MultiplicativeIdentity;
		Vector4<int> zeroA = Vector4<int>.Zero;
		Vector4<int> oneA = Vector4<int>.One;
		Vector4<double> addtitiveIdentityB = Vector4<double>.AdditiveIdentity;
		Vector4<double> multiplicativeIdentityB = Vector4<double>.MultiplicativeIdentity;
		Vector4<double> zeroB = Vector4<double>.Zero;
		Vector4<double> oneB = Vector4<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.X, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Y, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Z, Is.EqualTo( 0 ) );
		Assert.That( zeroA.W, Is.EqualTo( 0 ) );

		Assert.That( oneA.X, Is.EqualTo( 1 ) );
		Assert.That( oneA.Y, Is.EqualTo( 1 ) );
		Assert.That( oneA.Z, Is.EqualTo( 1 ) );
		Assert.That( oneA.W, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.X, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Y, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Z, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.W, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.X, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Y, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Z, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.W, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 2, 3, 4 );
		bool equalX_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 0, 2, 3, 4 );
		bool equalY_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 0, 3, 4 );
		bool equalZ_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 2, 0, 4 );
		bool equalW_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 2, 3, 0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
		Assert.That( equalZ_isfalse, Is.False );
		Assert.That( equalW_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 2, 3, 4 );
		bool notequalX = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 0, 2, 3, 4 );
		bool notequalY = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 0, 3, 4 );
		bool notequalZ = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 2, 0, 4 );
		bool notequalW = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 2, 3, 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
		Assert.That( notequalZ, Is.True );
		Assert.That( notequalW, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 2.0, 3.0, 4.0 );
		bool equalX_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 0.0, 2.0, 3.0, 4.0 );
		bool equalY_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 0.0, 3.0, 4.0 );
		bool equalZ_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 2.0, 0.0, 4.0 );
		bool equalW_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 2.0, 3.0, 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
		Assert.That( equalZ_isfalse, Is.False );
		Assert.That( equalW_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 2.0, 3.0, 4.0 );
		bool notequalX = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 0.0, 2.0, 3.0, 4.0 );
		bool notequalY = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 0.0, 3.0, 4.0 );
		bool notequalZ = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 2.0, 0.0, 4.0 );
		bool notequalW = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 2.0, 3.0, 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
		Assert.That( notequalZ, Is.True );
		Assert.That( notequalW, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );

		Vector4<int> expected = new( -1, -2, -3, -4 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );
		Vector4<int> vectorB = new( 2, 3, 4, 5 );

		Vector4<int> expected = new( 3, 5, 7, 9 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );
		Vector4<int> vectorB = new( 2, 3, 4, 5 );

		Vector4<int> expected = new( -1, -1, -1, -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );

		Vector4<int> expected = new( 2, 4, 6, 8 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Vector4<int> vectorA = new( 2, 4, 6, 8 );

		Vector4<int> expected = new( 1, 2, 3, 4 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Vector4<int> vectorA = new( 2, 4, 8, 16 );

		Vector4<int> expected = new( 16, 8, 4, 2 );
		Assert.That( 32 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
