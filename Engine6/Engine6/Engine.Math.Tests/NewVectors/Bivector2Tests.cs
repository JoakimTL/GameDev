using Engine.Math.NewVectors;
using Engine.Math.NewVectors.Calculations;

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

	[Test]
	public void MultiplyVector2Operator() {
		Bivector2<int> a = new( 1 );
		Vector2<int> b = new( 2, 3 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyBivector2Operator() {
		Bivector2<int> a = new( 1 );
		Bivector2<int> b = new( 2 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyRotor2Operator() {
		Bivector2<int> a = new( 1 );
		Rotor2<int> b = new( 2, 3 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}

	[Test]
	public void MultiplyMultivector2Operator() {
		Bivector2<int> a = new( 1 );
		Multivector2<int> b = new( 2, 3, 4, 5 );

		Assert.That( a * b, Is.EqualTo( GeometricAlgebraMath2.Multiply( a, b ) ) );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Bivector2<int> bivector = new( 1 );

		Multivector2<int> multivector = bivector.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Dot() {
		Bivector2<int> vectorA = new( 1 );
		Bivector2<int> vectorB = new( 2 );

		int dot = vectorA.Dot( vectorB );

		Assert.That( dot, Is.EqualTo( -2 ) );
	}

	[Test]
	public void Test_Equals() {
		Bivector2<int> bivector_1 = new( 1 );

		Assert.That( bivector_1.Equals( bivector_1 ), Is.True );
		Assert.That( bivector_1.Equals( new Bivector2<int>( 1 ) ), Is.True );
		Assert.That( bivector_1.Equals( new Bivector2<int>( 2 ) ), Is.False );
		Assert.That( bivector_1.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Bivector2<int> bivector_1 = new( 1 );
		Bivector2<int> bivector_2 = new( 1 );
		Bivector2<int> bivector_3 = new( 2 );
		Bivector2<int> bivector_4 = new( 2 );

		Assert.That( bivector_1.GetHashCode(), Is.EqualTo( bivector_2.GetHashCode() ) );
		Assert.That( bivector_1.GetHashCode(), Is.Not.EqualTo( bivector_3.GetHashCode() ) );
		Assert.That( bivector_3.GetHashCode(), Is.EqualTo( bivector_4.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Bivector2<int> bivector_1 = new( 1 );
		Bivector2<int> bivector_2 = new( -56000 );
		Bivector2<double> bivector_3 = new( -69.4201 );

		Assert.That( bivector_1.ToString(), Is.EqualTo( "[1XY]" ) );
		Assert.That( bivector_2.ToString(), Is.EqualTo( "[-56,000XY]" ) );
		Assert.That( bivector_3.ToString(), Is.EqualTo( "[-69.42XY]" ) );
	}
	#endregion
}
