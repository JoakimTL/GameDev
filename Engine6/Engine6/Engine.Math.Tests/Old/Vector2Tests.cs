//using Engine.Math.Old;

//namespace Engine.Math.Tests.Old;

//[TestFixture]
//public class Vector2Tests
//{
//    [Test]
//    public void Negation()
//    {
//        Vector2<double> a = new(1, 2);

//        Vector2<double> c = a.Negate();

//        Assert.That(c.X, Is.EqualTo(-1));
//        Assert.That(c.Y, Is.EqualTo(-2));
//    }

//    [Test]
//    public void Addition()
//    {
//        Vector2<double> a = new(1, 2);
//        Vector2<double> b = new(3, 4);

//        Vector2<double> c = a + b;

//        Assert.That(c.X, Is.EqualTo(4));
//        Assert.That(c.Y, Is.EqualTo(6));
//    }

//    [Test]
//    public void Subtraction()
//    {
//        Vector2<double> a = new(1, 2);
//        Vector2<double> b = new(3, 5);

//        Vector2<double> c = a - b;

//        Assert.That(c.X, Is.EqualTo(-2));
//        Assert.That(c.Y, Is.EqualTo(-3));
//    }

//    [Test]
//    public void Multiplication()
//    {
//        Vector2<double> a = new(1, 2);
//        Vector2<double> b = new(3, 5);

//        Vector2<double> c = a * b;

//        Assert.That(c.X, Is.EqualTo(3));
//        Assert.That(c.Y, Is.EqualTo(10));
//    }

//    [Test]
//    public void ScalarMultiplication()
//    {
//        Vector2<double> a = new(1, 2);

//        Vector2<double> c = a * 5;

//        Assert.That(c.X, Is.EqualTo(5));
//        Assert.That(c.Y, Is.EqualTo(10));
//    }

//    [Test]
//    public void Division()
//    {
//        Vector2<double> a = new(1, 2);
//        Vector2<double> b = new(3, 5);

//        Vector2<double> c = a / b;

//        Assert.That(c.X, Is.EqualTo(1.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(2.0 / 5.0).Within(0.0001d));
//    }

//    [Test]
//    public void ScalarDivision()
//    {
//        Vector2<double> a = new(1, 2);

//        Vector2<double> c = a / 3;

//        Assert.That(c.X, Is.EqualTo(1.0 / 3.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(2.0 / 3.0).Within(0.0001d));
//    }

//    [Test]
//    public void DotProduct()
//    {
//        Vector2<double> a = new(1, 2);
//        Vector2<double> b = new(3, 5);

//        double c = a.Dot(b);

//        Assert.That(c, Is.EqualTo(13));
//    }

//    [Test]
//    public void MagnitudeSquared()
//    {
//        Vector2<double> a = new(3, 4);

//        double c = a.MagnitudeSquared();

//        Assert.That(c, Is.EqualTo(25));
//    }

//    [Test]
//    public void Magnitude()
//    {
//        Vector2<double> a = new(3, 4);

//        double c = a.Magnitude();

//        Assert.That(c, Is.EqualTo(5));
//    }

//    [Test]
//    public void Normalize()
//    {
//        Vector2<double> a = new(3, 4);

//        Vector2<double> c = a.Normalize();

//        Assert.That(c.X, Is.EqualTo(3.0 / 5.0).Within(0.0001d));
//        Assert.That(c.Y, Is.EqualTo(4.0 / 5.0).Within(0.0001d));
//    }

//    [Test]
//    public void WorldTransform()
//    {
//        Vector2<double> a = new(4, 4);
//        Matrix4x4<double> b = new(
//            1, 0, 0, 0,
//            0, 2, 0, 0,
//            0, 0, 3, 0,
//            0, 0, 0, 4
//        );

//        Vector2<double> c = a.WorldTransform(b);

//        Assert.That(c.X, Is.EqualTo(1));
//        Assert.That(c.Y, Is.EqualTo(2));
//    }

//    [Test]
//    public void NormalTransform()
//    {
//        Vector2<double> a = new(1, 2);
//        Matrix4x4<double> b = new(
//            1, 0, 0, 0,
//            0, 2, 0, 0,
//            0, 0, 3, 0,
//            0, 0, 0, 4
//        );

//        Vector2<double> c = a.NormalTransform(b);

//        Assert.That(c.X, Is.EqualTo(1));
//        Assert.That(c.Y, Is.EqualTo(4));
//    }


//}
