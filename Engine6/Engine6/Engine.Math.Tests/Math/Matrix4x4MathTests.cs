using Engine.Math.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Math.Tests.Math;

[TestFixture]
public sealed class Matrix4x4MathTests {

	[Test]
	public void Negate() {
		Matrix4x4<int> matrix = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Assert.That( Matrix4x4Math<int>.Negate( matrix ), Is.EqualTo( new Matrix4x4<int>( -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15, -16 ) ) );
	}

	[Test]
	public void Add() {
		Matrix4x4<int> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Matrix4x4<int> r = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Assert.That( Matrix4x4Math<int>.Add( l, r ), Is.EqualTo( new Matrix4x4<int>( 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32 ) ) );
	}

	[Test]
	public void Subtract() {
		Matrix4x4<int> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Matrix4x4<int> r = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Assert.That( Matrix4x4Math<int>.Subtract( l, r ), Is.EqualTo( new Matrix4x4<int>() ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Matrix4x4<int> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Matrix4x4<int> r = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Assert.That( Matrix4x4Math<int>.MultiplyEntrywise( l, r ), Is.EqualTo( new Matrix4x4<int>( 1, 4, 9, 16, 25, 36, 49, 64, 81, 100, 121, 144, 169, 196, 225, 256 ) ) );
	}

	[Test]
	public void DivideEntrywise_Double() {
		Matrix4x4<double> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Matrix4x4<double> r = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Assert.That( Matrix4x4Math<double>.DivideEntrywise( l, r ), Is.EqualTo( new Matrix4x4<double>( 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ) ) );
	}

	[Test]
	public void MultiplyScalar() {
		Matrix4x4<int> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		int r = 2;

		Assert.That( Matrix4x4Math<int>.Multiply( l, r ), Is.EqualTo( new Matrix4x4<int>( 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32 ) ) );
	}

	[Test]
	public void DivideScalar() {
		Matrix4x4<int> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		int r = 2;

		Assert.That( Matrix4x4Math<int>.Divide( l, r ), Is.EqualTo( new Matrix4x4<int>( 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8 ) ) );
	}


	[Test]
	public void Multiply() {
		Matrix4x4<int> l = new
		(
		1, 2, 3, 4,
		5, 6, 7, 8,
		9, 10, 11, 12,
		13, 14, 15, 16
		);

		Matrix4x4<int> r = new
		(
		17, 18, 19, 20,
		21, 22, 23, 24,
		25, 26, 27, 28,
		29, 30, 31, 32
		);

		Assert.That( Matrix4x4Math<int>.Multiply( l, r ), Is.EqualTo( new Matrix4x4<int>( 250, 260, 270, 280, 618, 644, 670, 696, 986, 1028, 1070, 1112, 1354, 1412, 1470, 1528 ) ) );
	}

	//[Test]
	//public void GetDeterminantByExpansionOfMinors() {
	//	Matrix4x4<int> matrix = new
	//	(
	//	1, 2, 3, 4,
	//	5, 6, 7, 8,
	//	9, 10, 11, 12,
	//	13, 14, 15, 16
	//	);

	//	Assert.That( matrix.GetDeterminantByExpansionOfMinors(), Is.EqualTo( 0 ) );
	//}

}
