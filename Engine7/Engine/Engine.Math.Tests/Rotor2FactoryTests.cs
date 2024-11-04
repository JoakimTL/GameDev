namespace Engine.Math.Tests;

[TestFixture]
public sealed class Rotor2FactoryTests
{
    [Test]
    public void FromAngle()
    {
        float angle = MathF.PI / 2;

        Rotor2<float> result = Rotor2.FromAngle(angle);

        Rotor2<float> expected = new(MathF.Cos(angle / 2), new Bivector2<float>(-MathF.Sin(angle / 2)));

        Assert.That(result.Scalar, Is.EqualTo(expected.Scalar).Within(0.00001f));
        Assert.That(result.Bivector.XY, Is.EqualTo(expected.Bivector.XY).Within(0.00001f));
    }

    [Test]
    public void FromVectors()
    {
        Vector2<float> a = new(1, 0);
        Vector2<float> b = new(0, 1);

        Rotor2<float> rotor = Rotor2.FromVectors(a, b);

        Rotor2<float> expected = new(MathF.Cos(MathF.PI / 4), new Bivector2<float>(-MathF.Sin(MathF.PI / 4)));

        Assert.That(rotor.Scalar, Is.EqualTo(expected.Scalar).Within(0.00001f));
        Assert.That(rotor.Bivector.XY, Is.EqualTo(expected.Bivector.XY).Within(0.00001f));
    }

    [Test]
    public void FromVectors_Fails_ZeroFromVector()
    {
        Assert.Throws<ArgumentException>(() => Rotor2.FromVectors(new Vector2<float>(0, 0), new Vector2<float>(1, 0)));
    }

    [Test]
    public void FromVectors_Fails_ZeroToVector()
    {
        Assert.Throws<ArgumentException>(() => Rotor2.FromVectors(new Vector2<float>(0, 1), new Vector2<float>(0, 0)));
    }

    [Test]
    public void FromVectorsSameToAndFromVector()
    {
        Vector2<float> a = new(1, 0);
        Vector2<float> b = new(1, 0);

        Rotor2<float> result = Rotor2.FromVectors(a, b);
        Rotor2<float> expected = new(MathF.Cos(0), -new Bivector2<float>(MathF.Sin(0)));

        Assert.That(result.Scalar, Is.EqualTo(expected.Scalar).Within(0.00001f));
        Assert.That(result.Bivector.XY, Is.EqualTo(expected.Bivector.XY).Within(0.00001f));
    }

    [Test]
    public void FromVectorsOppositeToAndFromVector()
    {
        Vector2<float> a = new(1, 0);
        Vector2<float> b = new(-1, 0);

        Rotor2<float> result = Rotor2.FromVectors(a, b);
        Rotor2<float> expected = new(MathF.Cos(MathF.PI / 2), -new Bivector2<float>(MathF.Sin(MathF.PI / 2)));

        Assert.That(result.Scalar, Is.EqualTo(expected.Scalar).Within(0.00001f));
        Assert.That(result.Bivector.XY, Is.EqualTo(expected.Bivector.XY).Within(0.00001f));
    }

    [Test]
    public void TryFromVectors()
    {
        Vector2<float> a = new(1, 0);
        Vector2<float> b = new(0, 1);

        bool success = Rotor2.TryFromVectors(a, b, out Rotor2<float> result);

        Rotor2<float> expected = new(MathF.Cos(MathF.PI / 4), new Bivector2<float>(-MathF.Sin(MathF.PI / 4)));

        Assert.That(success, Is.True);
        Assert.That(result.Scalar, Is.EqualTo(expected.Scalar).Within(0.00001f));
        Assert.That(result.Bivector.XY, Is.EqualTo(expected.Bivector.XY).Within(0.00001f));
    }

    [Test]
    public void TryFromVectors_Fails_ZeroFromVector()
    {
        bool success = Rotor2.TryFromVectors(new Vector2<float>(0, 0), new Vector2<float>(1, 0), out Rotor2<float> result);

        Assert.That(success, Is.False);
        Assert.That(result, Is.EqualTo(Rotor2<float>.MultiplicativeIdentity));
    }

    [Test]
    public void TryFromVectors_Fails_ZeroToVector()
    {
        bool success = Rotor2.TryFromVectors(new Vector2<float>(0, 1), new Vector2<float>(0, 0), out Rotor2<float> result);

        Assert.That(success, Is.False);
        Assert.That(result, Is.EqualTo(Rotor2<float>.MultiplicativeIdentity));
    }
}
