using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class VectorTests {
	[Test]
	public void MagnitudeSquared() {
		Vector2<int> vectorA = new( 3, 4 );
		Vector2<double> vectorB = new( 3, 4 );

		Assert.That( vectorA.MagnitudeSquared<Vector2<int>, int>(), Is.EqualTo( 25 ) );
		Assert.That( vectorB.MagnitudeSquared<Vector2<double>, double>(), Is.EqualTo( 25.0 ) );
	}

	[Test]
	public void Magnitude() {
		Vector2<double> vectorB = new( 3, 4 );

		Assert.That( vectorB.Magnitude<Vector2<double>, double>(), Is.EqualTo( 5.0 ) );
	}

	[Test]
	public void Normalize() {
		Vector2<double> vectorB = new( 3, 4 );

		Assert.That( vectorB.Normalize<Vector2<double>, double>(), Is.EqualTo( new Vector2<double>( 0.6, 0.8 ) ) );
	}

	[Test]
	public void TryNormalize() {
		Vector2<double> vectorB = new( 3, 4 );

		bool success = vectorB.TryNormalize( out Vector2<double> result, out double originalMagnitude );

		Assert.That( success, Is.True );
		Assert.That( result, Is.EqualTo( new Vector2<double>( 0.6, 0.8 ) ) );
		Assert.That( originalMagnitude, Is.EqualTo( 5.0 ) );
	}

	[Test]
	public void TryNormalize_Fail() {
		Vector2<double> vectorB = new( 0, 0 );

		bool success = vectorB.TryNormalize( out Vector2<double> result, out double originalMagnitude );

		Assert.That( success, Is.False );
		Assert.That( result, Is.EqualTo( default( Vector2<double> ) ) );
		Assert.That( originalMagnitude, Is.EqualTo( 0 ) );
	}

	[Test]
	public void ReflectMirror() {
		Vector2<double> vectorB = new( 3, 4 );
		Vector2<double> normal = new( 1, 0 );

		Assert.That( vectorB.ReflectMirror<Vector2<double>, double>( normal ), Is.EqualTo( new Vector2<double>( -3, 4 ) ) );
	}

	[Test]
	public void Floor() {
		Vector2<double> vectorB = new( 3.5, 4.5 );

		Assert.That( vectorB.Floor<Vector2<double>, double>(), Is.EqualTo( new Vector2<double>( 3.0, 4.0 ) ) );
	}

	[Test]
	public void Ceiling() {
		Vector2<double> vectorB = new( 3.5, 4.5 );

		Assert.That( vectorB.Ceiling<Vector2<double>, double>(), Is.EqualTo( new Vector2<double>( 4.0, 5.0 ) ) );
	}

	[Test]
	public void Round() {
		Vector2<double> vectorB = new( 3.5, 4.5 );

		Assert.That( vectorB.Round<Vector2<double>, double>( 0, MidpointRounding.ToEven ), Is.EqualTo( new Vector2<double>( 4.0, 4.0 ) ) );
		Assert.That( vectorB.Round<Vector2<double>, double>( 0, MidpointRounding.ToNegativeInfinity ), Is.EqualTo( new Vector2<double>( 3.0, 4.0 ) ) );
		Assert.That( vectorB.Round<Vector2<double>, double>( 0, MidpointRounding.ToPositiveInfinity ), Is.EqualTo( new Vector2<double>( 4.0, 5.0 ) ) );
		Assert.That( vectorB.Round<Vector2<double>, double>( 0, MidpointRounding.AwayFromZero ), Is.EqualTo( new Vector2<double>( 4.0, 5.0 ) ) );
	}

	[Test]
	public void Cross() {
		Vector3<double> vectorA = new( 1, 0, 0 );
		Vector3<double> vectorB = new( 0, 1, 0 );

		Assert.That( vectorA.Cross( vectorB ), Is.EqualTo( new Vector3<double>( 0, 0, 1 ) ) );
	}
}
