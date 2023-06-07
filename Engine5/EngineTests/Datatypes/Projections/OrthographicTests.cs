using Engine.Datatypes.Projections;
using System.Numerics;

namespace EngineTests.Datatypes.Projections;

[TestFixture]
public class OrthographicTests {
	[Test]
	public void Orthographic_DefaultConstructor_MatrixIsCorrect() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		Matrix4x4 reference = Matrix4x4.CreateOrthographic( 1, 1, -1, 1 );
		Assert.That( orthographic.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Orthographic_AfterChangingSize_MatrixIsCorrect() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		orthographic.Size = new( 2, 2 );
		Matrix4x4 reference = Matrix4x4.CreateOrthographic( 2, 2, -1, 1 );
		Assert.That( orthographic.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Orthographic_AfterChangingZNear_MatrixIsCorrect() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		orthographic.ZNear = 0;
		Matrix4x4 reference = Matrix4x4.CreateOrthographic( 1, 1, 0, 1 );
		Assert.That( orthographic.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Orthographic_AfterChangingZFar_MatrixIsCorrect() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		orthographic.ZFar = 2;
		Matrix4x4 reference = Matrix4x4.CreateOrthographic( 1, 1, -1, 2 );
		Assert.That( orthographic.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Orthographic_SizeChanged_NoChangeToSize() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		orthographic.Size = new( 0, 0 );
		Assert.That( orthographic.Size, Is.EqualTo( new Vector2( 1, 1 ) ) );
	}

	[Test]
	public void Orthographic_ZNearChanged_NoChangeToZNear() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		orthographic.ZNear = 2;
		Assert.That( orthographic.ZNear, Is.EqualTo( -1 ) );
	}

	[Test]
	public void Orthographic_ZFarChanged_NoChangeToZFar() {
		Orthographic orthographic = new( new( 1, 1 ), -1, 1 );
		orthographic.ZFar = -2;
		Assert.That( orthographic.ZFar, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Orthographic_SizeIsZero_ThrowsException() {
		Assert.Throws<ArgumentOutOfRangeException>( () => new Orthographic( new Vector2( 0, 0 ), -1, 1 ) );
	}

	[Test]
	public void Orthographic_SizeIsNegative_ThrowsException() {
		Assert.Throws<ArgumentOutOfRangeException>( () => new Orthographic( new Vector2( -1, -1 ), -1, 1 ) );
	}

	[Test]
	public void Orthographic_ZFarIsNegative_ThrowsException() {
		Assert.Throws<ArgumentOutOfRangeException>( () => new Orthographic( new Vector2( 1, 1 ), -1, -1 ) );
	}

	[Test]
	public void Orthographic_ZFarIsLessThanZNear_ThrowsException() {
		Assert.Throws<ArgumentOutOfRangeException>( () => new Orthographic( new Vector2( 1, 1 ), 1, -1 ) );
	}

	[Test]
	public void Orthographic_ZFarIsEqualToZNear_ThrowsException() {
		Assert.Throws<ArgumentOutOfRangeException>( () => new Orthographic( new Vector2( 1, 1 ), 1, 1 ) );
	}

}
