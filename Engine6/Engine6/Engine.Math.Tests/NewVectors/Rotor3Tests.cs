using Engine.Math.NewVectors;
using Engine.Math.NewVectors.Calculations;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Rotor3Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Rotor3<int> vectorA = new( 1, new( 2, 3, 4 ) );
		Rotor3<double> vectorB = new( 1.0, new( 2.0, 3.0, 4.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Bivector.YZ, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Bivector.ZX, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 4 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Bivector.YZ, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Bivector.ZX, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 4.0 ) );
	}

	[Test]
	public void Identities() {
		Rotor3<int> addtitiveIdentityA = Rotor3<int>.AdditiveIdentity;
		Rotor3<int> multiplicativeIdentityA = Rotor3<int>.MultiplicativeIdentity;
		Rotor3<int> zeroA = Rotor3<int>.Zero;
		Rotor3<int> oneA = Rotor3<int>.One;
		Rotor3<double> addtitiveIdentityB = Rotor3<double>.AdditiveIdentity;
		Rotor3<double> multiplicativeIdentityB = Rotor3<double>.MultiplicativeIdentity;
		Rotor3<double> zeroB = Rotor3<double>.Zero;
		Rotor3<double> oneB = Rotor3<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.Scalar, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.XY, Is.EqualTo( 0 ) );

		Assert.That( oneA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.YZ, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.ZX, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.XY, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.Scalar, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.YZ, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.ZX, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.XY, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.YZ, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.ZX, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.XY, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Rotor3<int>( 1, new( 2, 3, 4 ) ) == new Rotor3<int>( 1, new( 2, 3, 4 ) );
		bool scalar_equal_isfalse = new Rotor3<int>( 1, new( 2, 3, 4 ) ) == new Rotor3<int>( 0, new( 2, 3, 4 ) );
		bool bivector_equal_isfalse = new Rotor3<int>( 1, new( 2, 3, 4 ) ) == new Rotor3<int>( 1, new( 0, 0, 0 ) );

		Assert.That( equal, Is.True );
		Assert.That( scalar_equal_isfalse, Is.False );
		Assert.That( bivector_equal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Rotor3<int>( 1, new( 2, 3, 4 ) ) != new Rotor3<int>( 1, new( 2, 3, 4 ) );
		bool scalar_equal = new Rotor3<int>( 1, new( 2, 3, 4 ) ) != new Rotor3<int>( 0, new( 2, 3, 4 ) );
		bool bivector_equal = new Rotor3<int>( 1, new( 2, 3, 4 ) ) != new Rotor3<int>( 1, new( 0, 0, 0 ) );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( scalar_equal, Is.True );
		Assert.That( bivector_equal, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) ) == new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) );
		bool scalar_equal_isfalse = new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) ) == new Rotor3<double>( 0.0, new( 2.0, 3.0, 4.0 ) );
		bool bivector_equal_isfalse = new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) ) == new Rotor3<double>( 1.0, new( 0.0, 0.0, 0.0 ) );

		Assert.That( equal, Is.True );
		Assert.That( scalar_equal_isfalse, Is.False );
		Assert.That( bivector_equal_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) ) != new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) );
		bool scalar_equal = new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) ) != new Rotor3<double>( 0.0, new( 2.0, 3.0, 4.0 ) );
		bool bivector_equal = new Rotor3<double>( 1.0, new( 2.0, 3.0, 4.0 ) ) != new Rotor3<double>( 1.0, new( 0.0, 0.0, 0.0 ) );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( scalar_equal, Is.True );
		Assert.That( bivector_equal, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Rotor3<int> vectorA = new( 1, new( 2, 3, 4 ) );

		Rotor3<int> expected = new( -1, new( -2, -3, -4 ) );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Rotor3<int> vectorA = new( 1, new( 2, 3, 4 ) );
		Rotor3<int> vectorB = new( 2, new( 3, 4, 5 ) );

		Rotor3<int> expected = new( 3, new( 5, 7, 9 ) );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Rotor3<int> vectorA = new( 2, new( 3, 4, 5 ) );
		Rotor3<int> vectorB = new( 1, new( 2, 3, 4 ) );

		Rotor3<int> expected = new( 1, new( 1, 1, 1 ) );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Rotor3<int> vectorA = new( 1, new( 2, 3, 4 ) );

		Rotor3<int> expected = new( 2, new( 4, 6, 8 ) );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Rotor3<int> vectorA = new( 2, new( 4, 6, 8 ) );

		Rotor3<int> expected = new( 1, new( 2, 3, 4 ) );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Rotor3<int> vectorA = new( 2, new( 4, 8, 16 ) );

		Rotor3<int> expected = new( 16, new( 8, 4, 2 ) );
		Assert.That( 32 / vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void MultiplyVector3Operator() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Vector3<int> b = new( 2, 3, 4 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyBivector3Operator() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Bivector3<int> b = new( 4, 5, 6 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyTrivector3Operator() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Trivector3<int> b = new( 4 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyRotor3Operator() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Rotor3<int> b = new( 4, 5, 6, 7 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMultivector3Operator() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Multivector3<int> b = new( 4, 5, 6, 7, 8, 9, 10, 11 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath3.Multiply( a, b ) ) );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Rotor3<int> a = new( 1, 2, 3, 4 );

		Multivector3<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 1 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.Z, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.YZ, Is.EqualTo( 2 ) );
		Assert.That( multivector.Bivector.ZX, Is.EqualTo( 3 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 4 ) );
		Assert.That( multivector.Trivector.XYZ, Is.EqualTo( 0 ) );
	}

	[Test]
	public void Dot() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Rotor3<int> b = new( 2, 3, 4, 5 );

		int dot = a.Dot( b );

		Assert.That( dot, Is.EqualTo( -36 ) );
	}

	[Test]
	public void Conjugate() {
		Rotor3<int> vectorA = new( 1, new( 2, 3, 4 ) );

		Rotor3<int> expected = new( 1, new( -2, -3, -4 ) );
		Assert.That( vectorA.Conjugate(), Is.EqualTo( expected ) );
	}

	[Test]
	public void Rotate() {
		Rotor3<double> rotor = Rotor3.FromAxisAngle( Vector3<double>.UnitZ, System.Math.PI / 2 );
		Vector3<double> vector = new( 1, 0, 0 );

		Vector3<double> result = rotor.Rotate( vector );
		Vector3<double> expected = new( 0, 1, 0 );

		Assert.That( result.X, Is.EqualTo( expected.X ).Within( 0.001 ) );
		Assert.That( result.Y, Is.EqualTo( expected.Y ).Within( 0.001 ) );
		Assert.That( result.Z, Is.EqualTo( expected.Z ).Within( 0.001 ) );
	}

	[Test]
	public void Test_Equals() {
		Rotor3<int> a = new( 1, 2, 3, 4 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Rotor3<int>( 1, 2, 3, 4 ) ), Is.True );
		Assert.That( a.Equals( new Rotor3<int>( 2, -3, -5, 7 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Rotor3<int> a = new( 1, 2, 3, 4 );
		Rotor3<int> b = new( 1, 2, 3, 4 );
		Rotor3<int> c = new( 2, 5, 7, 9 );
		Rotor3<int> d = new( 2, 5, 7, 9 );

		Assert.That( a.GetHashCode(), Is.EqualTo( b.GetHashCode() ) );
		Assert.That( a.GetHashCode(), Is.Not.EqualTo( c.GetHashCode() ) );
		Assert.That( c.GetHashCode(), Is.EqualTo( d.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Rotor3<int> a = new( 1, -56300, 17, 77 );
		Rotor3<double> b = new( -69.4201, 7031, 75.4, 4 );

		Assert.That( a.ToString(), Is.EqualTo( $"<1 + {a.Bivector}>" ) );
		Assert.That( b.ToString(), Is.EqualTo( $"<-69.42 + {b.Bivector}>" ) );
	}
	#endregion

	#region Casts
	[Test]
	public void CastToScalar() {
		Rotor3<int> a = new( 1, 2, 3, 4 );

		Assert.That( (int)a, Is.EqualTo( 1 ) );
	}

	[Test]
	public void CastToBivector2() {
		Rotor3<int> a = new( 1, 2, 3, 4 );

		Bivector3<int> b = (Bivector3<int>)a;

		Assert.That( b.YZ, Is.EqualTo( 2 ) );
		Assert.That( b.ZX, Is.EqualTo( 3 ) );
		Assert.That( b.XY, Is.EqualTo( 4 ) );
	}
	#endregion
}
