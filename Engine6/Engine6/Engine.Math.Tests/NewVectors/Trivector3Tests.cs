using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Trivector3Tests {
	[Test]
	public void Constructors() {
		Trivector3<int> vectorA = new( 1 );
		Trivector3<double> vectorB = new( 1.0 );

		Assert.That( vectorA.XYZ, Is.EqualTo( 1 ) );

		Assert.That( vectorB.XYZ, Is.EqualTo( 1.0 ) );
	}
}
