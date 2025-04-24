namespace Engine.Math.Tests;
public sealed class GeometricAlgebraMath3Tests {
	[Test]
	public static void Multiply_Vector3_Vector3() {
		Vector3<int> vectorA = new( 1, 2, 3 );
		Vector3<int> vectorB = new( 4, 5, 6 );

		Rotor3<int> result = GeometricAlgebraMath3.Multiply( vectorA, vectorB );
		Assert.That( result.Scalar, Is.EqualTo( 32 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( -3 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 6 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( -3 ) );
	}

	[Test]
	public static void Multiply_Vector3_Bivector3() {
		Vector3<int> vector = new( 1, 2, 3 );
		Bivector3<int> bivector = new( 4, 5, 6 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( vector, bivector );
		Assert.That( result.Scalar, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 3 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -6 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( 3 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 32 ) );
	}

	[Test]
	public static void Multiply_Vector3_Trivector3() {
		Vector3<int> vector = new( 1, 2, 3 );
		Trivector3<int> trivector = new( 4 );

		Bivector3<int> result = GeometricAlgebraMath3.Multiply( vector, trivector );
		Assert.That( result.YZ, Is.EqualTo( 4 ) );
		Assert.That( result.ZX, Is.EqualTo( 8 ) );
		Assert.That( result.XY, Is.EqualTo( 12 ) );
	}

	[Test]
	public static void Multiply_Vector3_Rotor3() {
		Vector3<int> vector = new( 1, 2, 3 );
		Rotor3<int> rotor = new( 4, 5, 6, 7 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( vector, rotor );
		Assert.That( result.Scalar, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 8 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( 16 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 38 ) );
	}

	[Test]
	public static void Multiply_Vector3_Multivector3() {
		Vector3<int> vector = new( 1, 2, 3 );
		Multivector3<int> multivector = new( 4, 5, 6, 7, 8, 9, 10, 11 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( vector, multivector );
		Assert.That( result.Scalar, Is.EqualTo( 38 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 11 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -6 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( 19 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 7 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 30 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 29 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 56 ) );
	}

	[Test]
	public static void Multiply_Bivector3_Vector3() {
		Bivector3<int> bivector = new( 1, 2, 3 );
		Vector3<int> vector = new( 4, 5, 6 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( bivector, vector );
		Assert.That( result.Scalar, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 3 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -6 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( 3 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 32 ) );
	}

	[Test]
	public static void Multiply_Bivector3_Bivector3() {
		Bivector3<int> bivectorA = new( 1, 2, 3 );
		Bivector3<int> bivectorB = new( 4, 5, 6 );

		Rotor3<int> result = GeometricAlgebraMath3.Multiply( bivectorA, bivectorB );
		Assert.That( result.Scalar, Is.EqualTo( -32 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 3 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( -6 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 3 ) );
	}

	[Test]
	public static void Multiply_Bivector3_Trivector3() {
		Bivector3<int> bivector = new( 1, 2, 3 );
		Trivector3<int> trivector = new( 4 );

		Vector3<int> result = GeometricAlgebraMath3.Multiply( bivector, trivector );
		Assert.That( result.X, Is.EqualTo( -4 ) );
		Assert.That( result.Y, Is.EqualTo( -8 ) );
		Assert.That( result.Z, Is.EqualTo( -12 ) );
	}

	[Test]
	public static void Multiply_Bivector3_Rotor3() {
		Bivector3<int> bivector = new( 1, 2, 3 );
		Rotor3<int> rotor = new( 4, 5, 6, 7 );

		Rotor3<int> result = GeometricAlgebraMath3.Multiply( bivector, rotor );
		Assert.That( result.Scalar, Is.EqualTo( -38 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 8 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 16 ) );
	}

	[Test]
	public static void Multiply_Bivector3_Multivector3() {
		Bivector3<int> bivector = new( 1, 2, 3 );
		Multivector3<int> multivector = new( 4, 5, 6, 7, 8, 9, 10, 11 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( bivector, multivector );
		Assert.That( result.Scalar, Is.EqualTo( -56 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -7 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -30 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -29 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 11 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( -6 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 19 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 38 ) );
	}

	[Test]
	public static void Multiply_Trivector3_Vector3() {
		Trivector3<int> trivector = new( 1 );
		Vector3<int> vector = new( 2, 3, 4 );

		Bivector3<int> result = GeometricAlgebraMath3.Multiply( trivector, vector );
		Assert.That( result.YZ, Is.EqualTo( 2 ) );
		Assert.That( result.ZX, Is.EqualTo( 3 ) );
		Assert.That( result.XY, Is.EqualTo( 4 ) );
	}

	[Test]
	public static void Multiply_Trivector3_Bivector3() {
		Trivector3<int> trivector = new( 1 );
		Bivector3<int> bivector = new( 2, 3, 4 );

		Vector3<int> result = GeometricAlgebraMath3.Multiply( trivector, bivector );
		Assert.That( result.X, Is.EqualTo( -2 ) );
		Assert.That( result.Y, Is.EqualTo( -3 ) );
		Assert.That( result.Z, Is.EqualTo( -4 ) );
	}

	[Test]
	public static void Multiply_Trivector3_Trivector3() {
		Trivector3<int> trivectorA = new( 1 );
		Trivector3<int> trivectorB = new( 2 );

		int result = GeometricAlgebraMath3.Multiply( trivectorA, trivectorB );
		Assert.That( result, Is.EqualTo( -2 ) );
	}

	[Test]
	public static void Multiply_Trivector3_Rotor3() {
		Trivector3<int> trivector = new( 1 );
		Rotor3<int> rotor = new( 2, 3, 4, 5 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( trivector, rotor );
		Assert.That( result.Scalar, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -3 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -4 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -5 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 2 ) );
	}

	[Test]
	public static void Multiply_Trivector3_Multivector3() {
		Trivector3<int> trivector = new( 1 );
		Multivector3<int> multivector = new( 2, 3, 4, 5, 6, 7, 8, 9 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( trivector, multivector );
		Assert.That( result.Scalar, Is.EqualTo( -9 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -6 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -7 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -8 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 3 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 4 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 5 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 2 ) );
	}

	[Test]
	public static void Multiply_Rotor3_Vector3() {
		Rotor3<int> rotor = new( 1, 2, 3, 4 );
		Vector3<int> vector = new( 5, 6, 7 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( rotor, vector );
		Assert.That( result.Scalar, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 8 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( 10 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 56 ) );
	}

	[Test]
	public static void Multiply_Rotor3_Bivector3() {
		Rotor3<int> rotor = new( 1, 2, 3, 4 );
		Bivector3<int> bivector = new( 5, 6, 7 );

		Rotor3<int> result = GeometricAlgebraMath3.Multiply( rotor, bivector );
		Assert.That( result.Scalar, Is.EqualTo( -56 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 8 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 10 ) );
	}

	[Test]
	public static void Multiply_Rotor3_Trivector3() {
		Rotor3<int> rotor = new( 1, 2, 3, 4 );
		Trivector3<int> trivector = new( 5 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( rotor, trivector );
		Assert.That( result.Scalar, Is.EqualTo( 0 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -10 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -15 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -20 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 5 ) );
	}

	[Test]
	public static void Multiply_Rotor3_Rotor3() {
		Rotor3<int> rotorA = new( 1, 2, 3, 4 );
		Rotor3<int> rotorB = new( 5, 6, 7, 8 );

		Rotor3<int> result = GeometricAlgebraMath3.Multiply( rotorA, rotorB );
		Assert.That( result.Scalar, Is.EqualTo( -60 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 20 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 14 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 32 ) );
	}

	[Test]
	public static void Multiply_Rotor3_Multivector3() {
		Rotor3<int> rotor = new( 1, 2, 3, 4 );
		Multivector3<int> multivector = new( 5, 6, 7, 8, 9, 10, 11, 12 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( rotor, multivector );
		Assert.That( result.Scalar, Is.EqualTo( -87 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -14 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -37 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -36 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 26 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 11 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 38 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 77 ) );
	}

	[Test]
	public static void Multiply_Multivector3_Vector3() {
		Multivector3<int> multivector = new( 1, 2, 3, 4, 5, 6, 7, 8 );
		Vector3<int> vector = new( 9, 10, 11 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( multivector, vector );
		Assert.That( result.Scalar, Is.EqualTo( 92 ) );
		Assert.That( result.Vector.X, Is.EqualTo( 13 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( 2 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( 15 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 65 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 94 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 81 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 182 ) );
	}

	[Test]
	public static void Multiply_Multivector3_Bivector3() {
		Multivector3<int> multivector = new( 1, 2, 3, 4, 5, 6, 7, 8 );
		Bivector3<int> bivector = new( 9, 10, 11 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( multivector, bivector );
		Assert.That( result.Scalar, Is.EqualTo( -182 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -65 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -94 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -81 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 13 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 2 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 15 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 92 ) );
	}

	[Test]
	public static void Multiply_Multivector3_Trivector3() {
		Multivector3<int> multivector = new( 1, 2, 3, 4, 5, 6, 7, 8 );
		Trivector3<int> trivector = new( 9 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( multivector, trivector );
		Assert.That( result.Scalar, Is.EqualTo( -72 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -45 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -54 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -63 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 18 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 27 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 36 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 9 ) );
	}

	[Test]
	public static void Multiply_Multivector3_Rotor3() {
		Multivector3<int> multivector = new( 1, 2, 3, 4, 5, 6, 7, 8 );
		Rotor3<int> rotor = new( 9, 10, 11, 12 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( multivector, rotor );
		Assert.That( result.Scalar, Is.EqualTo( -191 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -54 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -77 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -52 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 60 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 55 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 80 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 173 ) );
	}

	[Test]
	public static void Multiply_Multivector3_Multivector3() {
		Multivector3<int> multivectorA = new( 1, 2, 3, 4, 5, 6, 7, 8 );
		Multivector3<int> multivectorB = new( 9, 10, 11, 12, 13, 14, 15, 16 );

		Multivector3<int> result = GeometricAlgebraMath3.Multiply( multivectorA, multivectorB );
		Assert.That( result.Scalar, Is.EqualTo( -272 ) );
		Assert.That( result.Vector.X, Is.EqualTo( -140 ) );
		Assert.That( result.Vector.Y, Is.EqualTo( -202 ) );
		Assert.That( result.Vector.Z, Is.EqualTo( -168 ) );
		Assert.That( result.Bivector.YZ, Is.EqualTo( 170 ) );
		Assert.That( result.Bivector.ZX, Is.EqualTo( 204 ) );
		Assert.That( result.Bivector.XY, Is.EqualTo( 238 ) );
		Assert.That( result.Trivector.XYZ, Is.EqualTo( 416 ) );
	}
}
