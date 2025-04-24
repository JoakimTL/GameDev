namespace Engine.Math.Tests;

[TestFixture]
public sealed class AABBExtensionsTests {
	[Test]
	public void Intersects() {
		//If this fails in higher dimensions, the implementation in higher dimensions are wrong.
		AABB<Vector2<float>> aabb2 = new( new Vector2<float>( -1, -1 ), new Vector2<float>( 1, 1 ) );

		bool intersects2_true_1 = aabb2.Intersects( new( new Vector2<float>( 0.5F, 0.5F ), new Vector2<float>( 2, 2 ) ) );
		bool intersects2_true_2 = aabb2.Intersects( new( new Vector2<float>( -1, -1 ), new Vector2<float>( -0.5f, -0.5f ) ) );
		bool intersects2_true_3 = aabb2.Intersects( new( new Vector2<float>( -1.5f, -1.5f ), new Vector2<float>( -0.75f, -1f ) ) );
		bool intersects2_true_4 = aabb2.Intersects( new( new Vector2<float>( -1.5f, -1.5f ), new Vector2<float>( -1f, -0.75f ) ) );
		bool intersects2_true_5 = aabb2.Intersects( new( new Vector2<float>( -1, -1 ), new Vector2<float>( 1, 1 ) ) );
		bool intersects2_false_1 = aabb2.Intersects( new( new Vector2<float>( 1.5F, 1.5F ), new Vector2<float>( 2, 2 ) ) );
		bool intersects2_false_2 = aabb2.Intersects( new( new Vector2<float>( -1.5F, -1.5F ), new Vector2<float>( -2, -2 ) ) );
		bool intersects2_false_3 = aabb2.Intersects( new( new Vector2<float>( -1, -1.5F ), new Vector2<float>( 1, -2 ) ) );
		bool intersects2_false_4 = aabb2.Intersects( new( new Vector2<float>( -1, 1.5F ), new Vector2<float>( 1, 2 ) ) );
		bool intersects2_false_5 = aabb2.Intersects( new( new Vector2<float>( -1.5F, -1 ), new Vector2<float>( -2, 1 ) ) );
		bool intersects2_false_6 = aabb2.Intersects( new( new Vector2<float>( 1.5F, -1 ), new Vector2<float>( 2, 1 ) ) );

		Assert.That( intersects2_true_1, Is.True );
		Assert.That( intersects2_true_2, Is.True );
		Assert.That( intersects2_true_3, Is.True );
		Assert.That( intersects2_true_4, Is.True );
		Assert.That( intersects2_true_5, Is.True );
		Assert.That( intersects2_false_1, Is.False );
		Assert.That( intersects2_false_2, Is.False );
		Assert.That( intersects2_false_3, Is.False );
		Assert.That( intersects2_false_4, Is.False );
		Assert.That( intersects2_false_5, Is.False );
		Assert.That( intersects2_false_6, Is.False );
	}

	[Test]
	public void GetArea() {
		AABB<Vector2<float>> aabb2_1 = new( new Vector2<float>( -1, -1 ), new Vector2<float>( 1, 1 ) );
		AABB<Vector2<float>> aabb2_2 = new( new Vector2<float>( 0, -1 ), new Vector2<float>( 5, 3 ) );

		float area1 = aabb2_1.GetArea();
		float area2 = aabb2_2.GetArea();

		Assert.That( area1, Is.EqualTo( 4 ) );
		Assert.That( area2, Is.EqualTo( 20 ) );
	}

	[Test]
	public void GetSurfaceArea() {
		AABB<Vector3<float>> aabb3_1 = new( new Vector3<float>( -1, -1, -1 ), new Vector3<float>( 1, 1, 1 ) );
		AABB<Vector3<float>> aabb3_2 = new( new Vector3<float>( 0, -1, 0 ), new Vector3<float>( 5, 3, 2 ) );

		float surfaceArea1 = aabb3_1.GetSurfaceArea();
		float surfaceArea2 = aabb3_2.GetSurfaceArea();

		Assert.That( surfaceArea1, Is.EqualTo( 24 ) );
		Assert.That( surfaceArea2, Is.EqualTo( 76 ) );
	}

	[Test]
	public void GetVolume() {
		AABB<Vector3<float>> aabb3_1 = new( new Vector3<float>( -1, -1, -1 ), new Vector3<float>( 1, 1, 1 ) );
		AABB<Vector3<float>> aabb3_2 = new( new Vector3<float>( 0, -1, 0 ), new Vector3<float>( 5, 3, 2 ) );

		float volume1 = aabb3_1.GetVolume();
		float volume2 = aabb3_2.GetVolume();

		Assert.That( volume1, Is.EqualTo( 8 ) );
		Assert.That( volume2, Is.EqualTo( 40 ) );
	}

	[Test]
	public void GetCenter() {
		AABB<Vector2<float>> aabb2_1 = new( new Vector2<float>( -1, -1 ), new Vector2<float>( 1, 1 ) );
		AABB<Vector2<float>> aabb2_2 = new( new Vector2<float>( 0, -1 ), new Vector2<float>( 5, 3 ) );

		Vector2<float> center1 = aabb2_1.GetCenter();
		Vector2<float> center2 = aabb2_2.GetCenter();

		Assert.That( center1, Is.EqualTo( new Vector2<float>( 0, 0 ) ) );
		Assert.That( center2, Is.EqualTo( new Vector2<float>( 2.5f, 1 ) ) );
	}

}
