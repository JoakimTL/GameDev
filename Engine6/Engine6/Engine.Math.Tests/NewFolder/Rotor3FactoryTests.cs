using Engine.Math.NewFolder;

namespace Engine.Math.Tests.NewFolder;

[TestFixture]
public sealed class Rotor3FactoryTests
{
    [Test]
    public void FromAxisAngle()
    {
        float angle = MathF.PI / 2;
        Vector3<float> axis = new(0, 0, 1);

        Rotor3<float> result = Rotor3Factory.FromAxisAngle(axis, angle);

        Rotor3<float> expected = new(MathF.Cos(angle / 2), new Bivector3<float>(0, 0, -MathF.Sin(angle / 2)));

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FromVectors()
    {
        Vector3<float> a = new(1, 0, 0);
        Vector3<float> b = new(0, 1, 0);

        Rotor3<float> rotor = Rotor3Factory.FromVectors(a, b);

        Rotor3<float> expected = new(MathF.Cos(MathF.PI / 4), -new Bivector3<float>(0, 0, MathF.Sin(MathF.PI / 4)));

        Assert.That(rotor, Is.EqualTo(expected));
    }

    [Test]
    public void FromVectorsZeroVector()
    {
        Vector3<double> a = new(0, 1, 0);
        Vector3<double> b = new(0, 0, 0);

        Assert.Throws<ArgumentException>(() => Rotor3Factory.FromVectors(a, b));
    }

    [Test]
    public void FromVectors180()
    {
        Vector3<double> a = new(0, 1, 0);
        Vector3<double> b = new(0, -1, 0);

        Assert.Throws<ArgumentException>(() => Rotor3Factory.FromVectors(a, b));
    }

    [Test]
    public void FromVectors0()
    {
        Vector3<double> a = new(0, 1, 0);
        Vector3<double> b = new(0, 1, 0);


        Rotor3<double> rotor = Rotor3Factory.FromVectors(a, b);

        Rotor3<double> expected = new(MathF.Cos(0), -new Bivector3<double>(0, 0, MathF.Sin(0)));

        Assert.That(rotor, Is.EqualTo(expected));
    }
}