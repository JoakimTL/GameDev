using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Rotor2Tests {
	[Test]
	public void Constructors() {
		Rotor2<int> vectorA = new( 1, new( 2 ) );
		Rotor2<double> vectorB = new( 1.0, new( 2.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 2 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 2.0 ) );
	}
}
