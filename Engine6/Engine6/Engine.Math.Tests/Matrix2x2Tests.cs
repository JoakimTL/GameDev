namespace Engine.Math.Tests;

[TestFixture]
public class Matrix2x2Tests {

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
