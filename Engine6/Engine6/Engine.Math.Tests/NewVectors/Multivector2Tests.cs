using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Multivector2Tests {
	[Test]
	public void Constructors() {
		Multivector2<int> vectorA = new( 1, new( 2, 3 ), new( 4 ) );
		Multivector2<double> vectorB = new( 1.0, new( 2.0, 3.0 ), new( 4.0 ) );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 4 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Vector.X, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Vector.Y, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 4.0 ) );
	}
}
