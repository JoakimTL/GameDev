namespace Engine.Math.Tests;

[TestFixture]
public sealed class VectorTests {
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

	[Test]
	public void ZeroConditionalTests() {
		Vector3<int> highVector = new( 1, 1, 1 );
		Vector3<int> lowVector = new( -1, -1, -1 );
		Vector3<int> zeroVector = new( 0, 0, 0 );
		Vector3<int> mixedVector = new( 1, -1, 1 );
		Vector3<int> positiveVector = new( 1, 0, 1 );
		Vector3<int> negativeVector = new( -1, -1, 0 );

		Assert.That( highVector.IsHigherThanZero(), Is.True );
		Assert.That( highVector.IsLowerThanZero(), Is.False );
		Assert.That( highVector.IsPositiveOrZero(), Is.True );
		Assert.That( highVector.IsNegativeOrZero(), Is.False );
		Assert.That( lowVector.IsHigherThanZero(), Is.False );
		Assert.That( lowVector.IsLowerThanZero(), Is.True );
		Assert.That( lowVector.IsPositiveOrZero(), Is.False );
		Assert.That( lowVector.IsNegativeOrZero(), Is.True );
		Assert.That( zeroVector.IsHigherThanZero(), Is.False );
		Assert.That( zeroVector.IsLowerThanZero(), Is.False );
		Assert.That( zeroVector.IsPositiveOrZero(), Is.True );
		Assert.That( zeroVector.IsNegativeOrZero(), Is.True );
		Assert.That( mixedVector.IsHigherThanZero(), Is.False );
		Assert.That( mixedVector.IsLowerThanZero(), Is.False );
		Assert.That( mixedVector.IsPositiveOrZero(), Is.False );
		Assert.That( mixedVector.IsNegativeOrZero(), Is.False );
		Assert.That( positiveVector.IsHigherThanZero(), Is.False );
		Assert.That( positiveVector.IsLowerThanZero(), Is.False );
		Assert.That( positiveVector.IsPositiveOrZero(), Is.True );
		Assert.That( positiveVector.IsNegativeOrZero(), Is.False );
		Assert.That( negativeVector.IsHigherThanZero(), Is.False );
		Assert.That( negativeVector.IsLowerThanZero(), Is.False );
		Assert.That( negativeVector.IsPositiveOrZero(), Is.False );
		Assert.That( negativeVector.IsNegativeOrZero(), Is.True );
	}

	[Test]
	public void Vector2Casts() {
		Vector2<float> vectorNormal = new( 10.85f, 1.5f );
		Vector2<float> vectorOverflow = new( float.MaxValue, 1.5f );
		Vector2<int> vectorNormalInt = new( int.MaxValue, 10 );
		Vector2<long> vectorNormalLong = new( long.MaxValue, int.MaxValue );

		Assert.That( vectorNormal.CastChecked<float, int>(), Is.EqualTo( new Vector2<int>( 10, 1 ) ) );
		Assert.That( vectorNormal.CastSaturating<float, int>(), Is.EqualTo( new Vector2<int>( 10, 1 ) ) );
		Assert.That( vectorNormal.CastTruncating<float, int>(), Is.EqualTo( new Vector2<int>( 10, 1 ) ) );

		Assert.Throws<OverflowException>( () => vectorOverflow.CastChecked<float, int>() );
		Assert.That( vectorOverflow.CastSaturating<float, int>(), Is.EqualTo( new Vector2<int>( int.MaxValue, 1 ) ) );
		Assert.That( vectorOverflow.CastTruncating<float, int>(), Is.EqualTo( new Vector2<int>( int.MaxValue, 1 ) ) );

		Assert.That( vectorNormalInt.CastChecked<int, long>(), Is.EqualTo( new Vector2<long>( int.MaxValue, 10 ) ) );
		Assert.That( vectorNormalInt.CastSaturating<int, long>(), Is.EqualTo( new Vector2<long>( int.MaxValue, 10 ) ) );
		Assert.That( vectorNormalInt.CastTruncating<int, long>(), Is.EqualTo( new Vector2<long>( int.MaxValue, 10 ) ) );

		Assert.Throws<OverflowException>( () => vectorNormalLong.CastChecked<long, int>() );
		Assert.That( vectorNormalLong.CastSaturating<long, int>(), Is.EqualTo( new Vector2<int>( int.MaxValue, int.MaxValue ) ) );
		Assert.That( vectorNormalLong.CastTruncating<long, int>(), Is.EqualTo( new Vector2<int>( -1, int.MaxValue ) ) );
	}

	[Test]
	public void Vector3Casts() {
		Vector3<float> vectorNormal = new( 10.85f, 1.5f, -6 );
		Vector3<float> vectorOverflow = new( float.MaxValue, 1.5f, -8 );
		Vector3<int> vectorNormalInt = new( int.MaxValue, 10, -30 );
		Vector3<long> vectorNormalLong = new( long.MaxValue, int.MaxValue, long.MinValue );

		Assert.That( vectorNormal.CastChecked<float, int>(), Is.EqualTo( new Vector3<int>( 10, 1, -6 ) ) );
		Assert.That( vectorNormal.CastSaturating<float, int>(), Is.EqualTo( new Vector3<int>( 10, 1, -6 ) ) );
		Assert.That( vectorNormal.CastTruncating<float, int>(), Is.EqualTo( new Vector3<int>( 10, 1, -6 ) ) );

		Assert.Throws<OverflowException>( () => vectorOverflow.CastChecked<float, int>() );
		Assert.That( vectorOverflow.CastSaturating<float, int>(), Is.EqualTo( new Vector3<int>( int.MaxValue, 1, -8 ) ) );
		Assert.That( vectorOverflow.CastTruncating<float, int>(), Is.EqualTo( new Vector3<int>( int.MaxValue, 1, -8 ) ) );

		Assert.That( vectorNormalInt.CastChecked<int, long>(), Is.EqualTo( new Vector3<long>( int.MaxValue, 10, -30 ) ) );
		Assert.That( vectorNormalInt.CastSaturating<int, long>(), Is.EqualTo( new Vector3<long>( int.MaxValue, 10, -30 ) ) );
		Assert.That( vectorNormalInt.CastTruncating<int, long>(), Is.EqualTo( new Vector3<long>( int.MaxValue, 10, -30 ) ) );

		Assert.Throws<OverflowException>( () => vectorNormalLong.CastChecked<long, int>() );
		Assert.That( vectorNormalLong.CastSaturating<long, int>(), Is.EqualTo( new Vector3<int>( int.MaxValue, int.MaxValue, int.MinValue ) ) );
		Assert.That( vectorNormalLong.CastTruncating<long, int>(), Is.EqualTo( new Vector3<int>( -1, int.MaxValue, 0 ) ) );
	}

	[Test]
	public void Vector4Casts() {
		Vector4<float> vectorNormal = new( 10.85f, 1.5f, -6, 29 );
		Vector4<float> vectorOverflow = new( float.MaxValue, 1.5f, -8, 29 );
		Vector4<int> vectorNormalInt = new( int.MaxValue, 10, -30, 29 );
		Vector4<long> vectorNormalLong = new( long.MaxValue, int.MaxValue, long.MinValue, 29 );

		Assert.That( vectorNormal.CastChecked<float, int>(), Is.EqualTo( new Vector4<int>( 10, 1, -6, 29 ) ) );
		Assert.That( vectorNormal.CastSaturating<float, int>(), Is.EqualTo( new Vector4<int>( 10, 1, -6, 29 ) ) );
		Assert.That( vectorNormal.CastTruncating<float, int>(), Is.EqualTo( new Vector4<int>( 10, 1, -6, 29 ) ) );

		Assert.Throws<OverflowException>( () => vectorOverflow.CastChecked<float, int>() );
		Assert.That( vectorOverflow.CastSaturating<float, int>(), Is.EqualTo( new Vector4<int>( int.MaxValue, 1, -8, 29 ) ) );
		Assert.That( vectorOverflow.CastTruncating<float, int>(), Is.EqualTo( new Vector4<int>( int.MaxValue, 1, -8, 29 ) ) );

		Assert.That( vectorNormalInt.CastChecked<int, long>(), Is.EqualTo( new Vector4<long>( int.MaxValue, 10, -30, 29 ) ) );
		Assert.That( vectorNormalInt.CastSaturating<int, long>(), Is.EqualTo( new Vector4<long>( int.MaxValue, 10, -30, 29 ) ) );
		Assert.That( vectorNormalInt.CastTruncating<int, long>(), Is.EqualTo( new Vector4<long>( int.MaxValue, 10, -30, 29 ) ) );

		Assert.Throws<OverflowException>( () => vectorNormalLong.CastChecked<long, int>() );
		Assert.That( vectorNormalLong.CastSaturating<long, int>(), Is.EqualTo( new Vector4<int>( int.MaxValue, int.MaxValue, int.MinValue, 29 ) ) );
		Assert.That( vectorNormalLong.CastTruncating<long, int>(), Is.EqualTo( new Vector4<int>( -1, int.MaxValue, 0, 29 ) ) );
	}
}
