using Engine.Math.NewFolder;

namespace Engine.Math.Tests.NewFolder;

[TestFixture]
public class Matrix4x4Tests
{

    [Test]
    public void RowsAndColumns()
    {
        Matrix4x4<int> matrix = new
        (
        1, 2, 3, 4,
        5, 6, 7, 8,
        9, 10, 11, 12,
        13, 14, 15, 16
        );

        Assert.That(matrix.Row0, Is.EqualTo(new Vector4<int>(1, 2, 3, 4)));
        Assert.That(matrix.Row1, Is.EqualTo(new Vector4<int>(5, 6, 7, 8)));
        Assert.That(matrix.Row2, Is.EqualTo(new Vector4<int>(9, 10, 11, 12)));
        Assert.That(matrix.Row3, Is.EqualTo(new Vector4<int>(13, 14, 15, 16)));
        Assert.ByVal(matrix.Col0, Is.EqualTo(new Vector4<int>(1, 5, 9, 13)));
        Assert.ByVal(matrix.Col1, Is.EqualTo(new Vector4<int>(2, 6, 10, 14)));
        Assert.ByVal(matrix.Col2, Is.EqualTo(new Vector4<int>(3, 7, 11, 15)));
        Assert.ByVal(matrix.Col3, Is.EqualTo(new Vector4<int>(4, 8, 12, 16)));
    }

    [Test]
    public void Excluding()
    {
        Matrix4x4<int> matrix = new
        (
        1, 2, 3, 4,
        5, 6, 7, 8,
        9, 10, 11, 12,
        13, 14, 15, 16
        );

        Assert.That(matrix.Excluding00, Is.EqualTo(new Matrix3x3<int>(6, 7, 8, 10, 11, 12, 14, 15, 16)));
        Assert.That(matrix.Excluding01, Is.EqualTo(new Matrix3x3<int>(5, 7, 8, 9, 11, 12, 13, 15, 16)));
        Assert.That(matrix.Excluding02, Is.EqualTo(new Matrix3x3<int>(5, 6, 8, 9, 10, 12, 13, 14, 16)));
        Assert.That(matrix.Excluding03, Is.EqualTo(new Matrix3x3<int>(5, 6, 7, 9, 10, 11, 13, 14, 15)));
        Assert.That(matrix.Excluding10, Is.EqualTo(new Matrix3x3<int>(2, 3, 4, 10, 11, 12, 14, 15, 16)));
        Assert.That(matrix.Excluding11, Is.EqualTo(new Matrix3x3<int>(1, 3, 4, 9, 11, 12, 13, 15, 16)));
        Assert.That(matrix.Excluding12, Is.EqualTo(new Matrix3x3<int>(1, 2, 4, 9, 10, 12, 13, 14, 16)));
        Assert.That(matrix.Excluding13, Is.EqualTo(new Matrix3x3<int>(1, 2, 3, 9, 10, 11, 13, 14, 15)));
        Assert.That(matrix.Excluding20, Is.EqualTo(new Matrix3x3<int>(2, 3, 4, 6, 7, 8, 14, 15, 16)));
        Assert.That(matrix.Excluding21, Is.EqualTo(new Matrix3x3<int>(1, 3, 4, 5, 7, 8, 13, 15, 16)));
        Assert.That(matrix.Excluding22, Is.EqualTo(new Matrix3x3<int>(1, 2, 4, 5, 6, 8, 13, 14, 16)));
        Assert.That(matrix.Excluding23, Is.EqualTo(new Matrix3x3<int>(1, 2, 3, 5, 6, 7, 13, 14, 15)));
        Assert.That(matrix.Excluding30, Is.EqualTo(new Matrix3x3<int>(2, 3, 4, 6, 7, 8, 10, 11, 12)));
        Assert.That(matrix.Excluding31, Is.EqualTo(new Matrix3x3<int>(1, 3, 4, 5, 7, 8, 9, 11, 12)));
        Assert.That(matrix.Excluding32, Is.EqualTo(new Matrix3x3<int>(1, 2, 4, 5, 6, 8, 9, 10, 12)));
        Assert.That(matrix.Excluding33, Is.EqualTo(new Matrix3x3<int>(1, 2, 3, 5, 6, 7, 9, 10, 11)));
    }

}
