namespace Engine.Math.Tests;

[TestFixture]
public sealed class Multivector3Tests {

	[Test]
	public void Addition_1() {
		Multivector3<double> a = new( 1, 2, 3, 5, 7, 11, 13, 17 );
		Multivector3<double> b = new( 3, 5, 7, 11, 13, 17, 1, 2 );

		Multivector3<double> c = a + b;

		Assert.That( c.Scalar, Is.EqualTo( 4 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 7, 10, 16 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 20, 28, 14 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 19 ) ) );
	}

	[Test]
	public void Multiplication_ScalarOnly_1() {
		Multivector3<double> a = new( 1, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 3, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 3 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_VectorOnly_1() {
		Multivector3<double> a = new( 0, new Vector3<double>( 1, 2, 3 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, new Vector3<double>( 3, 5, 7 ), Bivector3<double>.Zero, 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 34 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( -1, 2, -1 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_VectorOnly_2() {
		Multivector3<double> a = new( 0, new Vector3<double>( 4, 4, 4 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, new Vector3<double>( -2, -6, -12 ), Bivector3<double>.Zero, 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( -80 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( -24, 40, -16 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_BivectorOnly_1() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, new Bivector3<double>( 1, 2, 3 ), 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( 3, 5, 7 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( -34 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 1, -2, 1 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_BivectorOnly_2() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, new Bivector3<double>( 4, 4, 4 ), 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( -2, -6, -12 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 80 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 24, -40, 16 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_TrivectorOnly_1() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 1 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( -4 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Scalar_Vector_1() {
		Multivector3<double> a = new( 3, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, new( 1, 2, 3 ), Bivector3<double>.Zero, 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 3, 6, 9 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Scalar_Vector_2() {
		Multivector3<double> a = new( 0, new( -3, 4, 7 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 2, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( -6, 8, 14 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Scalar_Bivector_1() {
		Multivector3<double> a = new( 3, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( 1, 2, 3 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 3, 6, 9 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Scalar_Bivector_2() {
		Multivector3<double> a = new( 2, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( -3, 4, 7 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( -6, 8, 14 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Scalar_Trivector_1() {
		Multivector3<double> a = new( 3, Vector3<double>.Zero, Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 1 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 3 ) ) );
	}

	[Test]
	public void Multiplication_Vector_x_Bivector_1() {
		Multivector3<double> a = new( 0, new( 1, 0, 0 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( 3, 5, 7 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 0, 7, -5 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 3 ) ) );
	}

	[Test]
	public void Multiplication_Vector_y_Bivector_1() {
		Multivector3<double> a = new( 0, new( 0, 1, 0 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( 3, 5, 7 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( -7, 0, 3 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 5 ) ) );
	}

	[Test]
	public void Multiplication_Vector_z_Bivector_1() {
		Multivector3<double> a = new( 0, new( 0, 0, 1 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, new Bivector3<double>( 3, 5, 7 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 5, -3, 0 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 7 ) ) );
	}

	[Test]
	public void Multiplication_Vector_x_Trivector_1() {
		Multivector3<double> a = new( 0, new( 1, 0, 0 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 4, 0, 0 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Vector_y_Trivector_1() {
		Multivector3<double> a = new( 0, new( 0, 1, 0 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 0, 4, 0 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Vector_z_Trivector_1() {
		Multivector3<double> a = new( 0, new( 0, 0, 1 ), Bivector3<double>.Zero, 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 0, 0, 4 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Bivector_yz_Trivector_1() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, new Bivector3<double>( 1, 0, 0 ), 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( -4, 0, 0 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Bivector_zx_Trivector_1() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, new Bivector3<double>( 0, 1, 0 ), 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 0, -4, 0 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Bivector_xy_Trivector_1() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, new Bivector3<double>( 0, 0, 1 ), 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 0, 0, -4 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Bivector_Trivector_1() {
		Multivector3<double> a = new( 0, Vector3<double>.Zero, new Bivector3<double>( 1, 1, 1 ), 0 );
		Multivector3<double> b = new( 0, Vector3<double>.Zero, Bivector3<double>.Zero, 4 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 0 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( -4, -4, -4 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( Bivector3<double>.Zero ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 0 ) ) );
	}

	[Test]
	public void Multiplication_Multivector_Multivector() {
		Multivector3<double> a = new( 2, new( 2, 3, 5 ), new( -3, -5, 13 ), 1 );
		Multivector3<double> b = new( 3, new( 7, -2, 11 ), new( 1.5, 2.5, 3.5 ), 0 );

		Multivector3<double> c = a * b;

		Assert.That( c.Scalar, Is.EqualTo( 40.5 ) );
		Assert.That( c.Vector, Is.EqualTo( new Vector3<double>( 49.5, -122, -8 ) ) );
		Assert.That( c.Bivector, Is.EqualTo( new Bivector3<double>( 94, -29, 32 ) ) );
		Assert.That( c.Trivector, Is.EqualTo( new Trivector3<double>( 163 ) ) );
	}

}
