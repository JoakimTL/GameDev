using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Multivector3Tests {
	[Test]
	public void Constructors() {
		Multivector3<int> vectorA = new( 1, new( 2, 3, 4 ), new( 5, 6, 7 ), new( 8 ) );
		Multivector3<double> vectorB = new( 1.0, new( 2.0, 3.0, 4.0 ), new( 5.0, 6.0, 7.0 ), new( 8.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Vector.Z, Is.EqualTo( 4 ) );
		Assert.That( vectorA.Bivector.YZ, Is.EqualTo( 5 ) );
		Assert.That( vectorA.Bivector.ZX, Is.EqualTo( 6 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 7 ) );
		Assert.That( vectorA.Trivector.XYZ, Is.EqualTo( 8 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Vector.X, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Vector.Y, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Vector.Z, Is.EqualTo( 4.0 ) );
		Assert.That( vectorB.Bivector.YZ, Is.EqualTo( 5.0 ) );
		Assert.That( vectorB.Bivector.ZX, Is.EqualTo( 6.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 7.0 ) );
		Assert.That( vectorB.Trivector.XYZ, Is.EqualTo( 8.0 ) );
	}
}
