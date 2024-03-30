using Engine.Math.Calculation;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Vector4MathTests : ILinearMathTests, IEntrywiseProductTests, IMatrixMultiplicationProductTests {

	private Vector4<int> _int_a;
	private Vector4<int> _int_b;
	private int _int_scalar;
	private Matrix4x4<int> _int_matrix;

	private Vector4<double> _double_a;
	private Vector4<double> _double_b;
	private double _double_scalar;

	private Vector4<int> _exp_negation;
	private Vector4<int> _exp_addition;
	private Vector4<int> _exp_subtraction;
	private Vector4<int> _exp_scalar_multiplication;
	private Vector4<double> _exp_scalar_division;
	private Vector4<int> _exp_entrywise_multiplication;
	private Vector4<double> _exp_entrywise_division;
	private Vector4<int> _exp_matrix_multiplication;

	[SetUp]
	public void Setup() {
		_int_a = new( 1, 2, 3, 4 );
		_int_b = new( 5, 6, 7, 8 );
		_int_scalar = 2;
		_int_matrix = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );
		_double_a = new( 1, 2, 3, 4 );
		_double_b = new( 5, 6, 7, 8 );
		_double_scalar = 2;

		_exp_negation = new( -1, -2, -3, -4 );
		_exp_addition = new( 6, 8, 10, 12 );
		_exp_subtraction = new( -4, -4, -4, -4 );
		_exp_scalar_multiplication = new( 2, 4, 6, 8 );
		_exp_scalar_division = new( 1 / _double_scalar, 2 / _double_scalar, 3 / _double_scalar, 4 / _double_scalar );
		_exp_entrywise_multiplication = new( 5, 12, 21, 32 );
		_exp_entrywise_division = new( 1d / 5, 2d / 6, 3d / 7, 4d / 8 );
		_exp_matrix_multiplication = new( 90, 100, 110, 120 );
	}

	[Test]
	public void Negate() => Assert.That( Vector4Math<int>.Negate( _int_a ), Is.EqualTo( _exp_negation ) );

	[Test]
	public void Add() => Assert.That( Vector4Math<int>.Add( _int_a, _int_b ), Is.EqualTo( _exp_addition ) );

	[Test]
	public void Subtract() => Assert.That( Vector4Math<int>.Subtract( _int_a, _int_b ), Is.EqualTo( _exp_subtraction ) );

	[Test]
	public void MultiplyScalar() => Assert.That( Vector4Math<int>.Multiply( _int_a, _int_scalar ), Is.EqualTo( _exp_scalar_multiplication ) );

	[Test]
	public void DivideScalar() => Assert.That( Vector4Math<double>.Divide( _double_a, _double_scalar ), Is.EqualTo( _exp_scalar_division ) );

	[Test]
	public void MultiplyEntrywise() => Assert.That( Vector4Math<int>.MultiplyEntrywise( _int_a, _int_b ), Is.EqualTo( _exp_entrywise_multiplication ) );

	[Test]
	public void DivideEntrywise() => Assert.That( Vector4Math<double>.DivideEntrywise( _double_a, _double_b ), Is.EqualTo( _exp_entrywise_division ) );

	[Test]
	public void MultiplyMatrix() => Assert.That( Vector4Math<int>.Multiply( _int_a, _int_matrix ), Is.EqualTo( _exp_matrix_multiplication ) );
}
