namespace Math.GeometricAlgebra.Tests;

[TestFixture]
public sealed class Bivector3Tests
{
	#region Initializations
	[Test]
	public void Constructors()
	{
		Bivector3<int> vectorA = new(1, 2, 3);
		Bivector3<double> vectorB = new(1.0, 2.0, 3.0);

		Assert.That(vectorA.YZ, Is.EqualTo(1));
		Assert.That(vectorA.ZX, Is.EqualTo(2));
		Assert.That(vectorA.XY, Is.EqualTo(3));

		Assert.That(vectorB.YZ, Is.EqualTo(1.0));
		Assert.That(vectorB.ZX, Is.EqualTo(2.0));
		Assert.That(vectorB.XY, Is.EqualTo(3.0));
	}

	[Test]
	public void Identities()
	{
		Bivector3<int> addtitiveIdentityA = Bivector3<int>.AdditiveIdentity;
		Bivector3<int> multiplicativeIdentityA = Bivector3<int>.MultiplicativeIdentity;
		Bivector3<int> zeroA = Bivector3<int>.Zero;
		Bivector3<int> oneA = Bivector3<int>.One;
		Bivector3<double> addtitiveIdentityB = Bivector3<double>.AdditiveIdentity;
		Bivector3<double> multiplicativeIdentityB = Bivector3<double>.MultiplicativeIdentity;
		Bivector3<double> zeroB = Bivector3<double>.Zero;
		Bivector3<double> oneB = Bivector3<double>.One;

		Assert.That(addtitiveIdentityA, Is.EqualTo(zeroA));
		Assert.That(multiplicativeIdentityA, Is.EqualTo(oneA));

		Assert.That(zeroA.YZ, Is.EqualTo(0));
		Assert.That(zeroA.ZX, Is.EqualTo(0));
		Assert.That(zeroA.XY, Is.EqualTo(0));

		Assert.That(oneA.YZ, Is.EqualTo(1));
		Assert.That(oneA.ZX, Is.EqualTo(1));
		Assert.That(oneA.XY, Is.EqualTo(1));

		Assert.That(addtitiveIdentityB, Is.EqualTo(zeroB));
		Assert.That(multiplicativeIdentityB, Is.EqualTo(oneB));

		Assert.That(zeroB.YZ, Is.EqualTo(0.0));
		Assert.That(zeroB.ZX, Is.EqualTo(0.0));
		Assert.That(zeroB.XY, Is.EqualTo(0.0));

		Assert.That(oneB.YZ, Is.EqualTo(1.0));
		Assert.That(oneB.ZX, Is.EqualTo(1.0));
		Assert.That(oneB.XY, Is.EqualTo(1.0));
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOpertator_Int()
	{
		bool equal = new Bivector3<int>(1, 2, 3) == new Bivector3<int>(1, 2, 3);
		bool equalyz_isfalse = new Bivector3<int>(1, 2, 3) == new Bivector3<int>(0, 2, 3);
		bool equalzx_isfalse = new Bivector3<int>(1, 2, 3) == new Bivector3<int>(1, 0, 3);
		bool equalxy_isfalse = new Bivector3<int>(1, 2, 3) == new Bivector3<int>(1, 2, 0);

		Assert.That(equal, Is.True);
		Assert.That(equalyz_isfalse, Is.False);
		Assert.That(equalzx_isfalse, Is.False);
		Assert.That(equalxy_isfalse, Is.False);
	}

	[Test]
	public void NotEqualOperator_Int()
	{
		bool equal_isfalse = new Bivector3<int>(1, 2, 3) != new Bivector3<int>(1, 2, 3);
		bool notequalyz = new Bivector3<int>(1, 2, 3) != new Bivector3<int>(0, 2, 3);
		bool notequalzx = new Bivector3<int>(1, 2, 3) != new Bivector3<int>(1, 0, 3);
		bool notequalxy = new Bivector3<int>(1, 2, 3) != new Bivector3<int>(1, 2, 0);

		Assert.That(equal_isfalse, Is.False);
		Assert.That(notequalyz, Is.True);
		Assert.That(notequalzx, Is.True);
		Assert.That(notequalxy, Is.True);
	}

	[Test]
	public void EqualOpertator_Double()
	{
		bool equal = new Bivector3<double>(1.0, 2.0, 3.0) == new Bivector3<double>(1.0, 2.0, 3.0);
		bool equalyz_isfalse = new Bivector3<double>(1.0, 2.0, 3.0) == new Bivector3<double>(0.0, 2.0, 3.0);
		bool equalzx_isfalse = new Bivector3<double>(1.0, 2.0, 3.0) == new Bivector3<double>(1.0, 0.0, 3.0);
		bool equalxy_isfalse = new Bivector3<double>(1.0, 2.0, 3.0) == new Bivector3<double>(1.0, 2.0, 0.0);

		Assert.That(equal, Is.True);
		Assert.That(equalyz_isfalse, Is.False);
		Assert.That(equalzx_isfalse, Is.False);
		Assert.That(equalxy_isfalse, Is.False);
	}

	[Test]
	public void NotEqualOperator_Double()
	{
		bool equal_isfalse = new Bivector3<double>(1.0, 2.0, 3.0) != new Bivector3<double>(1.0, 2.0, 3.0);
		bool notequalyz = new Bivector3<double>(1.0, 2.0, 3.0) != new Bivector3<double>(0.0, 2.0, 3.0);
		bool notequalzx = new Bivector3<double>(1.0, 2.0, 3.0) != new Bivector3<double>(1.0, 0.0, 3.0);
		bool notequalxy = new Bivector3<double>(1.0, 2.0, 3.0) != new Bivector3<double>(1.0, 2.0, 0.0);

		Assert.That(equal_isfalse, Is.False);
		Assert.That(notequalyz, Is.True);
		Assert.That(notequalzx, Is.True);
		Assert.That(notequalxy, Is.True);
	}

	[Test]
	public void NegateOperator()
	{
		Bivector3<int> vectorA = new(1, 2, 3);

		Bivector3<int> expected = new(-1, -2, -3);
		Assert.That(-vectorA, Is.EqualTo(expected));
	}

	[Test]
	public void AddOperator()
	{
		Bivector3<int> vectorA = new(1, 2, 3);
		Bivector3<int> vectorB = new(4, 5, 6);

		Bivector3<int> expected = new(5, 7, 9);
		Assert.That(vectorA + vectorB, Is.EqualTo(expected));
	}

	[Test]
	public void SubtractOperator()
	{
		Bivector3<int> vectorA = new(1, 2, 3);
		Bivector3<int> vectorB = new(4, 5, 6);

		Bivector3<int> expected = new(-3, -3, -3);
		Assert.That(vectorA - vectorB, Is.EqualTo(expected));
	}

	[Test]
	public void ScalarMultiplyOperator()
	{
		Bivector3<int> vectorA = new(3, 4, 5);

		Bivector3<int> expected = new(6, 8, 10);
		Assert.That(vectorA * 2, Is.EqualTo(expected));
		Assert.That(2 * vectorA, Is.EqualTo(expected));
	}

	[Test]
	public void ScalarDivideOperator()
	{
		Bivector3<int> vectorA = new(6, 8, 10);

		Bivector3<int> expected = new(3, 4, 5);
		Assert.That(vectorA / 2, Is.EqualTo(expected));
	}

	[Test]
	public void DivideScalarOperator()
	{
		Bivector3<int> vectorA = new(6, 8, 10);

		Bivector3<int> expected = new(4, 3, 2);
		Assert.That(24 / vectorA, Is.EqualTo(expected));
	}

	[Test]
	public void MultiplyVector3Operator()
	{
		Bivector3<int> a = new(1, 2, 3);
		Vector3<int> b = new(2, 3, 4);

		Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
	}

	[Test]
	public void MultiplyBivector3Operator()
	{
		Bivector3<int> a = new(1, 2, 3);
		Bivector3<int> b = new(4, 5, 6);

		Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
	}

	[Test]
	public void MultiplyTrivector3Operator()
	{
		Bivector3<int> a = new(1, 2, 3);
		Trivector3<int> b = new(4);

		Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
	}

	[Test]
	public void MultiplyRotor3Operator()
	{
		Bivector3<int> a = new(1, 2, 3);
		Rotor3<int> b = new(4, 5, 6, 7);

		Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
	}

	[Test]
	public void MultiplyMultivector3Operator()
	{
		Bivector3<int> a = new(1, 2, 3);
		Multivector3<int> b = new(4, 5, 6, 7, 8, 9, 10, 11);

		Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector()
	{
		Bivector3<int> bivector = new(1, 2, 3);

		Multivector3<int> multivector = bivector.GetMultivector();

		Assert.That(multivector.Scalar, Is.EqualTo(0));
		Assert.That(multivector.Vector.X, Is.EqualTo(0));
		Assert.That(multivector.Vector.Y, Is.EqualTo(0));
		Assert.That(multivector.Vector.Z, Is.EqualTo(0));
		Assert.That(multivector.Bivector.XY, Is.EqualTo(3));
		Assert.That(multivector.Bivector.YZ, Is.EqualTo(1));
		Assert.That(multivector.Bivector.ZX, Is.EqualTo(2));
		Assert.That(multivector.Trivector.XYZ, Is.EqualTo(0));
	}

	[Test]
	public void Dot()
	{
		Bivector3<int> bivectorA = new(1, 2, 3);
		Bivector3<int> bivectorB = new(4, 5, 6);

		int expected = -32;

		Assert.That(bivectorA.Dot(bivectorB), Is.EqualTo(expected));
	}

	[Test]
	public void MagnitudeSquared()
	{
		Bivector3<int> bivector = new(1, 2, 3);

		int expected = 14;

		Assert.That(bivector.MagnitudeSquared(), Is.EqualTo(expected));
	}

	[Test]
	public void Test_Equals()
	{
		Bivector3<int> bivector = new(1, 2, 3);

		Assert.That(bivector.Equals(bivector), Is.True);
		Assert.That(bivector.Equals(new Bivector3<int>(1, 2, 3)), Is.True);
		Assert.That(bivector.Equals(new Bivector3<int>(2, 5, 7)), Is.False);
		Assert.That(bivector.Equals("Test"), Is.False);
	}

	[Test]
	public void Test_GetHashCode()
	{
		Bivector3<int> bivector_1 = new(1, 2, 3);
		Bivector3<int> bivector_2 = new(1, 2, 3);
		Bivector3<int> bivector_3 = new(2, 5, 7);
		Bivector3<int> bivector_4 = new(2, 5, 7);

		Assert.That(bivector_1.GetHashCode(), Is.EqualTo(bivector_2.GetHashCode()));
		Assert.That(bivector_1.GetHashCode(), Is.Not.EqualTo(bivector_3.GetHashCode()));
		Assert.That(bivector_3.GetHashCode(), Is.EqualTo(bivector_4.GetHashCode()));
	}

	[Test]
	public void Test_ToString()
	{
		Bivector3<int> bivector_1 = new(1, 2, 3);
		Bivector3<int> bivector_2 = new(-56, 67, 1200);
		Bivector3<double> bivector_3 = new(-69.4201, 70.8882, 23.233);
		Bivector3<int> allNegative = new(-1, -2, -3);

		Assert.That(bivector_1.ToString(), Is.EqualTo("[1YZ + 2ZX + 3XY]"));
		Assert.That(bivector_2.ToString(), Is.EqualTo("[-56YZ + 67ZX + 1,200XY]"));
		Assert.That(bivector_3.ToString(), Is.EqualTo("[-69.42YZ + 70.888ZX + 23.233XY]"));
		Assert.That(allNegative.ToString(), Is.EqualTo("[-1YZ - 2ZX - 3XY]"));
	}
	#endregion
}
