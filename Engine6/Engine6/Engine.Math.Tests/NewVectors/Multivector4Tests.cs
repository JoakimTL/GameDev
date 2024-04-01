using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Multivector4Tests {
	[Test]
	public void Constructors() {
		Multivector4<int> vectorA = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		Multivector4<double> vectorB = new( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Vector.Z, Is.EqualTo( 4 ) );
		Assert.That( vectorA.Vector.W, Is.EqualTo( 5 ) );
		Assert.That( vectorA.Bivector.YZ, Is.EqualTo( 6 ) );
		Assert.That( vectorA.Bivector.ZX, Is.EqualTo( 7 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 8 ) );
		Assert.That( vectorA.Bivector.YW, Is.EqualTo( 9 ) );
		Assert.That( vectorA.Bivector.ZW, Is.EqualTo( 10 ) );
		Assert.That( vectorA.Bivector.XW, Is.EqualTo( 11 ) );
		Assert.That( vectorA.Trivector.YZW, Is.EqualTo( 12 ) );
		Assert.That( vectorA.Trivector.XZW, Is.EqualTo( 13 ) );
		Assert.That( vectorA.Trivector.XYW, Is.EqualTo( 14 ) );
		Assert.That( vectorA.Trivector.XYZ, Is.EqualTo( 15 ) );
		Assert.That( vectorA.Quadvector.XYZW, Is.EqualTo( 16 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Vector.X, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Vector.Y, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Vector.Z, Is.EqualTo( 4.0 ) );
		Assert.That( vectorB.Vector.W, Is.EqualTo( 5.0 ) );
		Assert.That( vectorB.Bivector.YZ, Is.EqualTo( 6.0 ) );
		Assert.That( vectorB.Bivector.ZX, Is.EqualTo( 7.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 8.0 ) );
		Assert.That( vectorB.Bivector.YW, Is.EqualTo( 9.0 ) );
		Assert.That( vectorB.Bivector.ZW, Is.EqualTo( 10.0 ) );
		Assert.That( vectorB.Bivector.XW, Is.EqualTo( 11.0 ) );
		Assert.That( vectorB.Trivector.YZW, Is.EqualTo( 12.0 ) );
		Assert.That( vectorB.Trivector.XZW, Is.EqualTo( 13.0 ) );
		Assert.That( vectorB.Trivector.XYW, Is.EqualTo( 14.0 ) );
		Assert.That( vectorB.Trivector.XYZ, Is.EqualTo( 15.0 ) );
		Assert.That( vectorB.Quadvector.XYZW, Is.EqualTo( 16.0 ) );
	}
}
