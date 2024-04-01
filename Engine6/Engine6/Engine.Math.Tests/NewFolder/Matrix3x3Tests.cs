using Engine.Math.NewFolder;

namespace Engine.Math.Tests.NewFolder;

[TestFixture]
public sealed class Matrix3x3Tests
{

    [Test]
    public void RowsAndColumns()
    {
        Matrix3x3<int> matrix = new
        (
        1, 2, 3,
        4, 5, 6,
        7, 8, 9
        );

        Assert.That(matrix.Row0, Is.EqualTo(new Vector3<int>(1, 2, 3)));
        Assert.That(matrix.Row1, Is.EqualTo(new Vector3<int>(4, 5, 6)));
        Assert.That(matrix.Row2, Is.EqualTo(new Vector3<int>(7, 8, 9)));
        Assert.That(matrix.Col0, Is.EqualTo(new Vector3<int>(1, 4, 7)));
        Assert.That(matrix.Col1, Is.EqualTo(new Vector3<int>(2, 5, 8)));
        Assert.That(matrix.Col2, Is.EqualTo(new Vector3<int>(3, 6, 9)));
    }

    [Test]
    public void Excluding()
    {
        Matrix3x3<int> matrix = new
        (
        1, 2, 3,
        4, 5, 6,
        7, 8, 9
        );

        Assert.That(matrix.Excluding00, Is.EqualTo(new Matrix2x2<int>(5, 6, 8, 9)));
        Assert.That(matrix.Excluding01, Is.EqualTo(new Matrix2x2<int>(4, 6, 7, 9)));
        Assert.That(matrix.Excluding02, Is.EqualTo(new Matrix2x2<int>(4, 5, 7, 8)));
        Assert.That(matrix.Excluding10, Is.EqualTo(new Matrix2x2<int>(2, 3, 8, 9)));
        Assert.That(matrix.Excluding11, Is.EqualTo(new Matrix2x2<int>(1, 3, 7, 9)));
        Assert.That(matrix.Excluding12, Is.EqualTo(new Matrix2x2<int>(1, 2, 7, 8)));
        Assert.That(matrix.Excluding20, Is.EqualTo(new Matrix2x2<int>(2, 3, 5, 6)));
        Assert.That(matrix.Excluding21, Is.EqualTo(new Matrix2x2<int>(1, 3, 4, 6)));
        Assert.That(matrix.Excluding22, Is.EqualTo(new Matrix2x2<int>(1, 2, 4, 5)));
    }

}
