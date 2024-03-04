//using Engine.Math.Old;

//namespace Engine.Math.Tests.Old;

//[TestFixture]
//public sealed class Vector3Tests
//{
//    [Test]
//    public void Negation()
//    {
//        Vector3<double> a = new(1, 2, 3);

//        Vector3<double> c = a.Negate();

//        Assert.That(c.X, Is.EqualTo(-1));
//        Assert.That(c.Y, Is.EqualTo(-2));
//        Assert.That(c.Z, Is.EqualTo(-3));
//    }

//    [Test]
//    public void Addition()
//    {
//        Vector3<double> a = new(1, 2, 3);
//        Vector3<double> b = new(3, 4, 5);

//        Vector3<double> c = a + b;

//        Assert.That(c.X, Is.EqualTo(4));
//        Assert.That(c.Y, Is.EqualTo(6));
//        Assert.That(c.Z, Is.EqualTo(8));
//    }

//    [Test]
//    public void Subtraction()
//    {
//        Vector3<double> a = new(1, 2, 3);
//        Vector3<double> b = new(3, 5, 7);

//        Vector3<double> c = a - b;

//        Assert.That(c.X, Is.EqualTo(-2));
//        Assert.That(c.Y, Is.EqualTo(-3));
//        Assert.That(c.Z, Is.EqualTo(-4));
//    }

//    [Test]
//    public void Multiplication()
//    {
//        Vector3<double> a = new(1, 2, 3);
//        Vector3<double> b = new(3, 5, 7);

//        Vector3<double> c = a * b;

//        Assert.That(c.X, Is.EqualTo(3));
//        Assert.That(c.Y, Is.EqualTo(10));
//        Assert.That(c.Z, Is.EqualTo(21));
//    }

//    [Test]
//    public void ScalarMultiplication()
//    {
//        Vector3<double> a = new(1, 2, 3);

//        Vector3<double> c = a * 5;

//        Assert.That(c.X, Is.EqualTo(5));
//        Assert.That(c.Y, Is.EqualTo(10));
//        Assert.That(c.Z, Is.EqualTo(15));
//    }

//    [Test]
//    public void Division()
//    {
//        Vector3<double> a = new(1, 2, 3);
//        Vector3<double> b = new(3, 5, 7);

//        Vector3<double> c = a / b;

//        Assert.That(c.X, Is.EqualTo(1.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(2.0 / 5.0).Within(0.0001d));
//        Assert.That(c.Z, Is.EqualTo(3.0 / 7.0).Within(0.0001d));
//    }

//    [Test]
//    public void ScalarDivision()
//    {
//        Vector3<double> a = new(1, 2, 3);

//        Vector3<double> c = a / 3;

//        Assert.That(c.X, Is.EqualTo(1.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(2.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Z, Is.EqualTo(3.0 / 3.0).Within(0.0001d));
//    }

//    [Test]
//    public void DotProduct()
//    {
//        Vector3<double> a = new(1, 2, 3);
//        Vector3<double> b = new(3, 5, 7);

//        double c = a.Dot(b);

//        Assert.That(c, Is.EqualTo(34));
//    }

//    [Test]
//    public void MagnitudeSquared()
//    {
//        Vector3<double> a = new(3, 4, 5);

//        double c = a.MagnitudeSquared();

//        Assert.That(c, Is.EqualTo(50));
//    }

//    [Test]
//    public void Magnitude()
//    {
//        Vector3<double> a = new(2, 3, 6);

//        double c = a.Magnitude();

//        Assert.That(c, Is.EqualTo(7));
//    }

//    [Test]
//    public void Normalize()
//    {
//        Vector3<double> a = new(2, 3, 6);

//        Vector3<double> c = a.Normalize();

//        Assert.That(c.X, Is.EqualTo(2.0 / 7.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(3.0 / 7.0).Within(0.0001d));
//        Assert.That(c.Z, Is.EqualTo(6.0 / 7.0).Within(0.0001d));
//    }

//    [Test]
//    public void WorldTransform()
//    {
//        Vector3<double> a = new(4, 4, 4);
//        Matrix4x4<double> b = new(
//            1, 0, 0, 0,
//            0, 2, 0, 0,
//            0, 0, 3, 0,
//            0, 0, 0, 4
//        );

//        Vector3<double> c = a.WorldTransform(b);

//        Assert.That(c.X, Is.EqualTo(1));
//        Assert.That(c.Y, Is.EqualTo(2));
//        Assert.That(c.Z, Is.EqualTo(3));
//    }

//    [Test]
//    public void NormalTransform()
//    {
//        Vector3<double> a = new(1, 2, 3);
//        Matrix4x4<double> b = new(
//            1, 0, 0, 0,
//            0, 2, 0, 0,
//            0, 0, 3, 0,
//            0, 0, 0, 4
//        );

//        Vector3<double> c = a.NormalTransform(b);

//        Assert.That(c.X, Is.EqualTo(1));
//        Assert.That(c.Y, Is.EqualTo(4));
//        Assert.That(c.Z, Is.EqualTo(9));
//    }
//}
