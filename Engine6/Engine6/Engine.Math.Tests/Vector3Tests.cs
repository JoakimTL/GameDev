namespace Engine.Math.Tests;

[TestFixture]
public sealed class Vector3Tests {

	[Test]
	public void	Addition() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a + b;

		Assert.That( c.X, Is.EqualTo( 5 ) );
		Assert.That( c.Y, Is.EqualTo( 7 ) );
		Assert.That( c.Z, Is.EqualTo( 9 ) );
	}

	[Test]
	public void Subtraction() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a - b;

		Assert.That( c.X, Is.EqualTo( -3 ) );
		Assert.That( c.Y, Is.EqualTo( -3 ) );
		Assert.That( c.Z, Is.EqualTo( -3 ) );
	}

	[Test]
	public void ScalarMultiplication() {
		Vector3<int> a = new( 1, 2, 3 );

		Vector3<int> b = a * 2;

		Assert.That( b.X, Is.EqualTo( 2 ) );
		Assert.That( b.Y, Is.EqualTo( 4 ) );
		Assert.That( b.Z, Is.EqualTo( 6 ) );
	}

	[Test]
	public void ScalarDivision() {
		Vector3<int> a = new( 1, 2, 3 );

		Vector3<int> b = a / 2;

		Assert.That( b.X, Is.EqualTo( 0 ) );
		Assert.That( b.Y, Is.EqualTo( 1 ) );
		Assert.That( b.Z, Is.EqualTo( 1 ) );
	}

	[Test]
	public void ScalarDivision2() {
		Vector3<double> a = new( 1, 2, 3 );

		Vector3<double> b = a / 2;

		Assert.That( b.X, Is.EqualTo( 0.5 ) );
		Assert.That( b.Y, Is.EqualTo( 1 ) );
		Assert.That( b.Z, Is.EqualTo( 1.5 ) );
	}

	[Test]
	public void EntrywiseMultiplication() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a * b;

		Assert.That( c.X, Is.EqualTo( 4 ) );
		Assert.That( c.Y, Is.EqualTo( 10 ) );
		Assert.That( c.Z, Is.EqualTo( 18 ) );
	}

	[Test]
	public void EntrywiseDivision() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a / b;

		Assert.That( c.X, Is.EqualTo( 0 ) );
		Assert.That( c.Y, Is.EqualTo( 0 ) );
		Assert.That( c.Z, Is.EqualTo( 0 ) );
	}

	[Test]
	public void Negation() {
		Vector3<int> a = new( 1, 2, 3 );

		Vector3<int> b = -a;

		Assert.That( b.X, Is.EqualTo( -1 ) );
		Assert.That( b.Y, Is.EqualTo( -2 ) );
		Assert.That( b.Z, Is.EqualTo( -3 ) );
	}

	[Test]
	public void ImplicitConversion() {
		(int, int, int) a = ( 1, 2, 3 );

		Vector3<int> b = a;

		Assert.That( b.X, Is.EqualTo( 1 ) );
		Assert.That( b.Y, Is.EqualTo( 2 ) );
		Assert.That( b.Z, Is.EqualTo( 3 ) );
	}

	[Test]
	public void Wedge() {
		Vector3<int> a = new( 1, 0, 0 );
		Vector3<int> b = new( 0, 1, 0 );

		Bivector3<int> c = a.Wedge( b );

		Assert.That( c.YZ, Is.EqualTo( 0 ) );
		Assert.That( c.ZX, Is.EqualTo( 0 ) );
		Assert.That( c.XY, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Wedge2() {
		Vector3<double> a = new( 1, 0, 1 );
		Vector3<double> b = new( 0, 1, 0 );

		Bivector3<double> c = a.Wedge( b );

		Assert.That( c.YZ, Is.EqualTo( -1 ) );
		Assert.That( c.ZX, Is.EqualTo( 0 ) );
		Assert.That( c.XY, Is.EqualTo( 1 ) );
	}
}
