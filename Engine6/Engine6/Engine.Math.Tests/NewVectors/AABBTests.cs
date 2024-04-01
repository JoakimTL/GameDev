using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class AABBTests {
	[Test]
	public void Constructors() {
		AABB<Vector2<int>, int> aabbA = new( new Vector2<int>( 0, 0 ), new Vector2<int>( 1, 1 ) );
		AABB<Vector2<int>, int> aabbB = new( new Vector2<int>( 0, 2 ), new Vector2<int>( 2, 0 ) );
		AABB<Vector2<int>, int> aabbC = new( new Vector2<int>( -2, 4 ), new Vector2<int>( 2, -3 ) );

		Assert.That( aabbA.Minima.X, Is.EqualTo( 0 ) );
		Assert.That( aabbA.Minima.Y, Is.EqualTo( 0 ) );
		Assert.That( aabbA.Maxima.X, Is.EqualTo( 1 ) );
		Assert.That( aabbA.Maxima.Y, Is.EqualTo( 1 ) );

		Assert.That( aabbB.Minima.X, Is.EqualTo( 0 ) );
		Assert.That( aabbB.Minima.Y, Is.EqualTo( 0 ) );
		Assert.That( aabbB.Maxima.X, Is.EqualTo( 2 ) );
		Assert.That( aabbB.Maxima.Y, Is.EqualTo( 2 ) );

		Assert.That( aabbC.Minima.X, Is.EqualTo( -2 ) );
		Assert.That( aabbC.Minima.Y, Is.EqualTo( -3 ) );
		Assert.That( aabbC.Maxima.X, Is.EqualTo( 2 ) );
		Assert.That( aabbC.Maxima.Y, Is.EqualTo( 4 ) );
	}
}
