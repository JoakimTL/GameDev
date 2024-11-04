namespace Engine.Math.Tests;

[TestFixture]
public sealed class Matrix4x4FactoryTests
{
    [Test]
    public void CreatePerspective()
    {
        float fov = 90F / 180 * MathF.PI;
        float aspect = 1;
        float near = 0.1F;
        float far = 100F;
        float top = near / MathF.Tan(fov / 2);
        float bottom = -top;
        float right = top * aspect;
        float left = -right;

        Matrix4x4<float> resultFoV = Matrix.Create4x4.PerspectiveFieldOfView(fov, aspect, near, far);
        Matrix4x4<float> resultFoVLh = Matrix.Create4x4.PerspectiveFieldOfView(fov, aspect, near, far, true);
        Matrix4x4<float> resultStd = Matrix.Create4x4.Perspective(left, top, right, bottom, near, far);
        Matrix4x4<float> resultStdLh = Matrix.Create4x4.Perspective(left, top, right, bottom, near, far, true);

        Matrix4x4<float> expected = new(
            2f * near / (right - left), 0, (right + left) / (right - left), 0,
            0, 2 * near / (top - bottom), (top + bottom) / (top - bottom), 0,
            0, 0, -(far + near) / (far - near), -2 * far * near / (far - near),
            0, 0, -1, 0
        );

        Matrix4x4<float> expectedLh = new(
            2f * near / (right - left), 0, (right + left) / (right - left), 0,
            0, 2 * near / (top - bottom), (top + bottom) / (top - bottom), 0,
            0, 0, -(far + near) / (far - near), 2 * far * near / (far - near),
            0, 0, 1, 0
        );

        Assert.That(resultFoV, Is.EqualTo(expected));
        Assert.That(resultStd, Is.EqualTo(expected));
        Assert.That(resultFoVLh, Is.EqualTo(expectedLh));
        Assert.That(resultStdLh, Is.EqualTo(expectedLh));

        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.PerspectiveFieldOfView(0, aspect, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.PerspectiveFieldOfView(MathF.PI, aspect, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.PerspectiveFieldOfView(fov, 0, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.Perspective(right, top, left, bottom, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.Perspective(left, bottom, right, top, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.Perspective(left, top, right, bottom, far, near));
    }

    [Test]
    public void CreateOrthographic()
    {
        float left = -1;
        float top = 1;
        float right = 1;
        float bottom = -1;
        float near = 0.1F;
        float far = 100F;

        Matrix4x4<float> result = Matrix.Create4x4.Orthographic(left, top, right, bottom, near, far);
        Matrix4x4<float> resultLh = Matrix.Create4x4.Orthographic(left, top, right, bottom, near, far, true);

        Matrix4x4<float> expected = new(
            2f / (right - left), 0, 0, -(right + left) / (right - left),
            0, 2f / (top - bottom), 0, -(top + bottom) / (top - bottom),
            0, 0, 2f / (far - near), -(far + near) / (far - near),
            0, 0, 0, 1
        );
        Matrix4x4<float> expectedLh = new(
            2f / (right - left), 0, 0, -(right + left) / (right - left),
            0, 2f / (top - bottom), 0, -(top + bottom) / (top - bottom),
            0, 0, -2f / (far - near), -(far + near) / (far - near),
            0, 0, 0, 1
        );

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(resultLh, Is.EqualTo(expectedLh));

        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.Orthographic(right, top, left, bottom, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.Orthographic(left, bottom, right, top, near, far));
        Assert.Throws<ArgumentException>(() => Matrix.Create4x4.Orthographic(left, top, right, bottom, far, near));
    }

    [Test]
    public void CreateScaling_2d()
    {
        float x = 3;
        float y = 5;

        Matrix4x4<float> resultComponents = Matrix.Create4x4.Scaling(x, y);
        Matrix4x4<float> resultVector = Matrix.Create4x4.Scaling(new Vector2<float>(x, y));

        Matrix4x4<float> expected = new(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        Assert.That(resultComponents, Is.EqualTo(expected));
        Assert.That(resultVector, Is.EqualTo(expected));
    }

    [Test]
    public void CreateScaling_3d()
    {
        float x = 3;
        float y = 5;
        float z = 7;

        Matrix4x4<float> resultComponents = Matrix.Create4x4.Scaling(x, y, z);
        Matrix4x4<float> resultVector = Matrix.Create4x4.Scaling(new Vector3<float>(x, y, z));

        Matrix4x4<float> expected = new(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1
        );

        Assert.That(resultComponents, Is.EqualTo(expected));
        Assert.That(resultVector, Is.EqualTo(expected));
    }

    [Test]
    public void CreateRotationX()
    {
        float angle = 90F / 180 * MathF.PI;

        Matrix4x4<float> result = Matrix.Create4x4.RotationX(angle);

        Matrix4x4<float> expected = new(
            1, 0, 0, 0,
            0, MathF.Cos(angle), -MathF.Sin(angle), 0,
            0, MathF.Sin(angle), MathF.Cos(angle), 0,
            0, 0, 0, 1
        );

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CreateRotationY()
    {
        float angle = 90F / 180 * MathF.PI;

        Matrix4x4<float> result = Matrix.Create4x4.RotationY(angle);

        Matrix4x4<float> expected = new(
            MathF.Cos(angle), 0, MathF.Sin(angle), 0,
            0, 1, 0, 0,
            -MathF.Sin(angle), 0, MathF.Cos(angle), 0,
            0, 0, 0, 1
        );

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CreateRotationZ()
    {
        float angle = 90F / 180 * MathF.PI;

        Matrix4x4<float> result = Matrix.Create4x4.RotationZ(angle);

        Matrix4x4<float> expected = new(
            MathF.Cos(angle), -MathF.Sin(angle), 0, 0,
            MathF.Sin(angle), MathF.Cos(angle), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CreateTranslation_2d()
    {
        float x = 3;
        float y = 5;

        Matrix4x4<float> resultComponents = Matrix.Create4x4.Translation(x, y);
        Matrix4x4<float> resultVector = Matrix.Create4x4.Translation(new Vector2<float>(x, y));

        Matrix4x4<float> expected = new(
            1, 0, 0, x,
            0, 1, 0, y,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        Assert.That(resultComponents, Is.EqualTo(expected));
        Assert.That(resultVector, Is.EqualTo(expected));
    }

    [Test]
    public void CreateTranslation_3d()
    {
        float x = 3;
        float y = 5;
        float z = 7;

        Matrix4x4<float> resultComponents = Matrix.Create4x4.Translation(x, y, z);
        Matrix4x4<float> resultVector = Matrix.Create4x4.Translation(new Vector3<float>(x, y, z));

        Matrix4x4<float> expected = new(
            1, 0, 0, x,
            0, 1, 0, y,
            0, 0, 1, z,
            0, 0, 0, 1
        );

        Assert.That(resultComponents, Is.EqualTo(expected));
        Assert.That(resultVector, Is.EqualTo(expected));
    }

    [Test]
    public void RotationAroundAxisX()
    {
        float angle = 90F / 180 * MathF.PI;
        Vector3<float> axis = new(1, 0, 0);

        Matrix4x4<float> result = Matrix.Create4x4.RotationAroundAxis(axis, angle);

        Matrix4x4<float> expected = new(
            1, 0, 0, 0,
            0, MathF.Cos(angle), -MathF.Sin(angle), 0,
            0, MathF.Sin(angle), MathF.Cos(angle), 0,
            0, 0, 0, 1
        );

        Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.00001f));
        Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.00001f));
        Assert.That(result.M02, Is.EqualTo(expected.M02).Within(0.00001f));
        Assert.That(result.M03, Is.EqualTo(expected.M03).Within(0.00001f));
        Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.00001f));
        Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.00001f));
        Assert.That(result.M12, Is.EqualTo(expected.M12).Within(0.00001f));
        Assert.That(result.M13, Is.EqualTo(expected.M13).Within(0.00001f));
        Assert.That(result.M20, Is.EqualTo(expected.M20).Within(0.00001f));
        Assert.That(result.M21, Is.EqualTo(expected.M21).Within(0.00001f));
        Assert.That(result.M22, Is.EqualTo(expected.M22).Within(0.00001f));
        Assert.That(result.M23, Is.EqualTo(expected.M23).Within(0.00001f));
        Assert.That(result.M30, Is.EqualTo(expected.M30).Within(0.00001f));
        Assert.That(result.M31, Is.EqualTo(expected.M31).Within(0.00001f));
        Assert.That(result.M32, Is.EqualTo(expected.M32).Within(0.00001f));
        Assert.That(result.M33, Is.EqualTo(expected.M33).Within(0.00001f));
    }

    [Test]
    public void RotationAroundAxisY()
    {
        float angle = 90F / 180 * MathF.PI;
        Vector3<float> axis = new(0, 1, 0);

        Matrix4x4<float> result = Matrix.Create4x4.RotationAroundAxis(axis, angle);

        Matrix4x4<float> expected = new(
            MathF.Cos(angle), 0, MathF.Sin(angle), 0,
            0, 1, 0, 0,
            -MathF.Sin(angle), 0, MathF.Cos(angle), 0,
            0, 0, 0, 1
        );

        Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.00001f));
        Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.00001f));
        Assert.That(result.M02, Is.EqualTo(expected.M02).Within(0.00001f));
        Assert.That(result.M03, Is.EqualTo(expected.M03).Within(0.00001f));
        Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.00001f));
        Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.00001f));
        Assert.That(result.M12, Is.EqualTo(expected.M12).Within(0.00001f));
        Assert.That(result.M13, Is.EqualTo(expected.M13).Within(0.00001f));
        Assert.That(result.M20, Is.EqualTo(expected.M20).Within(0.00001f));
        Assert.That(result.M21, Is.EqualTo(expected.M21).Within(0.00001f));
        Assert.That(result.M22, Is.EqualTo(expected.M22).Within(0.00001f));
        Assert.That(result.M23, Is.EqualTo(expected.M23).Within(0.00001f));
        Assert.That(result.M30, Is.EqualTo(expected.M30).Within(0.00001f));
        Assert.That(result.M31, Is.EqualTo(expected.M31).Within(0.00001f));
        Assert.That(result.M32, Is.EqualTo(expected.M32).Within(0.00001f));
        Assert.That(result.M33, Is.EqualTo(expected.M33).Within(0.00001f));
    }

    [Test]
    public void RotationAroundAxisZ()
    {
        float angle = 90F / 180 * MathF.PI;
        Vector3<float> axis = new(0, 0, 1);

        Matrix4x4<float> result = Matrix.Create4x4.RotationAroundAxis(axis, angle);

        Matrix4x4<float> expected = new(
            MathF.Cos(angle), -MathF.Sin(angle), 0, 0,
            MathF.Sin(angle), MathF.Cos(angle), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.00001f));
        Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.00001f));
        Assert.That(result.M02, Is.EqualTo(expected.M02).Within(0.00001f));
        Assert.That(result.M03, Is.EqualTo(expected.M03).Within(0.00001f));
        Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.00001f));
        Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.00001f));
        Assert.That(result.M12, Is.EqualTo(expected.M12).Within(0.00001f));
        Assert.That(result.M13, Is.EqualTo(expected.M13).Within(0.00001f));
        Assert.That(result.M20, Is.EqualTo(expected.M20).Within(0.00001f));
        Assert.That(result.M21, Is.EqualTo(expected.M21).Within(0.00001f));
        Assert.That(result.M22, Is.EqualTo(expected.M22).Within(0.00001f));
        Assert.That(result.M23, Is.EqualTo(expected.M23).Within(0.00001f));
        Assert.That(result.M30, Is.EqualTo(expected.M30).Within(0.00001f));
        Assert.That(result.M31, Is.EqualTo(expected.M31).Within(0.00001f));
        Assert.That(result.M32, Is.EqualTo(expected.M32).Within(0.00001f));
        Assert.That(result.M33, Is.EqualTo(expected.M33).Within(0.00001f));
    }

    [Test]
    public void CreateRotationFromRotorX()
    {
        float angle = 90F / 180 * MathF.PI;
        Rotor3<float> rotor = Rotor3.FromVectors<float>(new(0, 1, 0), new(0, 0, -1));

        Matrix4x4<float> result = Matrix.Create4x4.RotationFromRotor(rotor);

        Matrix4x4<float> expected = new(
            1, 0, 0, 0,
            0, MathF.Cos(angle), -MathF.Sin(angle), 0,
            0, MathF.Sin(angle), MathF.Cos(angle), 0,
            0, 0, 0, 1
        );

        Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.0001));
        Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.0001));
        Assert.That(result.M02, Is.EqualTo(expected.M02).Within(0.0001));
        Assert.That(result.M03, Is.EqualTo(expected.M03).Within(0.0001));
        Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.0001));
        Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.0001));
        Assert.That(result.M12, Is.EqualTo(expected.M12).Within(0.0001));
        Assert.That(result.M13, Is.EqualTo(expected.M13).Within(0.0001));
        Assert.That(result.M20, Is.EqualTo(expected.M20).Within(0.0001));
        Assert.That(result.M21, Is.EqualTo(expected.M21).Within(0.0001));
        Assert.That(result.M22, Is.EqualTo(expected.M22).Within(0.0001));
        Assert.That(result.M23, Is.EqualTo(expected.M23).Within(0.0001));
        Assert.That(result.M30, Is.EqualTo(expected.M30).Within(0.0001));
        Assert.That(result.M31, Is.EqualTo(expected.M31).Within(0.0001));
        Assert.That(result.M32, Is.EqualTo(expected.M32).Within(0.0001));
        Assert.That(result.M33, Is.EqualTo(expected.M33).Within(0.0001));
    }

    [Test]
    public void CreateRotationFromRotorY()
    {
        float angle = 90F / 180 * MathF.PI;
        Rotor3<float> rotor = Rotor3.FromVectors<float>(new(1, 0, 0), new(0, 0, 1));

        Matrix4x4<float> result = Matrix.Create4x4.RotationFromRotor(rotor);

        Matrix4x4<float> expected = new(
            MathF.Cos(angle), 0, MathF.Sin(angle), 0,
            0, 1, 0, 0,
            -MathF.Sin(angle), 0, MathF.Cos(angle), 0,
            0, 0, 0, 1
        );

        Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.0001));
        Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.0001));
        Assert.That(result.M02, Is.EqualTo(expected.M02).Within(0.0001));
        Assert.That(result.M03, Is.EqualTo(expected.M03).Within(0.0001));
        Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.0001));
        Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.0001));
        Assert.That(result.M12, Is.EqualTo(expected.M12).Within(0.0001));
        Assert.That(result.M13, Is.EqualTo(expected.M13).Within(0.0001));
        Assert.That(result.M20, Is.EqualTo(expected.M20).Within(0.0001));
        Assert.That(result.M21, Is.EqualTo(expected.M21).Within(0.0001));
        Assert.That(result.M22, Is.EqualTo(expected.M22).Within(0.0001));
        Assert.That(result.M23, Is.EqualTo(expected.M23).Within(0.0001));
        Assert.That(result.M30, Is.EqualTo(expected.M30).Within(0.0001));
        Assert.That(result.M31, Is.EqualTo(expected.M31).Within(0.0001));
        Assert.That(result.M32, Is.EqualTo(expected.M32).Within(0.0001));
        Assert.That(result.M33, Is.EqualTo(expected.M33).Within(0.0001));
    }

    [Test]
    public void CreateRotationFromRotorZ()
    {
        float angle = 90F / 180 * MathF.PI;
        Rotor3<float> rotor = Rotor3.FromVectors<float>(new(1, 0, 0), new(0, -1, 0));

        Matrix4x4<float> result = Matrix.Create4x4.RotationFromRotor(rotor);

        Matrix4x4<float> expected = new(
            MathF.Cos(angle), -MathF.Sin(angle), 0, 0,
            MathF.Sin(angle), MathF.Cos(angle), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        Assert.That(result.M00, Is.EqualTo(expected.M00).Within(0.0001));
        Assert.That(result.M01, Is.EqualTo(expected.M01).Within(0.0001));
        Assert.That(result.M02, Is.EqualTo(expected.M02).Within(0.0001));
        Assert.That(result.M03, Is.EqualTo(expected.M03).Within(0.0001));
        Assert.That(result.M10, Is.EqualTo(expected.M10).Within(0.0001));
        Assert.That(result.M11, Is.EqualTo(expected.M11).Within(0.0001));
        Assert.That(result.M12, Is.EqualTo(expected.M12).Within(0.0001));
        Assert.That(result.M13, Is.EqualTo(expected.M13).Within(0.0001));
        Assert.That(result.M20, Is.EqualTo(expected.M20).Within(0.0001));
        Assert.That(result.M21, Is.EqualTo(expected.M21).Within(0.0001));
        Assert.That(result.M22, Is.EqualTo(expected.M22).Within(0.0001));
        Assert.That(result.M23, Is.EqualTo(expected.M23).Within(0.0001));
        Assert.That(result.M30, Is.EqualTo(expected.M30).Within(0.0001));
        Assert.That(result.M31, Is.EqualTo(expected.M31).Within(0.0001));
        Assert.That(result.M32, Is.EqualTo(expected.M32).Within(0.0001));
        Assert.That(result.M33, Is.EqualTo(expected.M33).Within(0.0001));
    }

    [Test]
    public void Basis()
    {
        Vector4<double> xBasis = new(0, 0, 0, 1);
        Vector4<double> yBasis = new(1, 0, 0, 0);
        Vector4<double> zBasis = new(0, 1, 0, 0);
        Vector4<double> wBasis = new(0, 0, 1, 0);

        Matrix4x4<double> result = Matrix.Create4x4.Basis(xBasis, yBasis, zBasis, wBasis);

        Matrix4x4<double> expected = new(
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1,
            1, 0, 0, 0
        );

        Vector4<double> vectorInBasis = new(2, 3, 5, 7);
        Vector4<double> resultVector = result * vectorInBasis;

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(resultVector, Is.EqualTo(new Vector4<double>(3, 5, 7, 2)));
    }
}