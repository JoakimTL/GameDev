namespace Math.GeometricAlgebra.Tests;

[TestFixture]
public sealed class Matrix2x2FactoryTests
{
	[Test]
	public void CreateScaling()
	{
		float x = 3;
		float y = 5;

		Matrix2x2<float> resultComponents = Matrix.Create2x2.Scaling(x, y);
		Matrix2x2<float> resultVector = Matrix.Create2x2.Scaling(new Vector2<float>(x, y));

		Matrix2x2<float> expected = new(
			x, 0,
			0, y
		);

		Assert.That(resultComponents, Is.EqualTo(expected));
		Assert.That(resultVector, Is.EqualTo(expected));
	}

	[Test]
	public void CreateRotation()
	{
		float angle = 90F / 180 * MathF.PI;

		Matrix2x2<float> result = Matrix.Create2x2.Rotation(angle);

		Matrix2x2<float> expected = new(
			MathF.Cos(angle), -MathF.Sin(angle),
			MathF.Sin(angle), MathF.Cos(angle)
		);

		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public void CreateRotationFromRotor()
	{
		float angle = 90F / 180 * MathF.PI;
		Rotor2<float> rotor = Rotor2.FromVectors<float>(new(1, 0), new(0, -1));

		Matrix2x2<float> result = Matrix.Create2x2.RotationFromRotor(rotor);

		Matrix2x2<float> expected = new(
			MathF.Cos(angle), -MathF.Sin(angle),
			MathF.Sin(angle), MathF.Cos(angle)
		);

		Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.00001f));
		Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.00001f));
		Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.00001f));
		Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.00001f));
	}

	[Test]
	public void Basis()
	{
		Vector2<double> xBasis = new(0, 1);
		Vector2<double> yBasis = new(1, 0);

		Matrix2x2<double> result = Matrix.Create2x2.Basis(xBasis, yBasis);

		Matrix2x2<double> expected = new(
			0, 1,
			1, 0
		);

		Vector2<double> vectorInBasis = new(2, 3);
		Vector2<double> resultVector = result * vectorInBasis;

		Assert.That(result, Is.EqualTo(expected));
		Assert.That(resultVector, Is.EqualTo(new Vector2<double>(3, 2)));
	}
}
