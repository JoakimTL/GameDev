namespace Engine.Math.Tests;

[TestFixture]
public sealed class Matrix3x3FactoryTests {
	[Test]
	public void CreateScaling_2d() {
		float x = 3;
		float y = 5;

		Matrix3x3<float> resultComponents = Matrix.Create3x3.Scaling( x, y );
		Matrix3x3<float> resultVector = Matrix.Create3x3.Scaling( new Vector2<float>( x, y ) );

		Matrix3x3<float> expected = new(
			x, 0, 0,
			0, y, 0,
			0, 0, 1
		);

		Assert.That( resultComponents, Is.EqualTo( expected ) );
		Assert.That( resultVector, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateScaling_3d() {
		float x = 3;
		float y = 5;
		float z = 7;

		Matrix3x3<float> resultComponents = Matrix.Create3x3.Scaling( x, y, z );
		Matrix3x3<float> resultVector = Matrix.Create3x3.Scaling( new Vector3<float>( x, y, z ) );

		Matrix3x3<float> expected = new(
			x, 0, 0,
			0, y, 0,
			0, 0, z
		);

		Assert.That( resultComponents, Is.EqualTo( expected ) );
		Assert.That( resultVector, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationX() {
		float angle = 90F / 180 * MathF.PI;

		Matrix3x3<float> result = Matrix.Create3x3.RotationX( angle );

		Matrix3x3<float> expected = new(
			1, 0, 0,
			0, MathF.Cos( angle ), -MathF.Sin( angle ),
			0, MathF.Sin( angle ), MathF.Cos( angle )
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationY() {
		float angle = 90F / 180 * MathF.PI;

		Matrix3x3<float> result = Matrix.Create3x3.RotationY( angle );

		Matrix3x3<float> expected = new(
			MathF.Cos( angle ), 0, MathF.Sin( angle ),
			0, 1, 0,
			-MathF.Sin( angle ), 0, MathF.Cos( angle )
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationZ() {
		float angle = 90F / 180 * MathF.PI;

		Matrix3x3<float> result = Matrix.Create3x3.RotationZ( angle );

		Matrix3x3<float> expected = new(
			MathF.Cos( angle ), -MathF.Sin( angle ), 0,
			MathF.Sin( angle ), MathF.Cos( angle ), 0,
			0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void RotationAroundAxisX() {
		float angle = 90F / 180 * MathF.PI;
		Vector3<float> axis = new( 1, 0, 0 );

		Matrix3x3<float> result = Matrix.Create3x3.RotationAroundAxis( axis, angle );

		Matrix3x3<float> expected = new(
			1, 0, 0,
			0, MathF.Cos( angle ), -MathF.Sin( angle ),
			0, MathF.Sin( angle ), MathF.Cos( angle )
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.00001f ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.00001f ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.00001f ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.00001f ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.00001f ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.00001f ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.00001f ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.00001f ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.00001f ) );
	}

	[Test]
	public void RotationAroundAxisY() {
		float angle = 90F / 180 * MathF.PI;
		Vector3<float> axis = new( 0, 1, 0 );

		Matrix3x3<float> result = Matrix.Create3x3.RotationAroundAxis( axis, angle );

		Matrix3x3<float> expected = new(
			MathF.Cos( angle ), 0, MathF.Sin( angle ),
			0, 1, 0,
			-MathF.Sin( angle ), 0, MathF.Cos( angle )
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.00001f ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.00001f ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.00001f ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.00001f ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.00001f ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.00001f ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.00001f ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.00001f ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.00001f ) );
	}

	[Test]
	public void RotationAroundAxisZ() {
		float angle = 90F / 180 * MathF.PI;
		Vector3<float> axis = new( 0, 0, 1 );

		Matrix3x3<float> result = Matrix.Create3x3.RotationAroundAxis( axis, angle );

		Matrix3x3<float> expected = new(
			MathF.Cos( angle ), -MathF.Sin( angle ), 0,
			MathF.Sin( angle ), MathF.Cos( angle ), 0,
			0, 0, 1
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.00001f ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.00001f ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.00001f ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.00001f ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.00001f ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.00001f ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.00001f ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.00001f ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.00001f ) );
	}

	[Test]
	public void CreateRotationFromRotorX() {
		float angle = 90F / 180 * MathF.PI;
		Rotor3<float> rotor = Rotor3.FromVectors<float>( new( 0, 1, 0 ), new( 0, 0, -1 ) );

		Matrix3x3<float> result = Matrix.Create3x3.RotationFromRotor( rotor );

		Matrix3x3<float> expected = new(
			1, 0, 0,
			0, MathF.Cos( angle ), -MathF.Sin( angle ),
			0, MathF.Sin( angle ), MathF.Cos( angle )
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.00001f ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.00001f ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.00001f ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.00001f ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.00001f ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.00001f ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.00001f ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.00001f ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.00001f ) );
	}

	[Test]
	public void CreateRotationFromRotorY() {
		float angle = 90F / 180 * MathF.PI;
		Rotor3<float> rotor = Rotor3.FromVectors<float>( new( 1, 0, 0 ), new( 0, 0, 1 ) );

		Matrix3x3<float> result = Matrix.Create3x3.RotationFromRotor( rotor );

		Matrix3x3<float> expected = new(
			MathF.Cos( angle ), 0, MathF.Sin( angle ),
			0, 1, 0,
			-MathF.Sin( angle ), 0, MathF.Cos( angle )
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.00001f ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.00001f ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.00001f ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.00001f ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.00001f ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.00001f ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.00001f ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.00001f ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.00001f ) );
	}

	[Test]
	public void CreateRotationFromRotorZ() {
		float angle = 90F / 180 * MathF.PI;
		Rotor3<float> rotor = Rotor3.FromVectors<float>( new( 1, 0, 0 ), new( 0, -1, 0 ) );

		Matrix3x3<float> result = Matrix.Create3x3.RotationFromRotor( rotor );

		Matrix3x3<float> expected = new(
			MathF.Cos( angle ), -MathF.Sin( angle ), 0,
			MathF.Sin( angle ), MathF.Cos( angle ), 0,
			0, 0, 1
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.00001f ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.00001f ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.00001f ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.00001f ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.00001f ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.00001f ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.00001f ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.00001f ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.00001f ) );
	}

	[Test]
	public void Basis() {
		Vector3<double> xBasis = new( 0, 0, 1 );
		Vector3<double> yBasis = new( 1, 0, 0 );
		Vector3<double> zBasis = new( 0, 1, 0 );

		Matrix3x3<double> result = Matrix.Create3x3.Basis( xBasis, yBasis, zBasis );

		Matrix3x3<double> expected = new(
			0, 1, 0,
			0, 0, 1,
			1, 0, 0
		);

		Vector3<double> vectorInBasis = new( 2, 3, 5 );
		Vector3<double> resultVector = result * vectorInBasis;

		Assert.That( result, Is.EqualTo( expected ) );
		Assert.That( resultVector, Is.EqualTo( new Vector3<double>( 3, 5, 2 ) ) );
	}
}

[TestFixture]
public sealed class MatrixTests {
	[Test]
	public void TryFill() {
		/*
			uint sizeStorageBytes = (uint) (resultStorage.Length * sizeof( TData ));
			uint expectedSizeBytes = (uint) sizeof( TMatrix );
			if (expectedSizeBytes + destinationOffsetBytes > sizeStorageBytes)
				return false;
			TMatrix matrix = columnMajor ? m.GetTransposed() : m;
			fixed (TData* dstPtr = resultStorage)
				Unsafe.CopyBlock( (byte*) dstPtr + destinationOffsetBytes, &matrix, expectedSizeBytes );
			return true;
		 */
		//Success
		Matrix3x3<int> m = new(
			1, 2, 3,
			4, 5, 6,
			7, 8, 9
		);

		Span<int> bigEnoughSpan = stackalloc int[ 9 ];

		bool success = m.TryFill( bigEnoughSpan );

		Assert.That( success, Is.True );
		Assert.That( bigEnoughSpan[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( bigEnoughSpan[ 1 ], Is.EqualTo( 2 ) );
		Assert.That( bigEnoughSpan[ 2 ], Is.EqualTo( 3 ) );
		Assert.That( bigEnoughSpan[ 3 ], Is.EqualTo( 4 ) );
		Assert.That( bigEnoughSpan[ 4 ], Is.EqualTo( 5 ) );
		Assert.That( bigEnoughSpan[ 5 ], Is.EqualTo( 6 ) );
		Assert.That( bigEnoughSpan[ 6 ], Is.EqualTo( 7 ) );
		Assert.That( bigEnoughSpan[ 7 ], Is.EqualTo( 8 ) );
		Assert.That( bigEnoughSpan[ 8 ], Is.EqualTo( 9 ) );

		success = m.TryFill( bigEnoughSpan, columnMajor: true );

		Assert.That( success, Is.True );
		Assert.That( bigEnoughSpan[ 0 ], Is.EqualTo( 1 ) );
		Assert.That( bigEnoughSpan[ 1 ], Is.EqualTo( 4 ) );
		Assert.That( bigEnoughSpan[ 2 ], Is.EqualTo( 7 ) );
		Assert.That( bigEnoughSpan[ 3 ], Is.EqualTo( 2 ) );
		Assert.That( bigEnoughSpan[ 4 ], Is.EqualTo( 5 ) );
		Assert.That( bigEnoughSpan[ 5 ], Is.EqualTo( 8 ) );
		Assert.That( bigEnoughSpan[ 6 ], Is.EqualTo( 3 ) );
		Assert.That( bigEnoughSpan[ 7 ], Is.EqualTo( 6 ) );
		Assert.That( bigEnoughSpan[ 8 ], Is.EqualTo( 9 ) );
		//Failure
		Span<int> tooSmallSpan = stackalloc int[ 8 ];

		bool failure = m.TryFill( tooSmallSpan );

		Assert.That( failure, Is.False );
	}
}