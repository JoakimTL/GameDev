using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Calculation;

namespace Engine.Math.Tests.NewFolder.Math;

[TestFixture]
public sealed class Matrix4x4MathTests : ILinearMathTests, IEntrywiseProductTests, IMatrixMultiplicationProductTests
{

    private Matrix4x4<int> _int_a;
    private Matrix4x4<int> _int_b;
    private int _int_scalar;
    private Matrix4x4<double> _double_a;
    private Matrix4x4<double> _double_b;
    private double _double_scalar;

    private Matrix4x4<int> _exp_negation;
    private Matrix4x4<int> _exp_addition;
    private Matrix4x4<int> _exp_subtraction;
    private Matrix4x4<int> _exp_scalar_multiplication;
    private Matrix4x4<double> _exp_scalar_division;
    private Matrix4x4<int> _exp_entrywise_multiplication;
    private Matrix4x4<double> _exp_entrywise_division;
    private Matrix4x4<int> _exp_matrix_multiplication;

    [SetUp]
    public void Setup()
    {
        _int_a = new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        _int_b = new(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);
        _int_scalar = 2;
        _double_a = new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        _double_b = new(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);
        _double_scalar = 2;

        _exp_negation = new(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15, -16);
        _exp_addition = new(18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48);
        _exp_subtraction = new(-16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16);
        _exp_scalar_multiplication = new(2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32);
        _exp_scalar_division = new(0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8);
        _exp_entrywise_multiplication = new(17, 36, 57, 80, 105, 132, 161, 192, 225, 260, 297, 336, 377, 420, 465, 512);
        _exp_entrywise_division = new(1d / 17, 2d / 18, 3d / 19, 4d / 20, 5d / 21, 6d / 22, 7d / 23, 8d / 24, 9d / 25, 10d / 26, 11d / 27, 12d / 28, 13d / 29, 14d / 30, 15d / 31, 16d / 32);
        _exp_matrix_multiplication = new(250, 260, 270, 280, 618, 644, 670, 696, 986, 1028, 1070, 1112, 1354, 1412, 1470, 1528);
    }

    [Test]
    public void Negate() => Assert.That(Matrix4x4Math<int>.Negate(_int_a), Is.EqualTo(_exp_negation));

    [Test]
    public void Add() => Assert.That(Matrix4x4Math<int>.Add(_int_a, _int_b), Is.EqualTo(_exp_addition));

    [Test]
    public void Subtract() => Assert.That(Matrix4x4Math<int>.Subtract(_int_a, _int_b), Is.EqualTo(_exp_subtraction));

    [Test]
    public void MultiplyScalar() => Assert.That(Matrix4x4Math<int>.Multiply(_int_a, _int_scalar), Is.EqualTo(_exp_scalar_multiplication));

    [Test]
    public void DivideScalar() => Assert.That(Matrix4x4Math<double>.Divide(_double_a, _double_scalar), Is.EqualTo(_exp_scalar_division));

    [Test]
    public void MultiplyEntrywise() => Assert.That(Matrix4x4Math<int>.MultiplyEntrywise(_int_a, _int_b), Is.EqualTo(_exp_entrywise_multiplication));

    [Test]
    public void DivideEntrywise() => Assert.That(Matrix4x4Math<double>.DivideEntrywise(_double_a, _double_b), Is.EqualTo(_exp_entrywise_division));

    [Test]
    public void MultiplyMatrix() => Assert.That(Matrix4x4Math<int>.Multiply(_int_a, _int_b), Is.EqualTo(_exp_matrix_multiplication));
}