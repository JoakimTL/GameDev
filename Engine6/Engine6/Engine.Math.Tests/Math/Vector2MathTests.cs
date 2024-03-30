using Engine.Math.Calculation;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Vector2MathTests : ILinearMathTests, IEntrywiseProductTests, IMatrixMultiplicationProductTests {

	private Vector2<int> _int_a;
	private Vector2<int> _int_b;
	private int _int_scalar;
	private Matrix2x2<int> _int_matrix;
	private Vector2<double> _double_a;
	private Vector2<double> _double_b;
	private double _double_scalar;

	private Vector2<int> _exp_negation;
	private Vector2<int> _exp_addition;
	private Vector2<int> _exp_subtraction;
	private Vector2<int> _exp_scalar_multiplication;
	private Vector2<double> _exp_scalar_division;
	private Vector2<int> _exp_entrywise_multiplication;
	private Vector2<double> _exp_entrywise_division;
	private Vector2<int> _exp_matrix_multiplication;

	[SetUp]
	public void Setup() {
		_int_a = new( 1, 2 );
		_int_b = new( 3, 4 );
		_int_scalar = 2;
		_int_matrix = new( 1, 2, 3, 4 );
		_double_a = new( 1, 2 );
		_double_b = new( 3, 4 );
		_double_scalar = 2;

		_exp_negation = new( -1, -2 );
		_exp_addition = new( 4, 6 );
		_exp_subtraction = new( -2, -2 );
		_exp_scalar_multiplication = new( 2, 4 );
		_exp_scalar_division = new( 1 / _double_scalar, 2 / _double_scalar );
		_exp_entrywise_multiplication = new( 3, 8 );
		_exp_entrywise_division = new( 1d / 3, 2d / 4 );
		_exp_matrix_multiplication = new( 7, 10 );
	}

	[Test]
	public void Negate() => Assert.That( Vector2Math<int>.Negate( _int_a ), Is.EqualTo( _exp_negation ) );

	[Test]
	public void Add() => Assert.That( Vector2Math<int>.Add( _int_a, _int_b ), Is.EqualTo( _exp_addition ) );

	[Test]
	public void Subtract() => Assert.That( Vector2Math<int>.Subtract( _int_a, _int_b ), Is.EqualTo( _exp_subtraction ) );

	[Test]
	public void MultiplyScalar() => Assert.That( Vector2Math<int>.Multiply( _int_a, _int_scalar ), Is.EqualTo( _exp_scalar_multiplication ) );

	[Test]
	public void DivideScalar() => Assert.That( Vector2Math<double>.Divide( _double_a, _double_scalar ), Is.EqualTo( _exp_scalar_division ) );

	[Test]
	public void MultiplyEntrywise() => Assert.That( Vector2Math<int>.MultiplyEntrywise( _int_a, _int_b ), Is.EqualTo( _exp_entrywise_multiplication ) );

	[Test]
	public void DivideEntrywise() => Assert.That( Vector2Math<double>.DivideEntrywise( _double_a, _double_b ), Is.EqualTo( _exp_entrywise_division ) );

	[Test]
	public void MultiplyMatrix() => Assert.That( Vector2Math<int>.Multiply( _int_a, _int_matrix ), Is.EqualTo( _exp_matrix_multiplication ) );
}