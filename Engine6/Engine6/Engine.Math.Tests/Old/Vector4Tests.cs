//using Engine.Math.Old;

//namespace Engine.Math.Tests.Old;

//[TestFixture]
//public sealed class Vector4Tests
//{
//    [Test]
//    public void Negation()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);

//        Vector4<double> c = a.Negate();

//        Assert.That(c.X, Is.EqualTo(-1));
//        Assert.That(c.Y, Is.EqualTo(-2));
//        Assert.That(c.Z, Is.EqualTo(-3));
//        Assert.That(c.W, Is.EqualTo(-4));
//    }

//    [Test]
//    public void Addition()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);
//        Vector4<double> b = new(3, 4, 5, 6);

//        Vector4<double> c = a + b;

//        Assert.That(c.X, Is.EqualTo(4));
//        Assert.That(c.Y, Is.EqualTo(6));
//        Assert.That(c.Z, Is.EqualTo(8));
//        Assert.That(c.W, Is.EqualTo(10));
//    }

//    [Test]
//    public void Subtraction()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);
//        Vector4<double> b = new(3, 5, 7, 9);

//        Vector4<double> c = a - b;

//        Assert.That(c.X, Is.EqualTo(-2));
//        Assert.That(c.Y, Is.EqualTo(-3));
//        Assert.That(c.Z, Is.EqualTo(-4));
//        Assert.That(c.W, Is.EqualTo(-5));
//    }

//    [Test]
//    public void Multiplication()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);
//        Vector4<double> b = new(3, 5, 7, 9);

//        Vector4<double> c = a * b;

//        Assert.That(c.X, Is.EqualTo(3));
//        Assert.That(c.Y, Is.EqualTo(10));
//        Assert.That(c.Z, Is.EqualTo(21));
//        Assert.That(c.W, Is.EqualTo(36));
//    }

//    [Test]
//    public void ScalarMultiplication()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);

//        Vector4<double> c = a * 5;

//        Assert.That(c.X, Is.EqualTo(5));
//        Assert.That(c.Y, Is.EqualTo(10));
//        Assert.That(c.Z, Is.EqualTo(15));
//        Assert.That(c.W, Is.EqualTo(20));
//    }

//    [Test]
//    public void Division()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);
//        Vector4<double> b = new(3, 5, 7, 9);

//        Vector4<double> c = a / b;

//        Assert.That(c.X, Is.EqualTo(1.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(2.0 / 5.0).Within(0.0001d));
//        Assert.That(c.Z, Is.EqualTo(3.0 / 7.0).Within(0.0001d));
//        Assert.That(c.W, Is.EqualTo(4.0 / 9.0).Within(0.0001d));
//    }

//    [Test]
//    public void ScalarDivision()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);

//        Vector4<double> c = a / 3;

//        Assert.That(c.X, Is.EqualTo(1.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(2.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Z, Is.EqualTo(3.0 / 3.0).Within(0.0001d));
//        Assert.That(c.W, Is.EqualTo(4.0 / 3.0).Within(0.0001d));
//    }

//    [Test]
//    public void DotProduct()
//    {
//        Vector4<double> a = new(1, 2, 3, 4);
//        Vector4<double> b = new(3, 5, 7, 9);

//        double c = a.Dot(b);

//        Assert.That(c, Is.EqualTo(70));
//    }

//    [Test]
//    public void MagnitudeSquared()
//    {
//        Vector4<double> a = new(3, 4, 5, 6);

//        double c = a.MagnitudeSquared();

//        Assert.That(c, Is.EqualTo(86));
//    }

//    [Test]
//    public void Magnitude()
//    {
//        Vector4<double> a = new(6, 5, 4, 2);

//        double c = a.Magnitude();

//        Assert.That(c, Is.EqualTo(9));
//    }

//    [Test]
//    public void Normalize()
//    {
//        Vector4<double> a = new(6, 5, 4, 2);

//        Vector4<double> c = a.Normalize();

//        Assert.That(c.X, Is.EqualTo(6.0 / 9.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(5.0 / 9.0).Within(0.0001d));
//        Assert.That(c.Z, Is.EqualTo(4.0 / 9.0).Within(0.0001d));
//        Assert.That(c.W, Is.EqualTo(2.0 / 9.0).Within(0.0001d));
//    }

//    [Test]
//    public void MatrixMultiplication()
//    {

//        Vector4<double> a = new(1, 2, 3, 4);
//        Matrix4x4<double> b = new(
//            1, 2, 3, 4,
//            5, 6, 7, 8,
//            9, 10, 11, 12,
//            13, 14, 15, 16);

//        Vector4<double> c = a * b;

//        Assert.That(c.X, Is.EqualTo(90));
//        Assert.That(c.Y, Is.EqualTo(100));
//        Assert.That(c.Z, Is.EqualTo(110));
//        Assert.That(c.W, Is.EqualTo(120));
//    }
//}
