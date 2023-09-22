using Engine.Datatypes.Projections;
using System.Numerics;

namespace EngineTests.Datatypes.Projections;

[TestFixture]
public class PerspectiveTests {

	[Test]
	public void Perspective_AfterConstruction_MatrixIsCorrect() {
		Perspective perspective = new( 90, 1 );
		Matrix4x4 reference = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 2, 1, Perspective.DEFAULT_NEAR, Perspective.DEFAULT_FAR );
		Assert.That( perspective.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Perspective_AfterChangingFOV_MatrixIsCorrect() {
		Perspective perspective = new( 90, 1 );
		perspective.FOV = 45;
		Matrix4x4 reference = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 4, 1, Perspective.DEFAULT_NEAR, Perspective.DEFAULT_FAR );
		Assert.That( perspective.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Perspective_AfterChangingAspectRatio_MatrixIsCorrect() {
		Perspective perspective = new( 90, 1 );
		perspective.AspectRatio = 2;
		Matrix4x4 reference = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 2, 2, Perspective.DEFAULT_NEAR, Perspective.DEFAULT_FAR );
		Assert.That( perspective.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Perspective_AfterChangingZNear_MatrixIsCorrect() {
		Perspective perspective = new( 90, 1 );
		perspective.ZNear = 2;
		Matrix4x4 reference = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 2, 1, 2, Perspective.DEFAULT_FAR );
		Assert.That( perspective.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Perspective_AfterChangingZFar_MatrixIsCorrect() {
		Perspective perspective = new( 90, 1 );
		perspective.ZFar = 2;
		Matrix4x4 reference = Matrix4x4.CreatePerspectiveFieldOfView( MathF.PI / 2, 1, Perspective.DEFAULT_NEAR, 2 );
		Assert.That( perspective.Matrix, Is.EqualTo( reference ) );
	}

	[Test]
	public void Perspective_FOVChangeUnder180Degrees_ChangeToFOV() {
		Perspective perspective = new( 90, 1 );
		perspective.FOV = 45;
		Assert.That( perspective.FOV, Is.EqualTo( 45 ) );
	}

	[Test]

	public void Perspective_AspectRatioChange_ChangeToAspectRatio() {
		Perspective perspective = new( 90, 1 );
		perspective.AspectRatio = 2;
		Assert.That( perspective.AspectRatio, Is.EqualTo( 2 ) );
	}

	[Test]
	public void Perspective_ZNearChange_ChangeToZNear() {
		Perspective perspective = new( 90, 1 );
		perspective.ZNear = 2;
		Assert.That( perspective.ZNear, Is.EqualTo( 2 ) );
	}

	[Test]
	public void Perspective_ZFarChange_ChangeToZFar() {
		Perspective perspective = new( 90, 1 );
		perspective.ZFar = 2;
		Assert.That( perspective.ZFar, Is.EqualTo( 2 ) );
	}

	[Test]
	public void Perspective_FOVChangeOver180Degrees_NoChangeToFOV() {
		Perspective perspective = new( 90, 1 );
		perspective.FOV = 270;
		Assert.That( perspective.FOV, Is.EqualTo( 90 ) );
	}

	[Test]
	public void Perspective_AspectRatioChangeToZero_NoChangeToAspectRatio() {
		Perspective perspective = new( 90, 1 );
		perspective.AspectRatio = 0;
		Assert.That( perspective.AspectRatio, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Perspective_AspectRatioChangeToNegative_NoChangeToAspectRatio() {
		Perspective perspective = new( 90, 1 );
		perspective.AspectRatio = -1;
		Assert.That( perspective.AspectRatio, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Perspective_ZNearChangeToZFar_NoChangeToZNear() {
		Perspective perspective = new( 90, 1 );
		perspective.ZNear = perspective.ZFar;
		Assert.That( perspective.ZNear, Is.EqualTo( Perspective.DEFAULT_NEAR ) );
	}

	[Test]
	public void Perspective_ZNearChangeToOverZFar_NoChangeToZNear() {
		Perspective perspective = new( 90, 1 );
		perspective.ZNear = perspective.ZFar + 1;
		Assert.That( perspective.ZNear, Is.EqualTo( Perspective.DEFAULT_NEAR ) );
	}

	[Test]
	public void Perspective_ZFarChangeToZNear_NoChangeToZFar() {
		Perspective perspective = new( 90, 1 );
		perspective.ZFar = perspective.ZNear;
		Assert.That( perspective.ZFar, Is.EqualTo( Perspective.DEFAULT_FAR ) );
	}

	[Test]
	public void Perspective_ZFarChangeToNegative_NoChangeToZFar() {
		Perspective perspective = new( 90, 1 );
		perspective.ZFar = -1;
		Assert.That( perspective.ZFar, Is.EqualTo( Perspective.DEFAULT_FAR ) );
	}

	[Test]
	public void Perspective_Constructor_FOVLessThanZero_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( -1, 1 ) );

	[Test]
	public void Perspective_Constructor_FOVEqual180_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( 180, 1 ) );

	[Test]
	public void Perspective_Constructor_FOVOver180_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( 181, 1 ) );

	[Test]
	public void Perspective_Constructor_AspectRatioLessThanZero_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( 90, -1 ) );

	[Test]
	public void Perspective_Constructor_AspectRatioEqualToZero_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( 90, 0 ) );

	[Test]
	public void Perspective_Constructor_ZNearGreaterThanZFar_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( 90, 1, 1, 0 ) );

	[Test]
	public void Perspective_Constructor_ZFarEqualToZNear_Exception() => Assert.Throws<ArgumentOutOfRangeException>( () => new Perspective( 90, 1, 1, 1 ) );

	[Test]
	public void Perspective_Constructor_AspectRatioGreaterThanZero_NoException() => Assert.DoesNotThrow( () => new Perspective( 90, 1 ) );
}
