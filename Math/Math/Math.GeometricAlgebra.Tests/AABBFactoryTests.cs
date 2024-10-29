namespace Math.GeometricAlgebra.Tests;

[TestFixture]
public sealed class AABBFactoryTests
{
	[Test]
	public void Create()
	{
		Span<Vector3<float>> vectors = [
			new(1, 1, 1),
			new(1.3f, 2, 0),
			new(0, 0, 0),
			new(-1, -1, -1),
			new(2.6f, 1.2f, 4.5f)
		];

		AABB<Vector3<float>> aabb = AABB.Create(vectors);

		Assert.That(aabb.Minima, Is.EqualTo(new Vector3<float>(-1, -1, -1)));
		Assert.That(aabb.Maxima, Is.EqualTo(new Vector3<float>(2.6f, 2, 4.5f)));
	}
}