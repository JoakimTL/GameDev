namespace Math.GeometricAlgebra.Tests;

[TestFixture]
public sealed class AABBTests
{
	#region Initializations
	[Test]
	public void Constructors()
	{
		AABB<Vector2<int>> aabbA = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		AABB<Vector2<int>> aabbB = new(new Vector2<int>(0, 2), new Vector2<int>(2, 0));
		AABB<Vector2<int>> aabbC = new(new Vector2<int>(-2, 4), new Vector2<int>(2, -3));

		Assert.That(aabbA.Minima.X, Is.EqualTo(0));
		Assert.That(aabbA.Minima.Y, Is.EqualTo(0));
		Assert.That(aabbA.Maxima.X, Is.EqualTo(1));
		Assert.That(aabbA.Maxima.Y, Is.EqualTo(1));

		Assert.That(aabbB.Minima.X, Is.EqualTo(0));
		Assert.That(aabbB.Minima.Y, Is.EqualTo(0));
		Assert.That(aabbB.Maxima.X, Is.EqualTo(2));
		Assert.That(aabbB.Maxima.Y, Is.EqualTo(2));

		Assert.That(aabbC.Minima.X, Is.EqualTo(-2));
		Assert.That(aabbC.Minima.Y, Is.EqualTo(-3));
		Assert.That(aabbC.Maxima.X, Is.EqualTo(2));
		Assert.That(aabbC.Maxima.Y, Is.EqualTo(4));
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int()
	{
		bool equal = new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1)) == new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		bool equalOpposite = new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1)) == new AABB<Vector2<int>>(new Vector2<int>(1, 1), new Vector2<int>(0, 0));
		bool notequal_isfalse = new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1)) == new AABB<Vector2<int>>(new Vector2<int>(-1, 1), new Vector2<int>(0, 0));

		Assert.That(equal, Is.True);
		Assert.That(equalOpposite, Is.True);
		Assert.That(notequal_isfalse, Is.False);
	}

	[Test]
	public void NotEqualOperator_Int()
	{
		bool equal_isfalse = new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1)) != new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		bool equalOpposite_isfalse = new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1)) != new AABB<Vector2<int>>(new Vector2<int>(1, 1), new Vector2<int>(0, 0));
		bool notequal = new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1)) != new AABB<Vector2<int>>(new Vector2<int>(-1, 1), new Vector2<int>(0, 0));

		Assert.That(equal_isfalse, Is.False);
		Assert.That(equalOpposite_isfalse, Is.False);
		Assert.That(notequal, Is.True);
	}
	#endregion

	#region Methods
	[Test]
	public void Extend()
	{
		AABB<Vector2<int>> aabbA = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		AABB<Vector2<int>> extended = aabbA.Extend(new Vector2<int>(2, 2));

		AABB<Vector2<int>> expected = new(new Vector2<int>(0, 0), new Vector2<int>(2, 2));
		Assert.That(extended, Is.EqualTo(expected));
	}

	[Test]
	public void GetLargestBounds()
	{
		AABB<Vector2<int>> aabbA = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		AABB<Vector2<int>> aabbB = new(new Vector2<int>(0, 2), new Vector2<int>(2, 0));
		AABB<Vector2<int>> largest = aabbA.GetLargestBounds(aabbB);

		AABB<Vector2<int>> expected = new(new Vector2<int>(0, 0), new Vector2<int>(2, 2));
		Assert.That(largest, Is.EqualTo(expected));
	}

	[Test]
	public void GetSmallestBounds()
	{
		AABB<Vector2<int>> aabbA = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		AABB<Vector2<int>> aabbB = new(new Vector2<int>(0, 2), new Vector2<int>(2, 0));
		AABB<Vector2<int>> smallest = aabbA.GetSmallestBounds(aabbB);

		AABB<Vector2<int>> expected = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		Assert.That(smallest, Is.EqualTo(expected));
	}

	[Test]
	public void Test_Equals()
	{
		AABB<Vector2<int>> aabb = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));

		Assert.That(aabb.Equals(aabb), Is.True);
		Assert.That(aabb.Equals(new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(1, 1))), Is.True);
		Assert.That(aabb.Equals(new AABB<Vector2<int>>(new Vector2<int>(0, 0), new Vector2<int>(2, 1))), Is.False);
		Assert.That(aabb.Equals("Test"), Is.False);
	}

	[Test]
	public void Test_GetHashCode()
	{
		AABB<Vector2<int>> aabbA = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		AABB<Vector2<int>> aabbB = new(new Vector2<int>(0, 0), new Vector2<int>(1, 1));
		AABB<Vector2<int>> aabbC = new(new Vector2<int>(0, 0), new Vector2<int>(2, 2));
		AABB<Vector2<int>> aabbD = new(new Vector2<int>(0, 2), new Vector2<int>(2, 0));

		Assert.That(aabbA.GetHashCode(), Is.EqualTo(aabbB.GetHashCode()));
		Assert.That(aabbA.GetHashCode(), Is.Not.EqualTo(aabbC.GetHashCode()));
		Assert.That(aabbA.GetHashCode(), Is.Not.EqualTo(aabbD.GetHashCode()));
		Assert.That(aabbC.GetHashCode(), Is.EqualTo(aabbD.GetHashCode()));
	}

	[Test]
	public void Test_ToString()
	{
		AABB<Vector2<int>> aabbA = new(new Vector2<int>(0, 0), new Vector2<int>(1, 10000));

		Assert.That(aabbA.ToString(), Is.EqualTo($"<{aabbA.Minima} -> {aabbA.Maxima}>"));
	}
	#endregion
}