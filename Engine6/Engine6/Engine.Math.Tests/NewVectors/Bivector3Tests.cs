using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Bivector3Tests {
	[Test]
	public void Constructors() {
		Bivector3<int> vectorA = new( 1, 2, 3 );
		Bivector3<double> vectorB = new( 1.0, 2.0, 3.0 );

		Assert.That( vectorA.YZ, Is.EqualTo( 1 ) );
		Assert.That( vectorA.ZX, Is.EqualTo( 2 ) );
		Assert.That( vectorA.XY, Is.EqualTo( 3 ) );

		Assert.That( vectorB.YZ, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.ZX, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.XY, Is.EqualTo( 3.0 ) );
	}
}
