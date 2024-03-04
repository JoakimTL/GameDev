using Engine.Math.Math;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Matrix2x2MathTests {

	[Test]
	public void Negate() {
		Matrix2x2<int> matrix = new
		(
		1, 2,
		3, 4
		);

		Matrix2x2<int> expected = new
		(
		-1, -2,
		-3, -4
		);

		Assert.That( Matrix2x2Math<int>.Negate( matrix ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Add() {
		Matrix2x2<int> l = new
		(
		1, 2,
		3, 4
		);

		Matrix2x2<int> r = new
		(
		5, 6,
		7, 8
		);

		Matrix2x2<int> expected = new
		(
		6, 8,
		10, 12
		);

		Assert.That( Matrix2x2Math<int>.Add( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Subtract() {
		Matrix2x2<int> l = new
		(
		1, 2,
		3, 4
		);

		Matrix2x2<int> r = new
		(
		5, 6,
		7, 8
		);

		Matrix2x2<int> expected = new
		(
		-4, -4,
		-4, -4
		);

		Assert.That( Matrix2x2Math<int>.Subtract( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Divide_Double() {
		Matrix2x2<double> l = new
		(
		1, 2,
		3, 4
		);

		double r = 2;

		Matrix2x2<double> expected = new
		(
		1 / r, 2 / r,
		3 / r, 4 / r
		);

		Assert.That( Matrix2x2Math<double>.Divide( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Multiply() {
		Matrix2x2<int> l = new
		(
		1, 2,
		3, 4
		);

		int r = 2;

		Matrix2x2<int> expected = new
		(
		1 * r, 2 * r,
		3 * r, 4 * r
		);

		Assert.That( Matrix2x2Math<int>.Multiply( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Matrix2x2<int> l = new
		(
		1, 2,
		3, 4
		);

		Matrix2x2<int> r = new
		(
		5, 6,
		7, 8
		);

		Matrix2x2<int> expected = new
		(
		1 * 5, 2 * 6,
		3 * 7, 4 * 8
		);

		Assert.That( Matrix2x2Math<int>.MultiplyEntrywise( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideEntrywise_Double() {
		Matrix2x2<double> l = new
		(
		1, 2,
		3, 4
		);

		Matrix2x2<double> r = new
		(
		5, 6,
		7, 8
		);

		Matrix2x2<double> expected = new
		(
		1 / 5d, 2 / 6d,
		3 / 7d, 4 / 8d
		);

		Assert.That( Matrix2x2Math<double>.DivideEntrywise( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Multiply_Matrix2x2() {
		Matrix2x2<int> l = new
		(
		1, 2,
		3, 4
		);

		Matrix2x2<int> r = new
		(
		5, 6,
		7, 8
		);

		Matrix2x2<int> expected = new
		(
		1 * 5 + 2 * 7, 1 * 6 + 2 * 8,
		3 * 5 + 4 * 7, 3 * 6 + 4 * 8
		);

		Assert.That( Matrix2x2Math<int>.Multiply( l, r ), Is.EqualTo( expected ) );
	}


}