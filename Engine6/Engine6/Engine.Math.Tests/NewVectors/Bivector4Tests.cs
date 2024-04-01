using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Bivector4Tests {
	[Test]
	public void Constructors() {
		Bivector4<int> vectorA = new( 1, 2, 3, 4, 5, 6 );
		Bivector4<double> vectorB = new( 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 );

		Assert.That( vectorA.YZ, Is.EqualTo( 1 ) );
		Assert.That( vectorA.ZX, Is.EqualTo( 2 ) );
		Assert.That( vectorA.XY, Is.EqualTo( 3 ) );
		Assert.That( vectorA.YW, Is.EqualTo( 4 ) );
		Assert.That( vectorA.ZW, Is.EqualTo( 5 ) );
		Assert.That( vectorA.XW, Is.EqualTo( 6 ) );

		Assert.That( vectorB.YZ, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.ZX, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.XY, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.YW, Is.EqualTo( 4.0 ) );
		Assert.That( vectorB.ZW, Is.EqualTo( 5.0 ) );
		Assert.That( vectorB.XW, Is.EqualTo( 6.0 ) );
	}
}
