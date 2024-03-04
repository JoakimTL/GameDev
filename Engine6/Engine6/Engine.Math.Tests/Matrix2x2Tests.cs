using Engine.Math.Math;

namespace Engine.Math.Tests;

[TestFixture]
public class Matrix2x2Tests {

	[Test]
	public void Multiplication() {
		Matrix2x2<int> a = new( 1, 2, 3, 4 );
		Matrix2x2<int> b = new( 5, 6, 7, 8 );

		Matrix2x2<int> c = Matrix2x2Math<int>.Multiply( a, b );

		Assert.That( c.M00, Is.EqualTo( 19 ) );
		Assert.That( c.M01, Is.EqualTo( 22 ) );
		Assert.That( c.M10, Is.EqualTo( 43 ) );
		Assert.That( c.M11, Is.EqualTo( 50 ) );
	}

	[Test]
	public void RowsAndColumns() {
		Matrix2x2<int> matrix = new
		(
		1, 2,
		3, 4
		);

		Assert.That( matrix.Row0, Is.EqualTo( new Vector2<int>( 1, 2 ) ) );
		Assert.That( matrix.Row1, Is.EqualTo( new Vector2<int>( 3, 4 ) ) );
		Assert.That( matrix.Col0, Is.EqualTo( new Vector2<int>( 1, 3 ) ) );
		Assert.That( matrix.Col1, Is.EqualTo( new Vector2<int>( 2, 4 ) ) );
	}

}
