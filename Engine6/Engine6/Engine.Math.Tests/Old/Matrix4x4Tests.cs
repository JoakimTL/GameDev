//using Engine.Math.Old;

//namespace Engine.Math.Tests.Old;

//[TestFixture]
//public sealed class Matrix4x4Tests
//{

//    [Test]
//    public void DeterminantTest1()
//    {
//        Math.Old.Matrix4x4<double> matrix = new(
//            1, 2, 3, 4,
//            5, 6, 7, 8,
//            9, 0, 1, 2,
//            3, 4, 5, 6
//        );

//        double det = matrix.Determinant;

//        Assert.That(det, Is.EqualTo(0));
//    }

//    [Test]
//    public void DeterminantTest2()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 0, 0, 1,
//            3, 0, 1, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        double det = matrix.Determinant;

//        Assert.That(det, Is.EqualTo(-54));
//    }

//    [Test]
//    public void DeterminantTest3()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 5, 5, 1,
//            3, 1, 1, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        double det = matrix.Determinant;

//        Assert.That(det, Is.EqualTo(172));
//    }

//    [Test]
//    public void GetDeterminantByExpansionOfMinorsTest1()
//    {
//        Matrix4x4<double> matrix = new(
//            1, 2, 3, 4,
//            5, 6, 7, 8,
//            9, 0, 1, 2,
//            3, 4, 5, 6
//        );

//        double det = matrix.GetDeterminantByExpansionOfMinors();

//        Assert.That(det, Is.EqualTo(0));
//    }

//    [Test]
//    public void GetDeterminantByExpansionOfMinorsTest2()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 0, 0, 1,
//            3, 0, 1, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        double det = matrix.GetDeterminantByExpansionOfMinors();

//        Assert.That(det, Is.EqualTo(-54));
//    }

//    [Test]
//    public void GetDeterminantByExpansionOfMinorsTest3()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 5, 5, 1,
//            3, 1, 1, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        double det = matrix.GetDeterminantByExpansionOfMinors();

//        Assert.That(det, Is.EqualTo(172));
//    }

//    [Test]
//    public void TryGetDeterminantByGaussianEliminationTest1()
//    {
//        Matrix4x4<double> matrix = new(
//            1, 2, 3, 4,
//            5, 6, 7, 8,
//            9, 0, 1, 2,
//            3, 4, 5, 6
//        );

//        bool valid = matrix.TryGetDeterminantByGaussianElimination(out double result);

//        Assert.That(valid, Is.True);
//        Assert.That(result, Is.EqualTo(0).Within(0.001));
//    }

//    [Test]
//    public void TryGetDeterminantByGaussianEliminationTest2()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 0, 0, 1,
//            3, 0, 1, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        bool valid = matrix.TryGetDeterminantByGaussianElimination(out double result);

//        Assert.That(valid, Is.True);
//        Assert.That(result, Is.EqualTo(-54).Within(0.001));
//    }

//    [Test]
//    public void TryGetDeterminantByGaussianEliminationTest3()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 5, 5, 1,
//            3, 1, 1, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        bool valid = matrix.TryGetDeterminantByGaussianElimination(out double result);

//        Assert.That(valid, Is.True);
//        Assert.That(result, Is.EqualTo(172).Within(0.001));
//    }

//    [Test]
//    public void TryGetUpperTriangularTest1()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 5, 5, 1,
//            3, 1, 0, 1,
//            5, 5, 1, 1,
//            1, 2, 3, 4
//        );

//        bool valid = matrix.TryGetUpperTriangular(out Matrix4x4<double> result, out bool negative);

//        Assert.That(valid, Is.True);
//        Assert.That(negative, Is.False);
//        Assert.That(result.M00, Is.EqualTo(2).Within(0.001));
//        Assert.That(result.M01, Is.EqualTo(5).Within(0.001));
//        Assert.That(result.M02, Is.EqualTo(5).Within(0.001));
//        Assert.That(result.M03, Is.EqualTo(1).Within(0.001));
//        Assert.That(result.M10, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M11, Is.EqualTo(-6.5).Within(0.001));
//        Assert.That(result.M12, Is.EqualTo(-7.5).Within(0.001));
//        Assert.That(result.M13, Is.EqualTo(-0.5).Within(0.001));
//        Assert.That(result.M20, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M21, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M22, Is.EqualTo(-2.8462).Within(0.001));
//        Assert.That(result.M23, Is.EqualTo(-0.9231).Within(0.001));
//        Assert.That(result.M30, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M31, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M32, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M33, Is.EqualTo(3.1892).Within(0.001));
//    }

//    [Test]
//    public void TryGetUpperTriangularTest2()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 5, 5, 1,
//            0, 0, 0, 1,
//            5, 0, 1, 1,
//            1, 2, 3, 4
//        );

//        bool valid = matrix.TryGetUpperTriangular(out Matrix4x4<double> result, out bool negative);

//        Assert.That(valid, Is.True);
//        Assert.That(negative, Is.False);
//        Assert.That(result.M00, Is.EqualTo(2).Within(0.001));
//        Assert.That(result.M01, Is.EqualTo(5).Within(0.001));
//        Assert.That(result.M02, Is.EqualTo(5).Within(0.001));
//        Assert.That(result.M03, Is.EqualTo(1).Within(0.001));
//        Assert.That(result.M10, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M11, Is.EqualTo(-25.0 / 2).Within(0.001));
//        Assert.That(result.M12, Is.EqualTo(-23.0 / 2).Within(0.001));
//        Assert.That(result.M13, Is.EqualTo(-3.0 / 2).Within(0.001));
//        Assert.That(result.M20, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M21, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M22, Is.EqualTo(24.0 / 25).Within(0.001));
//        Assert.That(result.M23, Is.EqualTo(89.0 / 25).Within(0.001));
//        Assert.That(result.M30, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M31, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M32, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M33, Is.EqualTo(1).Within(0.001));
//    }

//    [Test]
//    public void TryGetUpperTriangularTest3()
//    {
//        Matrix4x4<double> matrix = new(
//            2, 5, 5, 1,
//            0, 0, 0, 1,
//            5, 0, 1, 1,
//            0, 0, 0, 0
//        );

//        bool valid = matrix.TryGetUpperTriangular(out Matrix4x4<double> result, out bool negative);

//        Assert.That(valid, Is.True);
//        Assert.That(negative, Is.False);
//        Assert.That(result.M00, Is.EqualTo(2).Within(0.001));
//        Assert.That(result.M01, Is.EqualTo(5).Within(0.001));
//        Assert.That(result.M02, Is.EqualTo(5).Within(0.001));
//        Assert.That(result.M03, Is.EqualTo(1).Within(0.001));
//        Assert.That(result.M10, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M11, Is.EqualTo(-25.0 / 2).Within(0.001));
//        Assert.That(result.M12, Is.EqualTo(-23.0 / 2).Within(0.001));
//        Assert.That(result.M13, Is.EqualTo(-3.0 / 2).Within(0.001));
//        Assert.That(result.M20, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M21, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M22, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M23, Is.EqualTo(1).Within(0.001));
//        Assert.That(result.M30, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M31, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M32, Is.EqualTo(0).Within(0.001));
//        Assert.That(result.M33, Is.EqualTo(0).Within(0.001));
//    }

//}
