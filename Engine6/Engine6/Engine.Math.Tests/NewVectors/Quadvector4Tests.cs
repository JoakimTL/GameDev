using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Quadvector4Tests {
	[Test]
	public void Constructors() {
		Quadvector4<int> vectorA = new( 1 );
		Quadvector4<double> vectorB = new( 1.0 );

		Assert.That( vectorA.XYZW, Is.EqualTo( 1 ) );

		Assert.That( vectorB.XYZW, Is.EqualTo( 1.0 ) );
	}
}
