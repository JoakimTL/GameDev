using Engine.Math.Operations;

namespace Engine.Math.Tests;

[TestFixture]
public sealed class Vector3OpsTests {

	[Test]
	public void Negate() {
		Vector3<int> a = new( 1, 2, 3 );

		Vector3<int> b = a.Negate();

		Assert.That( b.X, Is.EqualTo( -1 ) );
		Assert.That( b.Y, Is.EqualTo( -2 ) );
		Assert.That( b.Z, Is.EqualTo( -3 ) );
	}

	[Test]
	public void Add() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a.Add( b );

		Assert.That( c.X, Is.EqualTo( 5 ) );
		Assert.That( c.Y, Is.EqualTo( 7 ) );
		Assert.That( c.Z, Is.EqualTo( 9 ) );
	}

	[Test]
	public void Subtract() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a.Subtract( b );

		Assert.That( c.X, Is.EqualTo( -3 ) );
		Assert.That( c.Y, Is.EqualTo( -3 ) );
		Assert.That( c.Z, Is.EqualTo( -3 ) );
	}

	[Test]
	public void ScalarMultiply() {
		Vector3<int> a = new( 1, 2, 3 );

		Vector3<int> b = a.ScalarMultiply( 2 );

		Assert.That( b.X, Is.EqualTo( 2 ) );
		Assert.That( b.Y, Is.EqualTo( 4 ) );
		Assert.That( b.Z, Is.EqualTo( 6 ) );
	}

	[Test]
	public void ScalarDivide() {
		Vector3<double> a = new( 1, 2, 3 );

		Vector3<double> b = a.ScalarDivide( 2 );

		Assert.That( b.X, Is.EqualTo( 0.5 ) );
		Assert.That( b.Y, Is.EqualTo( 1 ) );
		Assert.That( b.Z, Is.EqualTo( 1.5 ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Vector3<int> c = a.MultiplyEntrywise( b );

		Assert.That( c.X, Is.EqualTo( 4 ) );
		Assert.That( c.Y, Is.EqualTo( 10 ) );
		Assert.That( c.Z, Is.EqualTo( 18 ) );
	}

	[Test]
	public void DivideEntrywise() {
		Vector3<double> a = new( 1, 2, 3 );
		Vector3<double> b = new( 4, 5, 6 );

		Vector3<double> c = a.DivideEntrywise( b );

		Assert.That( c.X, Is.EqualTo( 0.25 ) );
		Assert.That( c.Y, Is.EqualTo( 0.4 ) );
		Assert.That( c.Z, Is.EqualTo( 0.5 ) );
	}

	[Test]
	public void MultiplyVector() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		Rotor3<int> c = a.Multiply( b );

		Assert.That( c.Scalar, Is.EqualTo( (1 * 4) + (2 * 5) + (3 * 6) ) );
		Assert.That( c.Bivector.YZ, Is.EqualTo( (2 * 6) - (5 * 3) ) );
		Assert.That( c.Bivector.ZX, Is.EqualTo( (4 * 3) - (1 * 6) ) );
		Assert.That( c.Bivector.XY, Is.EqualTo( (1 * 5) - (4 * 2) ) );
	}

	[Test]
	public void Dot() {
		Vector3<int> a = new( 1, 2, 3 );
		Vector3<int> b = new( 4, 5, 6 );

		int c = a.Dot( b );

		Assert.That( c, Is.EqualTo( (1 * 4) + (2 * 5) + (3 * 6) ) );
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

	[Test]
	public void ReflectNormal() {
		Vector3<double> a = new( 1, 2, 3 );
		Vector3<double> normal = new( 0, 1, 0 );

		Vector3<double> b = a.ReflectNormal( normal );

		Assert.That( b.X, Is.EqualTo( -1 ) );
		Assert.That( b.Y, Is.EqualTo( 2 ) );
		Assert.That( b.Z, Is.EqualTo( -3 ) );
	}

	[Test]
	public void ReflectMirror() {
		Vector3<double> a = new( 1, 2, 3 );
		Vector3<double> normal = new( 0, 1, 0 );

		Vector3<double> b = a.ReflectMirror( normal );

		Assert.That( b.X, Is.EqualTo( 1 ) );
		Assert.That( b.Y, Is.EqualTo( -2 ) );
		Assert.That( b.Z, Is.EqualTo( 3 ) );
	}

	[Test]
	public void RotateFromAxisAngle() {
		Vector3<double> normal = new( 0, 0, 1 );

		Rotor3<double> rotor = Rotor3Factory.FromAxisAngle( normal, -System.Math.PI / 2 );

		Vector3<double> p = new( 1, 0, 0 );
		Vector3<double> q = p.Rotate( rotor );

		Assert.That( q.X, Is.EqualTo( 0 ).Within( 0.0001 ) );
		Assert.That( q.Y, Is.EqualTo( -1 ).Within( 0.0001 ) );
		Assert.That( q.Z, Is.EqualTo( 0 ).Within( 0.0001 ) );
	}

	[Test]
	public void RotateFromAxisAngle60Degrees() {
		Vector3<double> normal = new( 0, 0, 1 );

		double angle = 60d / 180 * System.Math.PI;

		Rotor3<double> rotor = Rotor3Factory.FromAxisAngle( normal, angle );

		Vector3<double> p = new( 1, 0, 0 );
		Vector3<double> q = p.Rotate( rotor );

		double sin = System.Math.Sin( angle );
		double cos = System.Math.Cos( angle );
		Assert.That( q.X, Is.EqualTo( cos ).Within( 0.0001 ) );
		Assert.That( q.Y, Is.EqualTo( sin ).Within( 0.0001 ) );
		Assert.That( q.Z, Is.EqualTo( 0 ).Within( 0.0001 ) );
	}

	[Test]
	public void RotateFromVectors() {
		Vector3<double> a = new( 1, 0, 0 );
		Vector3<double> b = new( 0, 1, 0 );

		Rotor3<double> rotor = Rotor3Factory.FromVectors( a, b );

		Vector3<double> p = new( 1, 0, 1 );
		Vector3<double> q = p.Rotate( rotor );

		Assert.That( q.X, Is.EqualTo( 0 ).Within( 0.0001 ) );
		Assert.That( q.Y, Is.EqualTo( 1 ).Within( 0.0001 ) );
		Assert.That( q.Z, Is.EqualTo( 1 ).Within( 0.0001 ) );
	}

	[Test]
	public void RotateFromVectorsReverse() {
		Vector3<double> a = new( 0, 1, 0 );
		Vector3<double> b = new( 0, 0, -1 );

		Rotor3<double> rotor = Rotor3Factory.FromVectors( a, b );

		Vector3<double> p = new( 1, 1, 0 );
		Vector3<double> q = p.Rotate( rotor );

		Assert.That( q.X, Is.EqualTo( 1 ).Within( 0.0001 ) );
		Assert.That( q.Y, Is.EqualTo( 0 ).Within( 0.0001 ) );
		Assert.That( q.Z, Is.EqualTo( -1 ).Within( 0.0001 ) );
	}
}