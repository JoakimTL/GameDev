using Engine.Math.Operations;

namespace Engine.Math.Tests;

[TestFixture]
public sealed class Matrix3x3OpsTests {

	[Test]
	public void GetTransposed() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Matrix3x3<int> transpose = matrix.GetTransposed();

		Assert.That( transpose, Is.EqualTo( new Matrix3x3<int>(
			1, 4, 7,
			2, 5, 8,
			3, 6, 9
		) ) );
	}

	[Test]
	public void TryFillRowMajor() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> stack = stackalloc int[ 9 ];

		bool success = matrix.TryFillRowMajor( stack );

		Assert.That( success, Is.True );
		Assert.That( stack[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 1 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 2 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 4 ) );
		Assert.That( stack[ 4 ], Is.EqualTo( 5 ) );
		Assert.That( stack[ 5 ], Is.EqualTo( 6 ) );
		Assert.That( stack[ 6 ], Is.EqualTo( 7 ) );
		Assert.That( stack[ 7 ], Is.EqualTo( 8 ) );
		Assert.That( stack[ 8 ], Is.EqualTo( 9 ) );
	}

	[Test]
	public void TryFillRowMajorOffset() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> stack = stackalloc int[ 12 ];

		bool success = matrix.TryFillRowMajor( stack, 8 );

		Assert.That( success, Is.True );
		Assert.That( stack[ 2 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 4 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 5 ], Is.EqualTo( 4 ) );
		Assert.That( stack[ 6 ], Is.EqualTo( 5 ) );
		Assert.That( stack[ 7 ], Is.EqualTo( 6 ) );
		Assert.That( stack[ 8 ], Is.EqualTo( 7 ) );
		Assert.That( stack[ 9 ], Is.EqualTo( 8 ) );
		Assert.That( stack[ 10 ], Is.EqualTo( 9 ) );
	}

	[Test]
	public void TryFillRowMajorOffsetFails() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> stack = stackalloc int[ 12 ];

		bool success = matrix.TryFillRowMajor( stack, 16 );

		Assert.That( success, Is.False );
	}

	[Test]
	public void TryFillColumnMajor() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> stack = stackalloc int[ 9 ];

		bool success = matrix.TryFillColumnMajor( stack );

		Assert.That( success, Is.True );
		Assert.That( stack[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 1 ], Is.EqualTo( 4 ) );
		Assert.That( stack[ 2 ], Is.EqualTo( 7 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 4 ], Is.EqualTo( 5 ) );
		Assert.That( stack[ 5 ], Is.EqualTo( 8 ) );
		Assert.That( stack[ 6 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 7 ], Is.EqualTo( 6 ) );
		Assert.That( stack[ 8 ], Is.EqualTo( 9 ) );
	}

	[Test]
	public void TryFillColumnMajorOffset() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> stack = stackalloc int[ 12 ];

		bool success = matrix.TryFillColumnMajor( stack, 8 );

		Assert.That( success, Is.True );
		Assert.That( stack[ 2 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 4 ) );
		Assert.That( stack[ 4 ], Is.EqualTo( 7 ) );
		Assert.That( stack[ 5 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 6 ], Is.EqualTo( 5 ) );
		Assert.That( stack[ 7 ], Is.EqualTo( 8 ) );
		Assert.That( stack[ 8 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 9 ], Is.EqualTo( 6 ) );
		Assert.That( stack[ 10 ], Is.EqualTo( 9 ) );
	}

	[Test]
	public void TryFillColumnMajorOffsetFails() {
		Matrix3x3<int> matrix = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> stack = stackalloc int[ 12 ];

		bool success = matrix.TryFillColumnMajor( stack, 16 );

		Assert.That( success, Is.False );
	}
}
