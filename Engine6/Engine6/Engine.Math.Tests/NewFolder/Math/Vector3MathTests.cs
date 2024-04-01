using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Calculation;

namespace Engine.Math.Tests.NewFolder.Math;

[TestFixture]
public sealed class Vector3MathTests : ILinearMathTests, IEntrywiseProductTests, IMatrixMultiplicationProductTests
{

    private Vector3<int> _int_a;
    private Vector3<int> _int_b;
    private int _int_scalar;
    private Matrix3x3<int> _int_matrix;

    private Vector3<double> _double_a;
    private Vector3<double> _double_b;
    private double _double_scalar;

    private Vector3<int> _exp_negation;
    private Vector3<int> _exp_addition;
    private Vector3<int> _exp_subtraction;
    private Vector3<int> _exp_scalar_multiplication;
    private Vector3<double> _exp_scalar_division;
    private Vector3<int> _exp_entrywise_multiplication;
    private Vector3<double> _exp_entrywise_division;
    private Vector3<int> _exp_matrix_multiplication;

    [SetUp]
    public void Setup()
    {
        _int_a = new(1, 2, 3);
        _int_b = new(4, 5, 6);
        _int_scalar = 2;
        _int_matrix = new(1, 2, 3, 4, 5, 6, 7, 8, 9);
        _double_a = new(1, 2, 3);
        _double_b = new(4, 5, 6);
        _double_scalar = 2;

        _exp_negation = new(-1, -2, -3);
        _exp_addition = new(5, 7, 9);
        _exp_subtraction = new(-3, -3, -3);
        _exp_scalar_multiplication = new(2, 4, 6);
        _exp_scalar_division = new(1 / _double_scalar, 2 / _double_scalar, 3 / _double_scalar);
        _exp_entrywise_multiplication = new(4, 10, 18);
        _exp_entrywise_division = new(1d / 4, 2d / 5, 3d / 6);
        _exp_matrix_multiplication = new(30, 36, 42);
    }

    [Test]
    public void Negate() => Assert.That(Vector3Math<int>.Negate(_int_a), Is.EqualTo(_exp_negation));

    [Test]
    public void Add() => Assert.That(Vector3Math<int>.Add(_int_a, _int_b), Is.EqualTo(_exp_addition));

    [Test]
    public void Subtract() => Assert.That(Vector3Math<int>.Subtract(_int_a, _int_b), Is.EqualTo(_exp_subtraction));

    [Test]
    public void MultiplyScalar() => Assert.That(Vector3Math<int>.Multiply(_int_a, _int_scalar), Is.EqualTo(_exp_scalar_multiplication));

    [Test]
    public void DivideScalar() => Assert.That(Vector3Math<double>.Divide(_double_a, _double_scalar), Is.EqualTo(_exp_scalar_division));

    [Test]
    public void MultiplyEntrywise() => Assert.That(Vector3Math<int>.MultiplyEntrywise(_int_a, _int_b), Is.EqualTo(_exp_entrywise_multiplication));

    [Test]
    public void DivideEntrywise() => Assert.That(Vector3Math<double>.DivideEntrywise(_double_a, _double_b), Is.EqualTo(_exp_entrywise_division));

    [Test]
    public void MultiplyMatrix() => Assert.That(Vector3Math<int>.Multiply(_int_a, _int_matrix), Is.EqualTo(_exp_matrix_multiplication));
}
