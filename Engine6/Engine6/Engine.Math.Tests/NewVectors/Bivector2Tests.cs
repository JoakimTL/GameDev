using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Bivector2Tests {
	[Test]
	public void Constructors() {
		Bivector2<int> vectorA = new( 1 );
		Bivector2<double> vectorB = new( 1.0 );

		Assert.That( vectorA.XY, Is.EqualTo( 1 ) );

		Assert.That( vectorB.XY, Is.EqualTo( 1.0 ) );
	}
}
