using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Bivector3Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Bivector3<int> vectorA = new( 1, 2, 3 );
		Bivector3<double> vectorB = new( 1.0, 2.0, 3.0 );

		Assert.That( vectorA.YZ, Is.EqualTo( 1 ) );
		Assert.That( vectorA.ZX, Is.EqualTo( 2 ) );
		Assert.That( vectorA.XY, Is.EqualTo( 3 ) );

		Assert.That( vectorB.YZ, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.ZX, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.XY, Is.EqualTo( 3.0 ) );
	}

	[Test]
	public void Identities() {
		Bivector3<int> addtitiveIdentityA = Bivector3<int>.AdditiveIdentity;
		Bivector3<int> multiplicativeIdentityA = Bivector3<int>.MultiplicativeIdentity;
		Bivector3<int> zeroA = Bivector3<int>.Zero;
		Bivector3<int> oneA = Bivector3<int>.One;
		Bivector3<double> addtitiveIdentityB = Bivector3<double>.AdditiveIdentity;
		Bivector3<double> multiplicativeIdentityB = Bivector3<double>.MultiplicativeIdentity;
		Bivector3<double> zeroB = Bivector3<double>.Zero;
		Bivector3<double> oneB = Bivector3<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.YZ, Is.EqualTo( 0 ) );
		Assert.That( zeroA.ZX, Is.EqualTo( 0 ) );
		Assert.That( zeroA.XY, Is.EqualTo( 0 ) );

		Assert.That( oneA.YZ, Is.EqualTo( 1 ) );
		Assert.That( oneA.ZX, Is.EqualTo( 1 ) );
		Assert.That( oneA.XY, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.YZ, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.ZX, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.XY, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.YZ, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.ZX, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.XY, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOpertator_Int() {
		bool equal = new Bivector3<int>( 1, 2, 3 ) == new Bivector3<int>( 1, 2, 3 );
		bool equalyz_isfalse = new Bivector3<int>( 1, 2, 3 ) == new Bivector3<int>( 0, 2, 3 );
		bool equalzx_isfalse = new Bivector3<int>( 1, 2, 3 ) == new Bivector3<int>( 1, 0, 3 );
		bool equalxy_isfalse = new Bivector3<int>( 1, 2, 3 ) == new Bivector3<int>( 1, 2, 0 );

		Assert.That( equal, Is.True );
		Assert.That( equalyz_isfalse, Is.False );
		Assert.That( equalzx_isfalse, Is.False );
		Assert.That( equalxy_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Bivector3<int>( 1, 2, 3 ) != new Bivector3<int>( 1, 2, 3 );
		bool notequalyz = new Bivector3<int>( 1, 2, 3 ) != new Bivector3<int>( 0, 2, 3 );
		bool notequalzx = new Bivector3<int>( 1, 2, 3 ) != new Bivector3<int>( 1, 0, 3 );
		bool notequalxy = new Bivector3<int>( 1, 2, 3 ) != new Bivector3<int>( 1, 2, 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalyz, Is.True );
		Assert.That( notequalzx, Is.True );
		Assert.That( notequalxy, Is.True );
	}

	[Test]
	public void EqualOpertator_Double() {
		bool equal = new Bivector3<double>( 1.0, 2.0, 3.0 ) == new Bivector3<double>( 1.0, 2.0, 3.0 );
		bool equalyz_isfalse = new Bivector3<double>( 1.0, 2.0, 3.0 ) == new Bivector3<double>( 0.0, 2.0, 3.0 );
		bool equalzx_isfalse = new Bivector3<double>( 1.0, 2.0, 3.0 ) == new Bivector3<double>( 1.0, 0.0, 3.0 );
		bool equalxy_isfalse = new Bivector3<double>( 1.0, 2.0, 3.0 ) == new Bivector3<double>( 1.0, 2.0, 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( equalyz_isfalse, Is.False );
		Assert.That( equalzx_isfalse, Is.False );
		Assert.That( equalxy_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Bivector3<double>( 1.0, 2.0, 3.0 ) != new Bivector3<double>( 1.0, 2.0, 3.0 );
		bool notequalyz = new Bivector3<double>( 1.0, 2.0, 3.0 ) != new Bivector3<double>( 0.0, 2.0, 3.0 );
		bool notequalzx = new Bivector3<double>( 1.0, 2.0, 3.0 ) != new Bivector3<double>( 1.0, 0.0, 3.0 );
		bool notequalxy = new Bivector3<double>( 1.0, 2.0, 3.0 ) != new Bivector3<double>( 1.0, 2.0, 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalyz, Is.True );
		Assert.That( notequalzx, Is.True );
		Assert.That( notequalxy, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Bivector3<int> vectorA = new( 1, 2, 3 );

		Bivector3<int> expected = new( -1, -2, -3 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Bivector3<int> vectorA = new( 1, 2, 3 );
		Bivector3<int> vectorB = new( 4, 5, 6 );

		Bivector3<int> expected = new( 5, 7, 9 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Bivector3<int> vectorA = new( 1, 2, 3 );
		Bivector3<int> vectorB = new( 4, 5, 6 );

		Bivector3<int> expected = new( -3, -3, -3 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Bivector3<int> vectorA = new( 3, 4, 5 );

		Bivector3<int> expected = new( 6, 8, 10 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Bivector3<int> vectorA = new( 6, 8, 10 );

		Bivector3<int> expected = new( 3, 4, 5 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Bivector3<int> vectorA = new( 6, 8, 10 );

		Bivector3<int> expected = new( 4, 3, 2 );
		Assert.That( 24 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
