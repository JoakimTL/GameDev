using Engine.Math.Calculation;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Matrix3x3MathTests : ILinearMathTests, IEntrywiseProductTests, IMatrixMultiplicationProductTests {

	private Matrix3x3<int> _int_a;
	private Matrix3x3<int> _int_b;
	private int _int_scalar;
	private Matrix3x3<double> _double_a;
	private Matrix3x3<double> _double_b;
	private double _double_scalar;

	private Matrix3x3<int> _exp_negation;
	private Matrix3x3<int> _exp_addition;
	private Matrix3x3<int> _exp_subtraction;
	private Matrix3x3<int> _exp_scalar_multiplication;
	private Matrix3x3<double> _exp_scalar_division;
	private Matrix3x3<int> _exp_entrywise_multiplication;
	private Matrix3x3<double> _exp_entrywise_division;
	private Matrix3x3<int> _exp_matrix_multiplication;

	[SetUp]
	public void Setup() {
		_int_a = new( 1, 2, 3, 4, 5, 6, 7, 8, 9 );
		_int_b = new( 10, 11, 12, 13, 14, 15, 16, 17, 18 );
		_int_scalar = 2;
		_double_a = new( 1, 2, 3, 4, 5, 6, 7, 8, 9 );
		_double_b = new( 10, 11, 12, 13, 14, 15, 16, 17, 18 );
		_double_scalar = 2;

		_exp_negation = new( -1, -2, -3, -4, -5, -6, -7, -8, -9 );
		_exp_addition = new( 11, 13, 15, 17, 19, 21, 23, 25, 27 );
		_exp_subtraction = new( -9, -9, -9, -9, -9, -9, -9, -9, -9 );
		_exp_scalar_multiplication = new( 2, 4, 6, 8, 10, 12, 14, 16, 18 );
		_exp_scalar_division = new( 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5 );
		_exp_entrywise_multiplication = new( 10, 22, 36, 52, 70, 90, 112, 136, 162 );
		_exp_entrywise_division = new( 1d / 10, 2d / 11, 3d / 12, 4d / 13, 5d / 14, 6d / 15, 7d / 16, 8d / 17, 9d / 18 );
		_exp_matrix_multiplication = new( 84, 90, 96, 201, 216, 231, 318, 342, 366 );
	}

	[Test]
	public void Negate() => Assert.That( Matrix3x3Math<int>.Negate( _int_a ), Is.EqualTo( _exp_negation ) );

	[Test]
	public void Add() => Assert.That( Matrix3x3Math<int>.Add( _int_a, _int_b ), Is.EqualTo( _exp_addition ) );

	[Test]
	public void Subtract() => Assert.That( Matrix3x3Math<int>.Subtract( _int_a, _int_b ), Is.EqualTo( _exp_subtraction ) );

	[Test]
	public void MultiplyScalar() => Assert.That( Matrix3x3Math<int>.Multiply( _int_a, _int_scalar ), Is.EqualTo( _exp_scalar_multiplication ) );

	[Test]
	public void DivideScalar() => Assert.That( Matrix3x3Math<double>.Divide( _double_a, _double_scalar ), Is.EqualTo( _exp_scalar_division ) );

	[Test]
	public void MultiplyEntrywise() => Assert.That( Matrix3x3Math<int>.MultiplyEntrywise( _int_a, _int_b ), Is.EqualTo( _exp_entrywise_multiplication ) );

	[Test]
	public void DivideEntrywise() => Assert.That( Matrix3x3Math<double>.DivideEntrywise( _double_a, _double_b ), Is.EqualTo( _exp_entrywise_division ) );

	[Test]
	public void MultiplyMatrix() => Assert.That( Matrix3x3Math<int>.Multiply( _int_a, _int_b ), Is.EqualTo( _exp_matrix_multiplication ) );
}
