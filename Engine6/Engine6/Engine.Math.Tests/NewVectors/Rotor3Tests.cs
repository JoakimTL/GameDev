using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Rotor3Tests {
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
}
