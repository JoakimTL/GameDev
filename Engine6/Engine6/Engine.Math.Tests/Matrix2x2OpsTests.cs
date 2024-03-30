using Engine.Math.Operations;

namespace Engine.Math.Tests;

[TestFixture]
public sealed class Matrix2x2OpsTests {

	[Test]
	public void GetTransposed() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Matrix2x2<int> transpose = matrix.GetTransposed();

		Assert.That( transpose, Is.EqualTo( new Matrix2x2<int>( 
			1, 3, 
			2, 4 
		) ) );
	}

	[Test]
	public void TryFillRowMajor() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Span<int> stack = stackalloc int[ 4 ];

		bool success = matrix.TryFillRowMajor( stack );

		Assert.That( success, Is.True );
		Assert.That( stack[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 1 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 2 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 4 ) );
	}

	[Test]
	public void TryFillRowMajorOffset() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Span<int> stack = stackalloc int[ 6 ];

		bool success = matrix.TryFillRowMajor( stack, 4 );

		Assert.That( success, Is.True );
		Assert.That( stack[ 1 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 2 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 4 ], Is.EqualTo( 4 ) );
	}

	[Test]
	public void TryFillRowMajorOffsetFails() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Span<int> stack = stackalloc int[ 6 ];

		bool success = matrix.TryFillRowMajor( stack, 12 );

		Assert.That( success, Is.False );
	}

	[Test]
	public void TryFillColumnMajor() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Span<int> stack = stackalloc int[ 4 ];

		bool success = matrix.TryFillColumnMajor( stack );

		Assert.That( success, Is.True );
		Assert.That( stack[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 1 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 2 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 4 ) );
	}

	[Test]
	public void TryFillColumnMajorOffset() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Span<int> stack = stackalloc int[ 6 ];

		bool success = matrix.TryFillColumnMajor( stack, 4 );

		Assert.That( success, Is.True );
		Assert.That( stack[ 1 ], Is.EqualTo( 1 ) );
		Assert.That( stack[ 2 ], Is.EqualTo( 3 ) );
		Assert.That( stack[ 3 ], Is.EqualTo( 2 ) );
		Assert.That( stack[ 4 ], Is.EqualTo( 4 ) );
	}

	[Test]
	public void TryFillColumnMajorOffsetFails() {
		Matrix2x2<int> matrix = new(
			1, 2,
			3, 4
		);

		Span<int> stack = stackalloc int[ 6 ];

		bool success = matrix.TryFillColumnMajor( stack, 12 );

		Assert.That( success, Is.False );
	}
}
