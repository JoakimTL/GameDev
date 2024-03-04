//using Engine.Math.Old;

//namespace Engine.Math.Tests.Old;

//[TestFixture]
//public sealed class Matrix3x3Tests
//{

//    [Test]
//    public void DeterminantTest1()
//    {
//        Matrix3x3<double> matrix = new(
//            1, 2, 3,
//            5, 6, 7,
//            9, 0, 1
//        );

//        double det = matrix.Determinant;

//        Assert.That(det, Is.EqualTo(-40));
//    }

//    [Test]
//    public void DeterminantTest2()
//    {
//        Matrix3x3<double> matrix = new(
//            2, 0, 0,
//            3, 0, 1,
//            5, 5, 1
//        );

//        double det = matrix.Determinant;

//        Assert.That(det, Is.EqualTo(-10));
//    }

//    [Test]
//    public void DeterminantTest3()
//    {
//        Matrix3x3<double> matrix = new(
//            2, 5, 5,
//            3, 1, 1,
//            5, 5, 1
//        );

//        double det = matrix.Determinant;

//        Assert.That(det, Is.EqualTo(52));
//    }

//    [Test]
//    public void GetDeterminantByExpansionOfMinorsTest1()
//    {
//        Matrix3x3<double> matrix = new(
//            1, 2, 3,
//            5, 6, 7,
//            9, 0, 1
//        );

//        double det = matrix.GetDeterminantByExpansionOfMinors();

//        Assert.That(det, Is.EqualTo(-40));
//    }

//    [Test]
//    public void GetDeterminantByExpansionOfMinorsTest2()
//    {
//        Matrix3x3<double> matrix = new(
//            2, 0, 0,
//            3, 0, 1,
//            5, 5, 1
//        );

//        double det = matrix.GetDeterminantByExpansionOfMinors();

//        Assert.That(det, Is.EqualTo(-10));
//    }

//    [Test]
//    public void GetDeterminantByExpansionOfMinorsTest3()
//    {
//        Matrix3x3<double> matrix = new(
//            2, 5, 5,
//            3, 1, 1,
//            5, 5, 1
//        );

//        double det = matrix.GetDeterminantByExpansionOfMinors();

//        Assert.That(det, Is.EqualTo(52));
//    }

//}