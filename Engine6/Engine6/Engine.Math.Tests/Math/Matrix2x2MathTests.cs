using Engine.Math.Calculation;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Matrix2x2MathTests : ILinearMathTests, IEntrywiseProductTests, IMatrixMultiplicationProductTests {

	private Matrix2x2<int> _int_a;
	private Matrix2x2<int> _int_b;
	private int _int_scalar;
	private Matrix2x2<double> _double_a;
	private Matrix2x2<double> _double_b;
	private double _double_scalar;

	private Matrix2x2<int> _exp_negation;
	private Matrix2x2<int> _exp_addition;
	private Matrix2x2<int> _exp_subtraction;
	private Matrix2x2<int> _exp_scalar_multiplication;
	private Matrix2x2<double> _exp_scalar_division;
	private Matrix2x2<int> _exp_entrywise_multiplication;
	private Matrix2x2<double> _exp_entrywise_division;
	private Matrix2x2<int> _exp_matrix_multiplication;

	[SetUp]
	public void Setup() {
		_int_a = new( 1, 2, 3, 4 );
		_int_b = new( 5, 6, 7, 8 );
		_int_scalar = 2;
		_double_a = new( 1, 2, 3, 4 );
		_double_b = new( 5, 6, 7, 8 );
		_double_scalar = 2;

		_exp_negation = new( -1, -2, -3, -4 );
		_exp_addition = new( 6, 8, 10, 12 );
		_exp_subtraction = new( -4, -4, -4, -4 );
		_exp_scalar_multiplication = new( 2, 4, 6, 8 );
		_exp_scalar_division = new( 1d / 2, 2d / 2, 3d / 2, 4d / 2 );
		_exp_entrywise_multiplication = new( 5, 12, 21, 32 );
		_exp_entrywise_division = new( 1d / 5, 2d / 6, 3d / 7, 4d / 8 );
		_exp_matrix_multiplication = new( 19, 22, 43, 50 );
	}

	[Test]
	public void Negate() => Assert.That( Matrix2x2Math<int>.Negate( _int_a ), Is.EqualTo( _exp_negation ) );

	[Test]
	public void Add() => Assert.That( Matrix2x2Math<int>.Add( _int_a, _int_b ), Is.EqualTo( _exp_addition ) );

	[Test]
	public void Subtract() => Assert.That( Matrix2x2Math<int>.Subtract( _int_a, _int_b ), Is.EqualTo( _exp_subtraction ) );

	[Test]
	public void MultiplyScalar() => Assert.That( Matrix2x2Math<int>.Multiply( _int_a, _int_scalar ), Is.EqualTo( _exp_scalar_multiplication ) );

	[Test]
	public void DivideScalar() => Assert.That( Matrix2x2Math<double>.Divide( _double_a, _double_scalar ), Is.EqualTo( _exp_scalar_division ) );

	[Test]
	public void MultiplyEntrywise() => Assert.That( Matrix2x2Math<int>.MultiplyEntrywise( _int_a, _int_b ), Is.EqualTo( _exp_entrywise_multiplication ) );

	[Test]
	public void DivideEntrywise() => Assert.That( Matrix2x2Math<double>.DivideEntrywise( _double_a, _double_b ), Is.EqualTo( _exp_entrywise_division ) );

	[Test]
	public void MultiplyMatrix() => Assert.That( Matrix2x2Math<int>.Multiply( _int_a, _int_b ), Is.EqualTo( _exp_matrix_multiplication ) );
}