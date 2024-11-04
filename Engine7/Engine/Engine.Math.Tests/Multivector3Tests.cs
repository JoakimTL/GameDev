namespace Engine.Math.Tests;

[TestFixture]
public sealed class Multivector3Tests
{
    #region Initializations
    [Test]
    public void Constructors()
    {
        Multivector3<int> vectorA = new(1, new(2, 3, 4), new(5, 6, 7), new(8));
        Multivector3<double> vectorB = new(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0));
        Multivector3<int> vectorC = new(1, 2, 3, 4, 5, 6, 7, 8);

        Assert.That(vectorA.Scalar, Is.EqualTo(1));
        Assert.That(vectorA.Vector.X, Is.EqualTo(2));
        Assert.That(vectorA.Vector.Y, Is.EqualTo(3));
        Assert.That(vectorA.Vector.Z, Is.EqualTo(4));
        Assert.That(vectorA.Bivector.YZ, Is.EqualTo(5));
        Assert.That(vectorA.Bivector.ZX, Is.EqualTo(6));
        Assert.That(vectorA.Bivector.XY, Is.EqualTo(7));
        Assert.That(vectorA.Trivector.XYZ, Is.EqualTo(8));

        Assert.That(vectorB.Scalar, Is.EqualTo(1.0));
        Assert.That(vectorB.Vector.X, Is.EqualTo(2.0));
        Assert.That(vectorB.Vector.Y, Is.EqualTo(3.0));
        Assert.That(vectorB.Vector.Z, Is.EqualTo(4.0));
        Assert.That(vectorB.Bivector.YZ, Is.EqualTo(5.0));
        Assert.That(vectorB.Bivector.ZX, Is.EqualTo(6.0));
        Assert.That(vectorB.Bivector.XY, Is.EqualTo(7.0));
        Assert.That(vectorB.Trivector.XYZ, Is.EqualTo(8.0));

        Assert.That(vectorC.Scalar, Is.EqualTo(1));
        Assert.That(vectorC.Vector.X, Is.EqualTo(2));
        Assert.That(vectorC.Vector.Y, Is.EqualTo(3));
        Assert.That(vectorC.Vector.Z, Is.EqualTo(4));
        Assert.That(vectorC.Bivector.YZ, Is.EqualTo(5));
        Assert.That(vectorC.Bivector.ZX, Is.EqualTo(6));
        Assert.That(vectorC.Bivector.XY, Is.EqualTo(7));
        Assert.That(vectorC.Trivector.XYZ, Is.EqualTo(8));
    }

    [Test]
    public void Identities()
    {
        Multivector3<int> addtitiveIdentityA = Multivector3<int>.AdditiveIdentity;
        Multivector3<int> multiplicativeIdentityA = Multivector3<int>.MultiplicativeIdentity;
        Multivector3<int> zeroA = Multivector3<int>.Zero;
        Multivector3<int> oneA = Multivector3<int>.One;
        Multivector3<double> addtitiveIdentityB = Multivector3<double>.AdditiveIdentity;
        Multivector3<double> multiplicativeIdentityB = Multivector3<double>.MultiplicativeIdentity;
        Multivector3<double> zeroB = Multivector3<double>.Zero;
        Multivector3<double> oneB = Multivector3<double>.One;

        Assert.That(addtitiveIdentityA, Is.EqualTo(zeroA));
        Assert.That(multiplicativeIdentityA, Is.EqualTo(oneA));

        Assert.That(zeroA.Scalar, Is.EqualTo(0));
        Assert.That(zeroA.Vector.X, Is.EqualTo(0));
        Assert.That(zeroA.Vector.Y, Is.EqualTo(0));
        Assert.That(zeroA.Vector.Z, Is.EqualTo(0));
        Assert.That(zeroA.Bivector.YZ, Is.EqualTo(0));
        Assert.That(zeroA.Bivector.ZX, Is.EqualTo(0));
        Assert.That(zeroA.Bivector.XY, Is.EqualTo(0));
        Assert.That(zeroA.Trivector.XYZ, Is.EqualTo(0));

        Assert.That(oneA.Scalar, Is.EqualTo(1));
        Assert.That(oneA.Vector.X, Is.EqualTo(1));
        Assert.That(oneA.Vector.Y, Is.EqualTo(1));
        Assert.That(oneA.Vector.Z, Is.EqualTo(1));
        Assert.That(oneA.Bivector.YZ, Is.EqualTo(1));
        Assert.That(oneA.Bivector.ZX, Is.EqualTo(1));
        Assert.That(oneA.Bivector.XY, Is.EqualTo(1));
        Assert.That(oneA.Trivector.XYZ, Is.EqualTo(1));

        Assert.That(addtitiveIdentityB, Is.EqualTo(zeroB));
        Assert.That(multiplicativeIdentityB, Is.EqualTo(oneB));

        Assert.That(zeroB.Scalar, Is.EqualTo(0.0));
        Assert.That(zeroB.Vector.X, Is.EqualTo(0.0));
        Assert.That(zeroB.Vector.Y, Is.EqualTo(0.0));
        Assert.That(zeroB.Vector.Z, Is.EqualTo(0.0));
        Assert.That(zeroB.Bivector.YZ, Is.EqualTo(0.0));
        Assert.That(zeroB.Bivector.ZX, Is.EqualTo(0.0));
        Assert.That(zeroB.Bivector.XY, Is.EqualTo(0.0));
        Assert.That(zeroB.Trivector.XYZ, Is.EqualTo(0.0));

        Assert.That(oneB.Scalar, Is.EqualTo(1.0));
        Assert.That(oneB.Vector.X, Is.EqualTo(1.0));
        Assert.That(oneB.Vector.Y, Is.EqualTo(1.0));
        Assert.That(oneB.Vector.Z, Is.EqualTo(1.0));
        Assert.That(oneB.Bivector.YZ, Is.EqualTo(1.0));
        Assert.That(oneB.Bivector.ZX, Is.EqualTo(1.0));
        Assert.That(oneB.Bivector.XY, Is.EqualTo(1.0));
        Assert.That(oneB.Trivector.XYZ, Is.EqualTo(1.0));
    }
    #endregion

    #region Operators
    [Test]
    public void EqualOpertator_Int()
    {
        bool equal = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) == new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8));
        bool equalscalar_isfalse = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) == new Multivector3<int>(0, new(2, 3, 4), new(5, 6, 7), new(8));
        bool equalvector_isfalse = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) == new Multivector3<int>(1, new(0, 0, 0), new(5, 6, 7), new(8));
        bool equalbivector_isfalse = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) == new Multivector3<int>(1, new(2, 3, 4), new(0, 0, 0), new(8));
        bool equaltrivector_isfalse = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) == new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(0));

        Assert.That(equal, Is.True);
        Assert.That(equalscalar_isfalse, Is.False);
        Assert.That(equalvector_isfalse, Is.False);
        Assert.That(equalbivector_isfalse, Is.False);
        Assert.That(equaltrivector_isfalse, Is.False);
    }

    [Test]
    public void NotEqualOperator_Int()
    {
        bool equal_isfalse = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) != new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8));
        bool notequalscalar = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) != new Multivector3<int>(0, new(2, 3, 4), new(5, 6, 7), new(8));
        bool notequalvector = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) != new Multivector3<int>(1, new(0, 0, 0), new(5, 6, 7), new(8));
        bool notequalbivector = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) != new Multivector3<int>(1, new(2, 3, 4), new(0, 0, 0), new(8));
        bool notequaltrivector = new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(8)) != new Multivector3<int>(1, new(2, 3, 4), new(5, 6, 7), new(0));

        Assert.That(equal_isfalse, Is.False);
        Assert.That(notequalscalar, Is.True);
        Assert.That(notequalvector, Is.True);
        Assert.That(notequalbivector, Is.True);
        Assert.That(notequaltrivector, Is.True);
    }

    [Test]
    public void EqualOperator_Double()
    {
        bool equal = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) == new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0));
        bool equalscalar_isfalse = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) == new Multivector3<double>(0.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0));
        bool equalvector_isfalse = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) == new Multivector3<double>(1.0, new(0.0, 0.0, 0.0), new(5.0, 6.0, 7.0), new(8.0));
        bool equalbivector_isfalse = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) == new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(0.0, 0.0, 0.0), new(8.0));
        bool equaltrivector_isfalse = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) == new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(0.0));

        Assert.That(equal, Is.True);
        Assert.That(equalscalar_isfalse, Is.False);
        Assert.That(equalvector_isfalse, Is.False);
        Assert.That(equalbivector_isfalse, Is.False);
        Assert.That(equaltrivector_isfalse, Is.False);
    }

    [Test]
    public void NotEqualOperator_Double()
    {
        bool equal_isfalse = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) != new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0));
        bool notequalscalar = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) != new Multivector3<double>(0.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0));
        bool notequalvector = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) != new Multivector3<double>(1.0, new(0.0, 0.0, 0.0), new(5.0, 6.0, 7.0), new(8.0));
        bool notequalbivector = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) != new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(0.0, 0.0, 0.0), new(8.0));
        bool notequaltrivector = new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(8.0)) != new Multivector3<double>(1.0, new(2.0, 3.0, 4.0), new(5.0, 6.0, 7.0), new(0.0));

        Assert.That(equal_isfalse, Is.False);
        Assert.That(notequalscalar, Is.True);
        Assert.That(notequalvector, Is.True);
        Assert.That(notequalbivector, Is.True);
        Assert.That(notequaltrivector, Is.True);
    }

    [Test]
    public void NegateOperator()
    {
        Multivector3<int> vectorA = new(1, new(2, 3, 4), new(5, 6, 7), new(8));

        Multivector3<int> expected = new(-1, new(-2, -3, -4), new(-5, -6, -7), new(-8));
        Assert.That(-vectorA, Is.EqualTo(expected));
    }

    [Test]
    public void AddOperator()
    {
        Multivector3<int> vectorA = new(1, new(2, 3, 4), new(5, 6, 7), new(8));
        Multivector3<int> vectorB = new(9, new(10, 11, 12), new(13, 14, 15), new(16));

        Multivector3<int> expected = new(10, new(12, 14, 16), new(18, 20, 22), new(24));
        Assert.That(vectorA + vectorB, Is.EqualTo(expected));
    }

    [Test]
    public void SubtractOperator()
    {
        Multivector3<int> vectorA = new(1, new(2, 3, 4), new(5, 6, 7), new(8));
        Multivector3<int> vectorB = new(9, new(10, 11, 12), new(13, 14, 15), new(16));

        Multivector3<int> expected = new(-8, new(-8, -8, -8), new(-8, -8, -8), new(-8));
        Assert.That(vectorA - vectorB, Is.EqualTo(expected));
    }

    [Test]
    public void ScalarMultiplyOperator()
    {
        Multivector3<int> vectorA = new(3, new(4, 5, 6), new(7, 8, 9), new(10));

        Multivector3<int> expected = new(6, new(8, 10, 12), new(14, 16, 18), new(20));
        Assert.That(vectorA * 2, Is.EqualTo(expected));
        Assert.That(2 * vectorA, Is.EqualTo(expected));
    }

    [Test]
    public void ScalarDivideOperator()
    {
        Multivector3<int> vectorA = new(6, new(8, 10, 12), new(14, 16, 18), new(20));

        Multivector3<int> expected = new(3, new(4, 5, 6), new(7, 8, 9), new(10));
        Assert.That(vectorA / 2, Is.EqualTo(expected));
    }

    [Test]
    public void DivideScalarOperator()
    {
        Multivector3<int> vectorA = new(6, new(8, 10, 12), new(14, 16, 18), new(20));

        Multivector3<int> expected = new(21, new(16, 12, 10), new(9, 8, 7), new(6));
        Assert.That(128 / vectorA, Is.EqualTo(expected));
    }

    [Test]
    public void MultiplyVector3Operator()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);
        Vector3<int> b = new(9, 10, 11);

        Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
    }

    [Test]
    public void MultiplyBivector3Operator()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);
        Bivector3<int> b = new(9, 10, 11);

        Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
    }

    [Test]
    public void MultiplyTrivector3Operator()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);
        Trivector3<int> b = new(9);

        Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
    }

    [Test]
    public void MultiplyRotor3Operator()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);
        Rotor3<int> b = new(9, 10, 11, 12);

        Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
    }

    [Test]
    public void MultiplyMultivector3Operator()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);
        Multivector3<int> b = new(9, 10, 11, 12, 13, 14, 15, 16);

        Assert.That(a * b, Is.EqualTo(GeometricAlgebraMath3.Multiply(a, b)));
    }
    #endregion

    #region Methods
    [Test]
    public void GetMultivector()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        Multivector3<int> multivector = a.GetMultivector();

        Assert.That(multivector.Scalar, Is.EqualTo(1));
        Assert.That(multivector.Vector.X, Is.EqualTo(2));
        Assert.That(multivector.Vector.Y, Is.EqualTo(3));
        Assert.That(multivector.Vector.Z, Is.EqualTo(4));
        Assert.That(multivector.Bivector.YZ, Is.EqualTo(5));
        Assert.That(multivector.Bivector.ZX, Is.EqualTo(6));
        Assert.That(multivector.Bivector.XY, Is.EqualTo(7));
        Assert.That(multivector.Trivector.XYZ, Is.EqualTo(8));
    }

    [Test]
    public void Dot()
    {
        Multivector3<int> vectorA = new(1, 2, 3, 4, 5, 6, 7, 8);
        Multivector3<int> vectorB = new(9, 10, 11, 12, 13, 14, 15, 16);

        int dot = vectorA.Dot(vectorB);

        Assert.That(dot, Is.EqualTo(-272));
    }

    [Test]
    public void MagnitudeSquared()
    {
        Multivector3<int> vectorA = new(1, 2, 3, 4, 5, 6, 7, 8);

        int magnitude = vectorA.MagnitudeSquared();

        Assert.That(magnitude, Is.EqualTo(204));
    }

    [Test]
    public void Test_Equals()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        Assert.That(a.Equals(a), Is.True);
        Assert.That(a.Equals(new Multivector3<int>(1, 2, 3, 4, 5, 6, 7, 8)), Is.True);
        Assert.That(a.Equals(new Multivector3<int>(2, 5, 6, 12, -6, 12, 15, 66)), Is.False);
        Assert.That(a.Equals("Test"), Is.False);
    }

    [Test]
    public void Test_GetHashCode()
    {
        Multivector3<int> multivector_1 = new(1, 2, 3, 4, 5, 6, 7, 8);
        Multivector3<int> multivector_2 = new(1, 2, 3, 4, 5, 6, 7, 8);
        Multivector3<int> multivector_3 = new(2, 5, 6, 10, -5, 12, 14, 16);
        Multivector3<int> multivector_4 = new(2, 5, 6, 10, -5, 12, 14, 16);

        Assert.That(multivector_1.GetHashCode(), Is.EqualTo(multivector_2.GetHashCode()));
        Assert.That(multivector_1.GetHashCode(), Is.Not.EqualTo(multivector_3.GetHashCode()));
        Assert.That(multivector_3.GetHashCode(), Is.EqualTo(multivector_4.GetHashCode()));
    }

    [Test]
    public void Test_ToString()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);
        Multivector3<double> b = new(-69.4201, 70.5, 50.9999, 23.232, 10, 12, 1300, 66.53412);


        Assert.That(a.ToString(), Is.EqualTo($"<1 + {a.Vector} + {a.Bivector} + {a.Trivector}>"));
        Assert.That(b.ToString(), Is.EqualTo($"<-69.42 + {b.Vector} + {b.Bivector} + {b.Trivector}>"));
    }
    #endregion

    #region Casts
    [Test]
    public void CastToScalar()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        int result = (int)a;

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void CastToVector()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        Vector3<int> result = (Vector3<int>)a;

        Assert.That(result.X, Is.EqualTo(2));
        Assert.That(result.Y, Is.EqualTo(3));
        Assert.That(result.Z, Is.EqualTo(4));
    }

    [Test]
    public void CastToBivector()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        Bivector3<int> result = (Bivector3<int>)a;

        Assert.That(result.YZ, Is.EqualTo(5));
        Assert.That(result.ZX, Is.EqualTo(6));
        Assert.That(result.XY, Is.EqualTo(7));
    }

    [Test]
    public void CastToTrivector()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        Trivector3<int> result = (Trivector3<int>)a;

        Assert.That(result.XYZ, Is.EqualTo(8));
    }

    [Test]
    public void CastToRotor()
    {
        Multivector3<int> a = new(1, 2, 3, 4, 5, 6, 7, 8);

        Rotor3<int> result = (Rotor3<int>)a;

        Assert.That(result.Scalar, Is.EqualTo(1));
        Assert.That(result.Bivector.YZ, Is.EqualTo(5));
        Assert.That(result.Bivector.ZX, Is.EqualTo(6));
        Assert.That(result.Bivector.XY, Is.EqualTo(7));
    }
    #endregion
}
