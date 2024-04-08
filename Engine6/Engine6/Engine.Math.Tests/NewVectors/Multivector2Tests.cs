using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Multivector2Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Multivector2<int> vectorA = new( 1, new( 2, 3 ), new( 4 ) );
		Multivector2<double> vectorB = new( 1.0, new( 2.0, 3.0 ), new( 4.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 4 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Vector.X, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Vector.Y, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 4.0 ) );
	}
	[Test]
	public void Identities() {
		Multivector2<int> addtitiveIdentityA = Multivector2<int>.AdditiveIdentity;
		Multivector2<int> multiplicativeIdentityA = Multivector2<int>.MultiplicativeIdentity;
		Multivector2<int> zeroA = Multivector2<int>.Zero;
		Multivector2<int> oneA = Multivector2<int>.One;
		Multivector2<double> addtitiveIdentityB = Multivector2<double>.AdditiveIdentity;
		Multivector2<double> multiplicativeIdentityB = Multivector2<double>.MultiplicativeIdentity;
		Multivector2<double> zeroB = Multivector2<double>.Zero;
		Multivector2<double> oneB = Multivector2<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.Scalar, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Vector.X, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.XY, Is.EqualTo( 0 ) );

		Assert.That( oneA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( oneA.Vector.X, Is.EqualTo( 1 ) );
		Assert.That( oneA.Vector.Y, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.XY, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.Scalar, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Vector.X, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Vector.Y, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.XY, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Vector.X, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Vector.Y, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.XY, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOpertator_Int() {
		bool equal = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) == new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) );
		bool equalscalar_isfalse = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) == new Multivector2<int>( 0, new( 2, 3 ), new( 4 ) );
		bool equalvector_isfalse = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) == new Multivector2<int>( 1, new( 0, 0 ), new( 4 ) );
		bool equalbivector_isfalse = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) == new Multivector2<int>( 1, new( 2, 3 ), new( 0 ) );

		Assert.That( equal, Is.True );
		Assert.That( equalscalar_isfalse, Is.False );
		Assert.That( equalvector_isfalse, Is.False );
		Assert.That( equalbivector_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool notequal_isfalse = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) != new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) );
		bool notequalscalar = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) != new Multivector2<int>( 0, new( 2, 3 ), new( 4 ) );
		bool notequalvector = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) != new Multivector2<int>( 1, new( 0, 0 ), new( 4 ) );
		bool notequalbivector = new Multivector2<int>( 1, new( 2, 3 ), new( 4 ) ) != new Multivector2<int>( 1, new( 2, 3 ), new( 0 ) );

		Assert.That( notequal_isfalse, Is.False );
		Assert.That( notequalscalar, Is.True );
		Assert.That( notequalvector, Is.True );
		Assert.That( notequalbivector, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) == new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) );
		bool equalscalar_isfalse = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) == new Multivector2<double>( 0.0, new( 2.0, 3.0 ), new( 4.0 ) );
		bool equalvector_isfalse = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) == new Multivector2<double>( 1.0, new( 0.0, 0.0 ), new( 4.0 ) );
		bool equalbivector_isfalse = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) == new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 0.0 ) );

		Assert.That( equal, Is.True );
		Assert.That( equalscalar_isfalse, Is.False );
		Assert.That( equalvector_isfalse, Is.False );
		Assert.That( equalbivector_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool notequal_isfalse = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) != new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) );
		bool notequalscalar = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) != new Multivector2<double>( 0.0, new( 2.0, 3.0 ), new( 4.0 ) );
		bool notequalvector = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) != new Multivector2<double>( 1.0, new( 0.0, 0.0 ), new( 4.0 ) );
		bool notequalbivector = new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 4.0 ) ) != new Multivector2<double>( 1.0, new( 2.0, 3.0 ), new( 0.0 ) );

		Assert.That( notequal_isfalse, Is.False );
		Assert.That( notequalscalar, Is.True );
		Assert.That( notequalvector, Is.True );
		Assert.That( notequalbivector, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Multivector2<int> vectorA = new( 1, new( 2, 3 ), new( 4 ) );

		Multivector2<int> expected = new( -1, new( -2, -3 ), new( -4 ) );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Multivector2<int> vectorA = new( 1, new( 2, 3 ), new( 4 ) );
		Multivector2<int> vectorB = new( 5, new( 6, 7 ), new( 8 ) );

		Multivector2<int> expected = new( 6, new( 8, 10 ), new( 12 ) );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Multivector2<int> vectorA = new( 1, new( 2, 3 ), new( 4 ) );
		Multivector2<int> vectorB = new( 5, new( 6, 7 ), new( 8 ) );

		Multivector2<int> expected = new( -4, new( -4, -4 ), new( -4 ) );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Multivector2<int> vectorA = new( 3, new( 4, 5 ), new( 6 ) );

		Multivector2<int> expected = new( 6, new( 8, 10 ), new( 12 ) );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Multivector2<int> vectorA = new( 6, new( 8, 10 ), new( 12 ) );

		Multivector2<int> expected = new( 3, new( 4, 5 ), new( 6 ) );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Multivector2<int> vectorA = new( 6, new( 8, 10 ), new( 12 ) );

		Multivector2<int> expected = new( 4, new( 3, 2 ), new( 2 ) );
		Assert.That( 24 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion
}
