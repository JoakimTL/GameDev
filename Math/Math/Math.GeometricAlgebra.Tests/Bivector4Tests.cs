namespace Math.GeometricAlgebra.Tests;

[TestFixture]
public sealed class Bivector4Tests
{
	#region Initializations
	[Test]
	public void Constructors()
	{
		Bivector4<int> vectorA = new(1, 2, 3, 4, 5, 6);
		Bivector4<double> vectorB = new(1.0, 2.0, 3.0, 4.0, 5.0, 6.0);

		Assert.That(vectorA.YZ, Is.EqualTo(1));
		Assert.That(vectorA.ZX, Is.EqualTo(2));
		Assert.That(vectorA.XY, Is.EqualTo(3));
		Assert.That(vectorA.YW, Is.EqualTo(4));
		Assert.That(vectorA.ZW, Is.EqualTo(5));
		Assert.That(vectorA.XW, Is.EqualTo(6));

		Assert.That(vectorB.YZ, Is.EqualTo(1.0));
		Assert.That(vectorB.ZX, Is.EqualTo(2.0));
		Assert.That(vectorB.XY, Is.EqualTo(3.0));
		Assert.That(vectorB.YW, Is.EqualTo(4.0));
		Assert.That(vectorB.ZW, Is.EqualTo(5.0));
		Assert.That(vectorB.XW, Is.EqualTo(6.0));
	}

	[Test]
	public void Identities()
	{
		Bivector4<int> addtitiveIdentityA = Bivector4<int>.AdditiveIdentity;
		Bivector4<int> multiplicativeIdentityA = Bivector4<int>.MultiplicativeIdentity;
		Bivector4<int> zeroA = Bivector4<int>.Zero;
		Bivector4<int> oneA = Bivector4<int>.One;
		Bivector4<double> addtitiveIdentityB = Bivector4<double>.AdditiveIdentity;
		Bivector4<double> multiplicativeIdentityB = Bivector4<double>.MultiplicativeIdentity;
		Bivector4<double> zeroB = Bivector4<double>.Zero;
		Bivector4<double> oneB = Bivector4<double>.One;

		Assert.That(addtitiveIdentityA, Is.EqualTo(zeroA));
		Assert.That(multiplicativeIdentityA, Is.EqualTo(oneA));

		Assert.That(zeroA.YZ, Is.EqualTo(0));
		Assert.That(zeroA.ZX, Is.EqualTo(0));
		Assert.That(zeroA.XY, Is.EqualTo(0));
		Assert.That(zeroA.YW, Is.EqualTo(0));
		Assert.That(zeroA.ZW, Is.EqualTo(0));
		Assert.That(zeroA.XW, Is.EqualTo(0));

		Assert.That(oneA.YZ, Is.EqualTo(1));
		Assert.That(oneA.ZX, Is.EqualTo(1));
		Assert.That(oneA.XY, Is.EqualTo(1));
		Assert.That(oneA.YW, Is.EqualTo(1));
		Assert.That(oneA.ZW, Is.EqualTo(1));
		Assert.That(oneA.XW, Is.EqualTo(1));

		Assert.That(addtitiveIdentityB, Is.EqualTo(zeroB));
		Assert.That(multiplicativeIdentityB, Is.EqualTo(oneB));

		Assert.That(zeroB.YZ, Is.EqualTo(0.0));
		Assert.That(zeroB.ZX, Is.EqualTo(0.0));
		Assert.That(zeroB.XY, Is.EqualTo(0.0));
		Assert.That(zeroB.YW, Is.EqualTo(0.0));
		Assert.That(zeroB.ZW, Is.EqualTo(0.0));
		Assert.That(zeroB.XW, Is.EqualTo(0.0));
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int()
	{
		bool equal = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(1, 2, 3, 4, 5, 6);
		bool notequalyz_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(0, 2, 3, 4, 5, 6);
		bool notequalzx_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(1, 0, 3, 4, 5, 6);
		bool notequalxy_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(1, 2, 0, 4, 5, 6);
		bool notequalyw_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(1, 2, 3, 0, 5, 6);
		bool notequalzw_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(1, 2, 3, 4, 0, 6);
		bool notequalxw_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) == new Bivector4<int>(1, 2, 3, 4, 5, 0);

		Assert.That(equal, Is.True);
		Assert.That(notequalyz_isfalse, Is.False);
		Assert.That(notequalzx_isfalse, Is.False);
		Assert.That(notequalxy_isfalse, Is.False);
		Assert.That(notequalyw_isfalse, Is.False);
		Assert.That(notequalzw_isfalse, Is.False);
		Assert.That(notequalxw_isfalse, Is.False);
	}

	[Test]
	public void NotEqualOperator_Int()
	{
		bool equal_isfalse = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(1, 2, 3, 4, 5, 6);
		bool notequalyz = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(0, 2, 3, 4, 5, 6);
		bool notequalzx = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(1, 0, 3, 4, 5, 6);
		bool notequalxy = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(1, 2, 0, 4, 5, 6);
		bool notequalyw = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(1, 2, 3, 0, 5, 6);
		bool notequalzw = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(1, 2, 3, 4, 0, 6);
		bool notequalxw = new Bivector4<int>(1, 2, 3, 4, 5, 6) != new Bivector4<int>(1, 2, 3, 4, 5, 0);

		Assert.That(equal_isfalse, Is.False);
		Assert.That(notequalyz, Is.True);
		Assert.That(notequalzx, Is.True);
		Assert.That(notequalxy, Is.True);
		Assert.That(notequalyw, Is.True);
		Assert.That(notequalzw, Is.True);
		Assert.That(notequalxw, Is.True);
	}

	[Test]
	public void EqualOperator_Double()
	{
		bool equal = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
		bool notequalyz_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(0.0, 2.0, 3.0, 4.0, 5.0, 6.0);
		bool notequalzx_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(1.0, 0.0, 3.0, 4.0, 5.0, 6.0);
		bool notequalxy_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(1.0, 2.0, 0.0, 4.0, 5.0, 6.0);
		bool notequalyw_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(1.0, 2.0, 3.0, 0.0, 5.0, 6.0);
		bool notequalzw_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 0.0, 6.0);
		bool notequalxw_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) == new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 0.0);

		Assert.That(equal, Is.True);
		Assert.That(notequalyz_isfalse, Is.False);
		Assert.That(notequalzx_isfalse, Is.False);
		Assert.That(notequalxy_isfalse, Is.False);
		Assert.That(notequalyw_isfalse, Is.False);
		Assert.That(notequalzw_isfalse, Is.False);
		Assert.That(notequalxw_isfalse, Is.False);
	}

	[Test]
	public void NotEqualOperator_Double()
	{
		bool equal_isfalse = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0);
		bool notequalyz = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(0.0, 2.0, 3.0, 4.0, 5.0, 6.0);
		bool notequalzx = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(1.0, 0.0, 3.0, 4.0, 5.0, 6.0);
		bool notequalxy = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(1.0, 2.0, 0.0, 4.0, 5.0, 6.0);
		bool notequalyw = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(1.0, 2.0, 3.0, 0.0, 5.0, 6.0);
		bool notequalzw = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 0.0, 6.0);
		bool notequalxw = new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 6.0) != new Bivector4<double>(1.0, 2.0, 3.0, 4.0, 5.0, 0.0);

		Assert.That(equal_isfalse, Is.False);
		Assert.That(notequalyz, Is.True);
		Assert.That(notequalzx, Is.True);
		Assert.That(notequalxy, Is.True);
		Assert.That(notequalyw, Is.True);
		Assert.That(notequalzw, Is.True);
		Assert.That(notequalxw, Is.True);
	}

	[Test]
	public void NegateOperator()
	{
		Bivector4<int> vectorA = new(1, 2, 3, 4, 5, 6);

		Bivector4<int> expected = new(-1, -2, -3, -4, -5, -6);
		Assert.That(-vectorA, Is.EqualTo(expected));
	}

	[Test]
	public void AddOperator()
	{
		Bivector4<int> vectorA = new(1, 2, 3, 4, 5, 6);
		Bivector4<int> vectorB = new(7, 8, 9, 10, 11, 12);

		Bivector4<int> expected = new(8, 10, 12, 14, 16, 18);
		Assert.That(vectorA + vectorB, Is.EqualTo(expected));
	}

	[Test]
	public void SubtractOperator()
	{
		Bivector4<int> vectorA = new(1, 2, 3, 4, 5, 6);
		Bivector4<int> vectorB = new(7, 8, 9, 10, 11, 12);

		Bivector4<int> expected = new(-6, -6, -6, -6, -6, -6);
		Assert.That(vectorA - vectorB, Is.EqualTo(expected));
	}

	[Test]
	public void ScalarMultiplyOperator()
	{
		Bivector4<int> vectorA = new(3, 4, 5, 6, 7, 8);

		Bivector4<int> expected = new Bivector4<int>(6, 8, 10, 12, 14, 16);
		Assert.That(vectorA * 2, Is.EqualTo(expected));
		Assert.That(2 * vectorA, Is.EqualTo(expected));
	}

	[Test]
	public void ScalarDivideOperator()
	{
		Bivector4<int> vectorA = new(6, 8, 10, 12, 14, 16);

		Bivector4<int> expected = new(3, 4, 5, 6, 7, 8);
		Assert.That(vectorA / 2, Is.EqualTo(expected));
	}

	[Test]
	public void DivideScalarOperator()
	{
		Bivector4<int> vectorA = new(6, 8, 10, 12, 14, 16);

		Bivector4<int> expected = new(8, 6, 4, 4, 3, 3);
		Assert.That(48 / vectorA, Is.EqualTo(expected));
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector()
	{
		Bivector4<int> bivector = new(1, 2, 3, 4, 5, 6);

		Multivector4<int> multivector = bivector.GetMultivector();

		Assert.That(multivector.Scalar, Is.EqualTo(0));
		Assert.That(multivector.Vector.X, Is.EqualTo(0));
		Assert.That(multivector.Vector.Y, Is.EqualTo(0));
		Assert.That(multivector.Vector.Z, Is.EqualTo(0));
		Assert.That(multivector.Vector.W, Is.EqualTo(0));
		Assert.That(multivector.Bivector.YZ, Is.EqualTo(1));
		Assert.That(multivector.Bivector.ZX, Is.EqualTo(2));
		Assert.That(multivector.Bivector.XY, Is.EqualTo(3));
		Assert.That(multivector.Bivector.YW, Is.EqualTo(4));
		Assert.That(multivector.Bivector.ZW, Is.EqualTo(5));
		Assert.That(multivector.Bivector.XW, Is.EqualTo(6));
		Assert.That(multivector.Trivector.YZW, Is.EqualTo(0));
		Assert.That(multivector.Trivector.XZW, Is.EqualTo(0));
		Assert.That(multivector.Trivector.XYW, Is.EqualTo(0));
		Assert.That(multivector.Trivector.XYZ, Is.EqualTo(0));
		Assert.That(multivector.Quadvector.XYZW, Is.EqualTo(0));
	}

	[Test]
	public void Dot()
	{
		Bivector4<int> bivectorA = new(1, 2, 3, 4, 5, 6);
		Bivector4<int> bivectorB = new(7, 8, 9, 10, 11, 12);

		int expected = -217;

		Assert.That(bivectorA.Dot(bivectorB), Is.EqualTo(expected));
	}

	[Test]
	public void MagnitudeSquared()
	{
		Bivector4<int> bivector = new(1, 2, 3, 4, 5, 6);

		int expected = 91;

		Assert.That(bivector.MagnitudeSquared(), Is.EqualTo(expected));
	}

	[Test]
	public void Test_Equals()
	{
		Bivector4<int> bivector = new(1, 2, 3, 4, 5, 6);

		Assert.That(bivector.Equals(bivector), Is.True);
		Assert.That(bivector.Equals(new Bivector4<int>(1, 2, 3, 4, 5, 6)), Is.True);
		Assert.That(bivector.Equals(new Bivector4<int>(2, 5, 7, 9, 1, 3)), Is.False);
		Assert.That(bivector.Equals("Test"), Is.False);
	}

	[Test]
	public void Test_GetHashCode()
	{
		Bivector4<int> bivector_1 = new(1, 2, 3, 4, 5, 6);
		Bivector4<int> bivector_2 = new(1, 2, 3, 4, 5, 6);
		Bivector4<int> bivector_3 = new(2, 5, 7, 9, 11, 13);
		Bivector4<int> bivector_4 = new(2, 5, 7, 9, 11, 13);

		Assert.That(bivector_1.GetHashCode(), Is.EqualTo(bivector_2.GetHashCode()));
		Assert.That(bivector_1.GetHashCode(), Is.Not.EqualTo(bivector_3.GetHashCode()));
		Assert.That(bivector_3.GetHashCode(), Is.EqualTo(bivector_4.GetHashCode()));
	}

	[Test]
	public void Test_ToString()
	{
		Bivector4<int> bivector_1 = new(1, 2, 3, 4, 5, 6);
		Bivector4<int> bivector_2 = new(-56, 67, 1200, 6, 9, -1);
		Bivector4<double> bivector_3 = new(-69.4201, 70.8882, 23.233, 60.6069, 420.4205, 666.6666);
		Bivector4<int> allNegative = new(-1, -2, -3, -4, -5, -6);

		Assert.That(bivector_1.ToString(), Is.EqualTo("[1YZ + 2ZX + 3XY + 4YW + 5ZW + 6XW]"));
		Assert.That(bivector_2.ToString(), Is.EqualTo("[-56YZ + 67ZX + 1,200XY + 6YW + 9ZW - 1XW]"));
		Assert.That(bivector_3.ToString(), Is.EqualTo("[-69.42YZ + 70.888ZX + 23.233XY + 60.607YW + 420.421ZW + 666.667XW]"));
		Assert.That(allNegative.ToString(), Is.EqualTo("[-1YZ - 2ZX - 3XY - 4YW - 5ZW - 6XW]"));
	}
	#endregion
}
