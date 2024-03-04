using Engine.Math.Math;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Matrix3x3MathTests {

	[Test]
	public void Negate() {
		Matrix3x3<int> matrix = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> expected = new
		(
		-1, -2, -3,
		-4, -5, -6,
		-7, -8, -9
		);

		Assert.That( Matrix3x3Math<int>.Negate( matrix ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Add() {
		Matrix3x3<int> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> r = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> expected = new
		(
		2, 4, 6,
		8, 10, 12,
		14, 16, 18
		);

		Assert.That( Matrix3x3Math<int>.Add( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Subtract() {
		Matrix3x3<int> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> r = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> expected = new
		(
		0, 0, 0,
		0, 0, 0,
		0, 0, 0
		);

		Assert.That( Matrix3x3Math<int>.Subtract( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Matrix3x3<int> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> r = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> expected = new
		(
		1, 4, 9,
		16, 25, 36,
		49, 64, 81
		);

		Assert.That( Matrix3x3Math<int>.MultiplyEntrywise( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideEntrywise_Double() {
		Matrix3x3<double> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<double> r = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<double> expected = new
		(
		1, 1, 1,
		1, 1, 1,
		1, 1, 1
		);

		Assert.That( Matrix3x3Math<double>.DivideEntrywise( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void MultiplyScalar() {
		Matrix3x3<int> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		int r = 2;

		Matrix3x3<int> expected = new
		(
		2, 4, 6,
		8, 10, 12,
		14, 16, 18
		);

		Assert.That( Matrix3x3Math<int>.Multiply( l, r ), Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalar() {
		Matrix3x3<int> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		int r = 2;

		Matrix3x3<int> expected = new
		(
		0, 1, 1,
		2, 2, 3,
		3, 4, 4
		);

		Assert.That( Matrix3x3Math<int>.Divide( l, r ), Is.EqualTo( expected ) );
	}


	[Test]
	public void Multiply() {
		Matrix3x3<int> l = new
		(
		1, 2, 3,
		4, 5, 6,
		7, 8, 9
		);

		Matrix3x3<int> r = new
		(
		10, 11, 12,
		13, 14, 15,
		16, 17, 18
		);

		Matrix3x3<int> expected = new
		(
		84, 90, 96,
		201, 216, 231,
		318, 342, 366
		);

		Assert.That( Matrix3x3Math<int>.Multiply( l, r ), Is.EqualTo( expected ) );
	}
}
