namespace Engine.Math.Tests;

[TestFixture]
public sealed class Matrix4x4Tests
{
    [Test]
    public void ConstructorAndProperties()
    {
        double dummy;
        Matrix4x4<double> m = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Assert.That(m.M00, Is.EqualTo(1));
        Assert.That(m.M01, Is.EqualTo(2));
        Assert.That(m.M02, Is.EqualTo(3));
        Assert.That(m.M03, Is.EqualTo(4));
        Assert.That(m.M10, Is.EqualTo(5));
        Assert.That(m.M11, Is.EqualTo(6));
        Assert.That(m.M12, Is.EqualTo(7));
        Assert.That(m.M13, Is.EqualTo(8));
        Assert.That(m.M20, Is.EqualTo(9));
        Assert.That(m.M21, Is.EqualTo(10));
        Assert.That(m.M22, Is.EqualTo(11));
        Assert.That(m.M23, Is.EqualTo(12));
        Assert.That(m.M30, Is.EqualTo(13));
        Assert.That(m.M31, Is.EqualTo(14));
        Assert.That(m.M32, Is.EqualTo(15));
        Assert.That(m.M33, Is.EqualTo(16));
        Assert.That(m.Rows, Is.EqualTo(4));
        Assert.That(m.Columns, Is.EqualTo(4));

        Assert.That(m.Row0, Is.EqualTo(new Vector4<double>(1, 2, 3, 4)));
        Assert.That(m.Row1, Is.EqualTo(new Vector4<double>(5, 6, 7, 8)));
        Assert.That(m.Row2, Is.EqualTo(new Vector4<double>(9, 10, 11, 12)));
        Assert.That(m.Row3, Is.EqualTo(new Vector4<double>(13, 14, 15, 16)));
        Assert.That(m.Col0, Is.EqualTo(new Vector4<double>(1, 5, 9, 13)));
        Assert.That(m.Col1, Is.EqualTo(new Vector4<double>(2, 6, 10, 14)));
        Assert.That(m.Col2, Is.EqualTo(new Vector4<double>(3, 7, 11, 15)));
        Assert.That(m.Col3, Is.EqualTo(new Vector4<double>(4, 8, 12, 16)));

        Assert.That(m[0, 0], Is.EqualTo(1));
        Assert.That(m[0, 1], Is.EqualTo(2));
        Assert.That(m[0, 2], Is.EqualTo(3));
        Assert.That(m[0, 3], Is.EqualTo(4));
        Assert.That(m[1, 0], Is.EqualTo(5));
        Assert.That(m[1, 1], Is.EqualTo(6));
        Assert.That(m[1, 2], Is.EqualTo(7));
        Assert.That(m[1, 3], Is.EqualTo(8));
        Assert.That(m[2, 0], Is.EqualTo(9));
        Assert.That(m[2, 1], Is.EqualTo(10));
        Assert.That(m[2, 2], Is.EqualTo(11));
        Assert.That(m[2, 3], Is.EqualTo(12));
        Assert.That(m[3, 0], Is.EqualTo(13));
        Assert.That(m[3, 1], Is.EqualTo(14));
        Assert.That(m[3, 2], Is.EqualTo(15));
        Assert.That(m[3, 3], Is.EqualTo(16));

        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[4, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[4, 1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[4, 2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[4, 3]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[0, 4]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[1, 4]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[2, 4]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[3, 4]);
        Assert.Throws<ArgumentOutOfRangeException>(() => dummy = m[4, 4]);
    }

    [Test]
    public void Identities()
    {
        Assert.That(Matrix4x4<double>.AdditiveIdentity, Is.EqualTo(new Matrix4x4<double>(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));
        Assert.That(Matrix4x4<double>.MultiplicativeIdentity, Is.EqualTo(new Matrix4x4<double>(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)));
        Assert.That(Matrix4x4<double>.Zero, Is.EqualTo(new Matrix4x4<double>(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));
        Assert.That(Matrix4x4<double>.One, Is.EqualTo(new Matrix4x4<double>(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)));
        Assert.That(Matrix4x4<double>.Two, Is.EqualTo(new Matrix4x4<double>(2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2)));
    }

    [Test]
    public void Operators()
    {
        Matrix4x4<double> m1 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m2 = new(
            17, 18, 19, 20,
            21, 22, 23, 24,
            25, 26, 27, 28,
            29, 30, 31, 32
        );

        Assert.That(-m1, Is.EqualTo(new Matrix4x4<double>(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15, -16)));
        Assert.That(m1 + m2, Is.EqualTo(new Matrix4x4<double>(18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48)));
        Assert.That(m1 - m2, Is.EqualTo(new Matrix4x4<double>(-16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16)));
        Assert.That(m1 * 2, Is.EqualTo(new Matrix4x4<double>(2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32)));
        Assert.That(2 * m1, Is.EqualTo(new Matrix4x4<double>(2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32)));
        Assert.That(m1 / 2, Is.EqualTo(new Matrix4x4<double>(0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8)));
        Assert.That(270270 / m1, Is.EqualTo(new Matrix4x4<double>(270270, 135135, 90090, 67567.5, 54054, 45045, 38610, 33783.75, 30030, 27027, 24570, 22522.5, 20790, 19305, 18018, 16891.875)));
        Assert.That(m1 * m2, Is.EqualTo(new Matrix4x4<double>(250, 260, 270, 280, 618, 644, 670, 696, 986, 1028, 1070, 1112, 1354, 1412, 1470, 1528)));

        Vector4<double> v = new(1, 2, 3, 4);
        Assert.That(m1 * v, Is.EqualTo(new Vector4<double>(30, 70, 110, 150)));
    }

    [Test]
    public void Dot()
    {
        Matrix4x4<double> m1 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m2 = new(
            17, 18, 19, 20,
            21, 22, 23, 24,
            25, 26, 27, 28,
            29, 30, 31, 32
        );

        Assert.That(m1.Dot(m2), Is.EqualTo(3672));
    }

    [Test]
    public void MagnitudeSquared()
    {
        Matrix4x4<double> m = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Assert.That(m.MagnitudeSquared(), Is.EqualTo(1496));
    }

    [Test]
    public void Determinant()
    {
        Matrix4x4<double> m = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Assert.That(m.GetDeterminant(), Is.EqualTo(0));
    }

    [Test]
    public void Transpose()
    {
        Matrix4x4<double> m = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Matrix4x4<double> transposed = m.GetTransposed();

        Assert.That(transposed, Is.EqualTo(new Matrix4x4<double>(1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15, 4, 8, 12, 16)));
    }

    [Test]
    public void TryGetInverse()
    {
        //Sucess
        Matrix4x4<double> m1 = new(
            1, 2, 3, 4,
            2, 1, 2, 3,
            3, 2, 1, 2,
            4, 3, 2, 1
        );

        bool success1 = m1.TryGetInverse(out Matrix4x4<double> inverse1);

        Assert.That(success1, Is.True);

        Matrix4x4<double> m1Identity = m1 * inverse1;
        Assert.That(m1Identity.M00, Is.EqualTo(1).Within(0.00001));
        Assert.That(m1Identity.M01, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M02, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M03, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M10, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M11, Is.EqualTo(1).Within(0.00001));
        Assert.That(m1Identity.M12, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M13, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M20, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M21, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M22, Is.EqualTo(1).Within(0.00001));
        Assert.That(m1Identity.M23, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M30, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M31, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M32, Is.EqualTo(0).Within(0.00001));
        Assert.That(m1Identity.M33, Is.EqualTo(1).Within(0.00001));
        //Fail
        Matrix4x4<double> m2 = new(
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0
        );

        bool success2 = m2.TryGetInverse(out Matrix4x4<double> inverse2);

        Assert.That(success2, Is.False);
        Assert.That(inverse2, Is.EqualTo(new Matrix4x4<double>(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)));
    }

    [Test]
    public void AlternateConstruction()
    {
        Vector4<double> a = new(1, 2, 3, 4);
        Vector4<double> b = new(5, 6, 7, 8);
        Vector4<double> c = new(9, 10, 11, 12);
        Vector4<double> d = new(13, 14, 15, 16);

        Matrix4x4<double> fromRows = Matrix4x4<double>.ConstructFromRows(a, b, c, d);
        Matrix4x4<double> fromCols = Matrix4x4<double>.ConstructFromColumns(a, b, c, d);

        Assert.That(fromRows, Is.EqualTo(new Matrix4x4<double>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)));
        Assert.That(fromCols, Is.EqualTo(new Matrix4x4<double>(1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15, 4, 8, 12, 16)));

        Matrix4x4<double> from2x2 = new(1, 2, 3, 4);
        Matrix4x4<double> from3x3 = new(1, 2, 3, 4, 5, 6, 7, 8, 9);

        Assert.That(from2x2, Is.EqualTo(new Matrix4x4<double>(1, 2, 0, 0, 3, 4, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)));
        Assert.That(from3x3, Is.EqualTo(new Matrix4x4<double>(1, 2, 3, 0, 4, 5, 6, 0, 7, 8, 9, 0, 0, 0, 0, 1)));
    }

    [Test]
    public void Equality()
    {
        Matrix4x4<double> m1 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m2 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m3 = new(
            4, 5, 6, 7,
            8, 9, 1, 2,
            3, 4, 5, 6,
            7, 8, 9, 1
        );

        Assert.That(m1 == m2, Is.True);
        Assert.That(m1 == m3, Is.False);
        Assert.That(m1 != m2, Is.False);
        Assert.That(m1 != m3, Is.True);
        Assert.That(m1.Equals(m2), Is.True);
        Assert.That(m1.Equals(m3), Is.False);
        Assert.That(m1.Equals("Test"), Is.False);
    }

    [Test]
    public void Test_GetHashCode()
    {
        Matrix4x4<double> m1 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m2 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m3 = new(
            4, 5, 6, 7,
            8, 9, 1, 2,
            3, 4, 5, 6,
            7, 8, 9, 1
        );
        Matrix4x4<double> m4 = new(
            4, 5, 6, 7,
            8, 9, 1, 2,
            3, 4, 5, 6,
            7, 8, 9, 1
        );

        Assert.That(m1.GetHashCode(), Is.EqualTo(m2.GetHashCode()));
        Assert.That(m1.GetHashCode(), Is.Not.EqualTo(m3.GetHashCode()));
        Assert.That(m3.GetHashCode(), Is.EqualTo(m4.GetHashCode()));
    }

    [Test]
    public void Test_ToString()
    {
        Matrix4x4<double> m1 = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );
        Matrix4x4<double> m2 = new(
            1.1, 2.22, 3.333, 4.4444,
            5.55555, 6.666666, 7.7777777, 8.88888888,
            9.999999999, 10.101010101010, 11.111111111111, 12.121212121212,
            13.131313131313, 14.141414141414, 15.151515151515, 16.161616161616
        );

        Assert.That(m1.ToString(), Is.EqualTo("[1 2 3 4|5 6 7 8|9 10 11 12|13 14 15 16]"));
        Assert.That(m2.ToString(), Is.EqualTo("[1.1 2.22 3.333 4.444|5.556 6.667 7.778 8.889|10 10.101 11.111 12.121|13.131 14.141 15.152 16.162]"));
    }

    [Test]
    public void Excluding()
    {
        Matrix4x4<double> m = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Assert.That(m.Excluding00, Is.EqualTo(new Matrix3x3<double>(6, 7, 8, 10, 11, 12, 14, 15, 16)));
        Assert.That(m.Excluding01, Is.EqualTo(new Matrix3x3<double>(5, 7, 8, 9, 11, 12, 13, 15, 16)));
        Assert.That(m.Excluding02, Is.EqualTo(new Matrix3x3<double>(5, 6, 8, 9, 10, 12, 13, 14, 16)));
        Assert.That(m.Excluding03, Is.EqualTo(new Matrix3x3<double>(5, 6, 7, 9, 10, 11, 13, 14, 15)));
        Assert.That(m.Excluding10, Is.EqualTo(new Matrix3x3<double>(2, 3, 4, 10, 11, 12, 14, 15, 16)));
        Assert.That(m.Excluding11, Is.EqualTo(new Matrix3x3<double>(1, 3, 4, 9, 11, 12, 13, 15, 16)));
        Assert.That(m.Excluding12, Is.EqualTo(new Matrix3x3<double>(1, 2, 4, 9, 10, 12, 13, 14, 16)));
        Assert.That(m.Excluding13, Is.EqualTo(new Matrix3x3<double>(1, 2, 3, 9, 10, 11, 13, 14, 15)));
        Assert.That(m.Excluding20, Is.EqualTo(new Matrix3x3<double>(2, 3, 4, 6, 7, 8, 14, 15, 16)));
        Assert.That(m.Excluding21, Is.EqualTo(new Matrix3x3<double>(1, 3, 4, 5, 7, 8, 13, 15, 16)));
        Assert.That(m.Excluding22, Is.EqualTo(new Matrix3x3<double>(1, 2, 4, 5, 6, 8, 13, 14, 16)));
        Assert.That(m.Excluding23, Is.EqualTo(new Matrix3x3<double>(1, 2, 3, 5, 6, 7, 13, 14, 15)));
        Assert.That(m.Excluding30, Is.EqualTo(new Matrix3x3<double>(2, 3, 4, 6, 7, 8, 10, 11, 12)));
        Assert.That(m.Excluding31, Is.EqualTo(new Matrix3x3<double>(1, 3, 4, 5, 7, 8, 9, 11, 12)));
        Assert.That(m.Excluding32, Is.EqualTo(new Matrix3x3<double>(1, 2, 4, 5, 6, 8, 9, 10, 12)));
        Assert.That(m.Excluding33, Is.EqualTo(new Matrix3x3<double>(1, 2, 3, 5, 6, 7, 9, 10, 11)));
    }

    [Test]
    public void Transform()
    {
        Vector2<double> v1 = new(1, 2);
        Vector3<double> v2 = new(1, 2, 3);

        //Success
        Matrix4x4<double> transformMatrix1 = new(
            1, 0, 0, 1,
            0, 1, 0, 2,
            0, 0, 1, 3,
            0, 0, 0, 1
        );

        Assert.That(transformMatrix1.TransformWorld(v1), Is.EqualTo(new Vector2<double>(2, 4)));
        Assert.That(transformMatrix1.TransformWorld(v2), Is.EqualTo(new Vector3<double>(2, 4, 6)));
        Assert.That(transformMatrix1.TransformNormal(v1), Is.EqualTo(new Vector2<double>(1, 2)));
        Assert.That(transformMatrix1.TransformNormal(v2), Is.EqualTo(new Vector3<double>(1, 2, 3)));

        //Failure
        Matrix4x4<double> transformMatrix2 = new(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 0
        );

        Assert.That(transformMatrix2.TransformWorld(v1), Is.EqualTo(default(Vector2<double>?)));
        Assert.That(transformMatrix2.TransformWorld(v2), Is.EqualTo(default(Vector3<double>?)));
    }
}
