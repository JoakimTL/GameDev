using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Vector3Tests {
	[Test]
	public void Constructors() {
		Vector3<int> vectorA = new( 1, 2, 3 );
		Vector3<double> vectorB = new( 1.0, 2.0, 3.0 );

		Assert.That( vectorA.X, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Y, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Z, Is.EqualTo( 3 ) );

		Assert.That( vectorB.X, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Y, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Z, Is.EqualTo( 3.0 ) );
	}
}
