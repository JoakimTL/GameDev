using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Trivector4Tests {
	[Test]
	public void Constructors() {
		Trivector4<int> vectorA = new( 1, 2, 3, 4 );
		Trivector4<double> vectorB = new( 1.0, 2.0, 3, 4 );

		Assert.That( vectorA.YZW, Is.EqualTo( 1 ) );
		Assert.That( vectorA.XZW, Is.EqualTo( 2 ) );
		Assert.That( vectorA.XYW, Is.EqualTo( 3 ) );
		Assert.That( vectorA.XYZ, Is.EqualTo( 4 ) );

		Assert.That( vectorB.YZW, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.XZW, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.XYW, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.XYZ, Is.EqualTo( 4.0 ) );
	}
}
