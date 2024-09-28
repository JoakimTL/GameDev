namespace Engine.Math.Tests.Calculations;

public sealed class GeometricAlgebraMath2Tests {
	[Test]
	public static void Multiply_Vector2_Vector2() {
		Vector2<int> vectorA = new( 1, 2 );
		Vector2<int> vectorB = new( 3, 4 );

		Rotor2<int> result = GeometricAlgebraMath2.Multiply( vectorA, vectorB );
		Assert.That( result.Scalar, Is.EqualTo( 11 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( -2 ) );
	}

	[Test]
	public static void Multiply_Vector2_Bivector2() {
		Vector2<int> vector = new( 1, 2 );
		Bivector2<int> bivector = new( 3 );

		Vector2<int> result = GeometricAlgebraMath2.Multiply( vector, bivector );
		Assert.That( result.X, Is.EqualTo( -6 ) );
		Assert.That( result.Y, Is.EqualTo( 3 ) );
	}

	[Test]
	public static void Multiply_Vector2_Rotor2() {
		Vector2<int> vector = new( 1, 2 );
		Rotor2<int> rotor = new( 3, 4 );

		Vector2<int> result = GeometricAlgebraMath2.Multiply( vector, rotor );
		Assert.That( result.X, Is.EqualTo( -5 ) );
		Assert.That( result.Y, Is.EqualTo( 10 ) );
	}

	[Test]
	public static void Multiply_Vector2_Multivector2() {
		Vector2<int> vector = new( 1, 2 );
		Multivector2<int> multivector = new( 3, 4, 5, 6 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( vector, multivector );
		Assert.That( result.Scalar, Is.EqualTo( 14 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -9 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 12 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( -3 ) );
	}

	[Test]
	public static void Multiply_Bivector2_Vector2() {
		Bivector2<int> bivector = new( 1 );
		Vector2<int> vector = new( 2, 3 );

		Vector2<int> result = GeometricAlgebraMath2.Multiply( bivector, vector );
		Assert.That( result.X, Is.EqualTo( 3 ) );
		Assert.That( result.Y, Is.EqualTo( -2 ) );
	}

	[Test]
	public static void Multiply_Bivector2_Bivector2() {
		Bivector2<int> bivectorA = new( 1 );
		Bivector2<int> bivectorB = new( 2 );

		int result = GeometricAlgebraMath2.Multiply( bivectorA, bivectorB );
		Assert.That( result, Is.EqualTo( -2 ) );
	}

	[Test]
	public static void Multiply_Bivector2_Rotor2() {
		Bivector2<int> bivector = new( 1 );
		Rotor2<int> rotor = new( 2, 3 );

		Rotor2<int> result = GeometricAlgebraMath2.Multiply( bivector, rotor );
		Assert.That( result.Scalar, Is.EqualTo( -3 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 2 ) );
	}

	[Test]
	public static void Multiply_Bivector2_Multivector2() {
		Bivector2<int> bivector = new( 1 );
		Multivector2<int> multivector = new( 2, 3, 4, 5 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( bivector, multivector );
		Assert.That( result.Scalar, Is.EqualTo( -5 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 4 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -3 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 2 ) );
	}

	[Test]
	public static void Multiply_Rotor2_Vector2() {
		Rotor2<int> rotor = new( 1, 2 );
		Vector2<int> vector = new( 3, 4 );

		Vector2<int> result = GeometricAlgebraMath2.Multiply( rotor, vector );
		Assert.That( result.X, Is.EqualTo( 11 ) );
		Assert.That( result.Y, Is.EqualTo( -2 ) );
	}

	[Test]
	public static void Multiply_Rotor2_Bivector2() {
		Rotor2<int> rotor = new( 1, 2 );
		Bivector2<int> bivector = new( 3 );

		Rotor2<int> result = GeometricAlgebraMath2.Multiply( rotor, bivector );
		Assert.That( result.Scalar, Is.EqualTo( -6 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 3 ) );
	}

	[Test]
	public static void Multiply_Rotor2_Rotor2() {
		Rotor2<int> rotorA = new( 1, 2 );
		Rotor2<int> rotorB = new( 3, 4 );

		Rotor2<int> result = GeometricAlgebraMath2.Multiply( rotorA, rotorB );
		Assert.That( result.Scalar, Is.EqualTo( -5 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 10 ) );
	}

	[Test]
	public static void Multiply_Rotor2_Multivector2() {
		Rotor2<int> rotor = new( 1, 2 );
		Multivector2<int> multivector = new( 3, 4, 5, 6 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( rotor, multivector );
		Assert.That( result.Scalar, Is.EqualTo( -9 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 14 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -3 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 12 ) );
	}

	[Test]
	public static void Multiply_Multivector2_Vector2() {
		Multivector2<int> multivector = new( 1, 2, 3, 4 );
		Vector2<int> vector = new( 5, 6 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( multivector, vector );
		Assert.That( result.Scalar, Is.EqualTo( 28 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 29 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -14 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( -3 ) );
	}

	[Test]
	public static void Multiply_Multivector2_Bivector2() {
		Multivector2<int> multivector = new( 1, 2, 3, 4 );
		Bivector2<int> bivector = new( 5 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( multivector, bivector );
		Assert.That( result.Scalar, Is.EqualTo( -20 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -15 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 10 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 5 ) );
	}

	[Test]
	public static void Multiply_Multivector2_Rotor2() {
		Multivector2<int> multivector = new( 1, 2, 3, 4 );
		Rotor2<int> rotor = new( 5, 6 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( multivector, rotor );
		Assert.That( result.Scalar, Is.EqualTo( -19 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -8 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 27 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 26 ) );
	}

	[Test]
	public static void Multiply_Multivector2_Multivector2() {
		Multivector2<int> multivectorA = new( 1, 2, 3, 4 );
		Multivector2<int> multivectorB = new( 5, 6, 7, 8 );

		Multivector2<int> result = GeometricAlgebraMath2.Multiply( multivectorA, multivectorB );
		Assert.That( result.Scalar, Is.EqualTo( 6 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 20 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 14 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 24 ) );
	}
}