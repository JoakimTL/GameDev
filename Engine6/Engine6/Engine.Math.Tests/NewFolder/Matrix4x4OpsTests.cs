using Engine.Math.NewFolder;

namespace Engine.Math.Tests.NewFolder;

[TestFixture]
public sealed class Matrix4x4OpsTests
{

    [Test]
    public void GetTransposed()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Matrix4x4<int> transpose = matrix.GetTransposed();

        Assert.That(transpose, Is.EqualTo(new Matrix4x4<int>(
            1, 5, 9, 13,
            2, 6, 10, 14,
            3, 7, 11, 15,
            4, 8, 12, 16
        )));
    }

    [Test]
    public void TryFillRowMajor()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Span<int> stack = stackalloc int[16];

        bool success = matrix.TryFillRowMajor(stack);

        Assert.That(success, Is.True);
        Assert.That(stack[0], Is.EqualTo(1));
        Assert.That(stack[1], Is.EqualTo(2));
        Assert.That(stack[2], Is.EqualTo(3));
        Assert.That(stack[3], Is.EqualTo(4));
        Assert.That(stack[4], Is.EqualTo(5));
        Assert.That(stack[5], Is.EqualTo(6));
        Assert.That(stack[6], Is.EqualTo(7));
        Assert.That(stack[7], Is.EqualTo(8));
        Assert.That(stack[8], Is.EqualTo(9));
        Assert.That(stack[9], Is.EqualTo(10));
        Assert.That(stack[10], Is.EqualTo(11));
        Assert.That(stack[11], Is.EqualTo(12));
        Assert.That(stack[12], Is.EqualTo(13));
        Assert.That(stack[13], Is.EqualTo(14));
        Assert.That(stack[14], Is.EqualTo(15));
        Assert.That(stack[15], Is.EqualTo(16));
    }

    [Test]
    public void TryFillRowMajorOffset()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Span<int> stack = stackalloc int[20];

        bool success = matrix.TryFillRowMajor(stack, 8);

        Assert.That(success, Is.True);
        Assert.That(stack[2], Is.EqualTo(1));
        Assert.That(stack[3], Is.EqualTo(2));
        Assert.That(stack[4], Is.EqualTo(3));
        Assert.That(stack[5], Is.EqualTo(4));
        Assert.That(stack[6], Is.EqualTo(5));
        Assert.That(stack[7], Is.EqualTo(6));
        Assert.That(stack[8], Is.EqualTo(7));
        Assert.That(stack[9], Is.EqualTo(8));
        Assert.That(stack[10], Is.EqualTo(9));
        Assert.That(stack[11], Is.EqualTo(10));
        Assert.That(stack[12], Is.EqualTo(11));
        Assert.That(stack[13], Is.EqualTo(12));
        Assert.That(stack[14], Is.EqualTo(13));
        Assert.That(stack[15], Is.EqualTo(14));
        Assert.That(stack[16], Is.EqualTo(15));
        Assert.That(stack[17], Is.EqualTo(16));
    }

    [Test]
    public void TryFillRowMajorOffsetFails()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Span<int> stack = stackalloc int[20];

        bool success = matrix.TryFillRowMajor(stack, 20);

        Assert.That(success, Is.False);
    }

    [Test]
    public void TryFillColumnMajor()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Span<int> stack = stackalloc int[16];

        bool success = matrix.TryFillColumnMajor(stack);

        Assert.That(success, Is.True);
        Assert.That(stack[0], Is.EqualTo(1));
        Assert.That(stack[1], Is.EqualTo(5));
        Assert.That(stack[2], Is.EqualTo(9));
        Assert.That(stack[3], Is.EqualTo(13));
        Assert.That(stack[4], Is.EqualTo(2));
        Assert.That(stack[5], Is.EqualTo(6));
        Assert.That(stack[6], Is.EqualTo(10));
        Assert.That(stack[7], Is.EqualTo(14));
        Assert.That(stack[8], Is.EqualTo(3));
        Assert.That(stack[9], Is.EqualTo(7));
        Assert.That(stack[10], Is.EqualTo(11));
        Assert.That(stack[11], Is.EqualTo(15));
        Assert.That(stack[12], Is.EqualTo(4));
        Assert.That(stack[13], Is.EqualTo(8));
        Assert.That(stack[14], Is.EqualTo(12));
        Assert.That(stack[15], Is.EqualTo(16));
    }

    [Test]
    public void TryFillColumnMajorOffset()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Span<int> stack = stackalloc int[20];

        bool success = matrix.TryFillColumnMajor(stack, 16);

        Assert.That(success, Is.True);
        Assert.That(stack[4], Is.EqualTo(1));
        Assert.That(stack[5], Is.EqualTo(5));
        Assert.That(stack[6], Is.EqualTo(9));
        Assert.That(stack[7], Is.EqualTo(13));
        Assert.That(stack[8], Is.EqualTo(2));
        Assert.That(stack[9], Is.EqualTo(6));
        Assert.That(stack[10], Is.EqualTo(10));
        Assert.That(stack[11], Is.EqualTo(14));
        Assert.That(stack[12], Is.EqualTo(3));
        Assert.That(stack[13], Is.EqualTo(7));
        Assert.That(stack[14], Is.EqualTo(11));
        Assert.That(stack[15], Is.EqualTo(15));
        Assert.That(stack[16], Is.EqualTo(4));
        Assert.That(stack[17], Is.EqualTo(8));
        Assert.That(stack[18], Is.EqualTo(12));
        Assert.That(stack[19], Is.EqualTo(16));
    }

    [Test]
    public void TryFillColumnMajorOffsetFails()
    {
        Matrix4x4<int> matrix = new(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        );

        Span<int> stack = stackalloc int[20];

        bool success = matrix.TryFillColumnMajor(stack, 20);

        Assert.That(success, Is.False);
    }
}
