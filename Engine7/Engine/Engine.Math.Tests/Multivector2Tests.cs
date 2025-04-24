namespace Engine.Math.Tests;

[TestFixture]
public sealed class Multivector2Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Multivector2<int> vectorA = new( 1, new( 2, 3 ), new( 4 ) );
		Multivector2<double> vectorB = new( 1.0, new( 2.0, 3.0 ), new( 4.0 ) );
		Multivector2<int> vectorC = new( 1, 2, 3, 4 );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 4 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Vector.X, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Vector.Y, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 4.0 ) );

		Assert.That( vectorC.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorC.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorC.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorC.Bivector.XY, Is.EqualTo( 4 ) );
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

	[Test]
	public void MultiplyVector2Operator() {
		Multivector2<int> a = new( 1, 2, 3, 4 );
		Vector2<int> b = new( 5, 6 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyBivector2Operator() {
		Multivector2<int> a = new( 1, 2, 3, 4 );
		Bivector2<int> b = new( 5 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyRotor2Operator() {
		Multivector2<int> a = new( 1, 2, 3, 4 );
		Rotor2<int> b = new( 5, 6 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMultivector2Operator() {
		Multivector2<int> a = new( 1, 2, 3, 4 );
		Multivector2<int> b = new( 5, 6, 7, 8 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Multivector2<int> a = new( 1, 2, 3, 4 );

		Multivector2<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 1 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 4 ) );
	}

	[Test]
	public void Dot() {
		Multivector2<int> vectorA = new( 1, 2, 3, 4 );
		Multivector2<int> vectorB = new( 5, 6, 7, 8 );

		int dot = vectorA.Dot( vectorB );

		Assert.That( dot, Is.EqualTo( 6 ) );
	}

	[Test]
	public void MagnitudeSquared() {
		Multivector2<int> vectorA = new( 1, 2, 3, 4 );

		int magnitudeSquared = vectorA.MagnitudeSquared();

		Assert.That( magnitudeSquared, Is.EqualTo( 30 ) );
	}

	[Test]
	public void Test_Equals() {
		Multivector2<int> a = new( 1, 2, 3, 4 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Multivector2<int>( 1, 2, 3, 4 ) ), Is.True );
		Assert.That( a.Equals( new Multivector2<int>( 2, 5, 6, 12 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Multivector2<int> multivector_1 = new( 1, 2, 3, 4 );
		Multivector2<int> multivector_2 = new( 1, 2, 3, 4 );
		Multivector2<int> multivector_3 = new( 2, 5, 6, 10 );
		Multivector2<int> multivector_4 = new( 2, 5, 6, 10 );

		Assert.That( multivector_1.GetHashCode(), Is.EqualTo( multivector_2.GetHashCode() ) );
		Assert.That( multivector_1.GetHashCode(), Is.Not.EqualTo( multivector_3.GetHashCode() ) );
		Assert.That( multivector_3.GetHashCode(), Is.EqualTo( multivector_4.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Multivector2<int> a = new( 1, 2, 3, 4 );
		Multivector2<int> b = new( -56, 60, 1230, -5 );
		Multivector2<double> c = new( -69.4201, 70.5, 50.9999, 23.232 );

		Assert.That( a.ToString(), Is.EqualTo( $"<1 + {a.Vector} + {a.Bivector}>" ) );
		Assert.That( b.ToString(), Is.EqualTo( $"<-56 + {b.Vector} + {b.Bivector}>" ) );
		Assert.That( c.ToString(), Is.EqualTo( $"<-69.42 + {c.Vector} + {c.Bivector}>" ) );
	}
	#endregion

	#region Casts
	[Test]
	public void CastToScalar() {
		Multivector2<int> a = new( 1, 2, 3, 4 );

		int result = (int) a;

		Assert.That( result, Is.EqualTo( 1 ) );
	}

	[Test]
	public void CastToVector() {
		Multivector2<int> a = new( 1, 2, 3, 4 );

		Vector2<int> result = (Vector2<int>) a;

		Assert.That( result.X, Is.EqualTo( 2 ) );
		Assert.That( result.Y, Is.EqualTo( 3 ) );
	}

	[Test]
	public void CastToBivector() {
		Multivector2<int> a = new( 1, 2, 3, 4 );

		Bivector2<int> result = (Bivector2<int>) a;

		Assert.That( result.XY, Is.EqualTo( 4 ) );
	}

	[Test]
	public void CastToRotor() {
		Multivector2<int> a = new( 1, 2, 3, 4 );

		Rotor2<int> result = (Rotor2<int>) a;

		Assert.That( result.Scalar, Is.EqualTo( 1 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 4 ) );
	}
	#endregion
}
