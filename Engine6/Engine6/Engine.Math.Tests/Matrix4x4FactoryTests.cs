namespace Engine.Math.Tests;

public sealed class Matrix4x4FactoryTests {

	[Test]
	public void CreatePerspectiveFieldOfView() {
		float fov = 90F / 180 * MathF.PI;
		float aspect = 1;
		float near = 0.1F;
		float far = 100F;

		Matrix4x4<float> result = Matrix4x4Factory.CreatePerspectiveFieldOfView( fov, aspect, near, far );

		float top = near / MathF.Tan( fov / 2 );
		float bottom = -top;
		float right = top * aspect;
		float left = -right;

		Matrix4x4<float> expected = new(
			2f * near / (right - left), 0, (right + left) / (right - left), 0,
			0, 2 * near / (top - bottom), (top + bottom) / (top - bottom), 0,
			0, 0, -(far + near) / (far - near), -2 * far * near / (far - near),
			0, 0, -1, 0
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreatePerspective() {
		float left = -1;
		float top = 1;
		float right = 1;
		float bottom = -1;
		float near = 0.1F;
		float far = 100F;

		Matrix4x4<float> result = Matrix4x4Factory.CreatePerspective( left, top, right, bottom, near, far );

		Matrix4x4<float> expected = new(
			2f * near / (right - left), 0, (right + left) / (right - left), 0,
			0, 2 * near / (top - bottom), (top + bottom) / (top - bottom), 0,
			0, 0, -(far + near) / (far - near), -2 * far * near / (far - near),
			0, 0, -1, 0
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateOrthographic() {
		float left = -1;
		float top = 1;
		float right = 1;
		float bottom = -1;
		float near = 0.1F;
		float far = 100F;

		Matrix4x4<float> result = Matrix4x4Factory.CreateOrthographic( left, top, right, bottom, near, far );

		Matrix4x4<float> expected = new(
			2f / (right - left), 0, 0, -(right + left) / (right - left),
			0, 2f / (top - bottom), 0, -(top + bottom) / (top - bottom),
			0, 0, 2f / (far - near), -(far + near) / (far - near),
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateScaling_2d_Scalars() {
		float x = 3;
		float y = 5;

		Matrix4x4<float> result = Matrix4x4Factory.CreateScaling( x, y );

		Matrix4x4<float> expected = new(
			x, 0, 0, 0,
			0, y, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateScaling_2d_Vector() {
		float x = 3;
		float y = 5;

		Matrix4x4<float> result = Matrix4x4Factory.CreateScaling( new Vector2<float>( x, y ) );

		Matrix4x4<float> expected = new(
			x, 0, 0, 0,
			0, y, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateScaling_3d_Scalars() {
		float x = 3;
		float y = 5;
		float z = 7;

		Matrix4x4<float> result = Matrix4x4Factory.CreateScaling( x, y, z );

		Matrix4x4<float> expected = new(
			x, 0, 0, 0,
			0, y, 0, 0,
			0, 0, z, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateScaling_3d_Vector() {
		float x = 3;
		float y = 5;
		float z = 7;

		Matrix4x4<float> result = Matrix4x4Factory.CreateScaling( new Vector3<float>( x, y, z ) );

		Matrix4x4<float> expected = new(
			x, 0, 0, 0,
			0, y, 0, 0,
			0, 0, z, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationX() {
		float angle = 90F / 180 * MathF.PI;

		Matrix4x4<float> result = Matrix4x4Factory.CreateRotationX( angle );

		Matrix4x4<float> expected = new(
			1, 0, 0, 0,
			0, MathF.Cos( angle ), -MathF.Sin( angle ), 0,
			0, MathF.Sin( angle ), MathF.Cos( angle ), 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationY() {
		float angle = 90F / 180 * MathF.PI;

		Matrix4x4<float> result = Matrix4x4Factory.CreateRotationY( angle );

		Matrix4x4<float> expected = new(
			MathF.Cos( angle ), 0, MathF.Sin( angle ), 0,
			0, 1, 0, 0,
			-MathF.Sin( angle ), 0, MathF.Cos( angle ), 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationZ() {
		float angle = 90F / 180 * MathF.PI;

		Matrix4x4<float> result = Matrix4x4Factory.CreateRotationZ( angle );

		Matrix4x4<float> expected = new(
			MathF.Cos( angle ), -MathF.Sin( angle ), 0, 0,
			MathF.Sin( angle ), MathF.Cos( angle ), 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateTranslation_2d_Scalars() {
		float x = 3;
		float y = 5;

		Matrix4x4<float> result = Matrix4x4Factory.CreateTranslation( x, y );

		Matrix4x4<float> expected = new(
			1, 0, 0, x,
			0, 1, 0, y,
			0, 0, 1, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateTranslation_2d_Vector() {
		float x = 3;
		float y = 5;

		Matrix4x4<float> result = Matrix4x4Factory.CreateTranslation( new Vector2<float>( x, y ) );

		Matrix4x4<float> expected = new(
			1, 0, 0, x,
			0, 1, 0, y,
			0, 0, 1, 0,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateTranslation_3d_Scalars() {
		float x = 3;
		float y = 5;
		float z = 7;

		Matrix4x4<float> result = Matrix4x4Factory.CreateTranslation( x, y, z );

		Matrix4x4<float> expected = new(
			1, 0, 0, x,
			0, 1, 0, y,
			0, 0, 1, z,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateTranslation_3d_Vector() {
		float x = 3;
		float y = 5;
		float z = 7;

		Matrix4x4<float> result = Matrix4x4Factory.CreateTranslation( new Vector3<float>( x, y, z ) );

		Matrix4x4<float> expected = new(
			1, 0, 0, x,
			0, 1, 0, y,
			0, 0, 1, z,
			0, 0, 0, 1
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateRotationFromRotor_X() {
		float angle = 90F / 180 * MathF.PI;
		Rotor3<float> rotor = Rotor3Factory.FromVectors<float>( new( 0, 1, 0 ), new( 0, 0, -1 ) );

		Matrix4x4<float> result = Matrix4x4Factory.CreateRotationFromRotor( rotor );

		Matrix4x4<float> expected = new(
			1, 0, 0, 0,
			0, MathF.Cos( angle ), -MathF.Sin( angle ), 0,
			0, MathF.Sin( angle ), MathF.Cos( angle ), 0,
			0, 0, 0, 1
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.0001 ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.0001 ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.0001 ) );
		Assert.That( result.M03, Is.EqualTo( expected.M03 ).Within( 0.0001 ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.0001 ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.0001 ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.0001 ) );
		Assert.That( result.M13, Is.EqualTo( expected.M13 ).Within( 0.0001 ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.0001 ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.0001 ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.0001 ) );
		Assert.That( result.M23, Is.EqualTo( expected.M23 ).Within( 0.0001 ) );
		Assert.That( result.M30, Is.EqualTo( expected.M30 ).Within( 0.0001 ) );
		Assert.That( result.M31, Is.EqualTo( expected.M31 ).Within( 0.0001 ) );
		Assert.That( result.M32, Is.EqualTo( expected.M32 ).Within( 0.0001 ) );
		Assert.That( result.M33, Is.EqualTo( expected.M33 ).Within( 0.0001 ) );
	}

	[Test]
	public void CreateRotationFromRotor_Y() {
		float angle = 90F / 180 * MathF.PI;
		Rotor3<float> rotor = Rotor3Factory.FromVectors<float>( new( 1, 0, 0 ), new( 0, 0, 1 ) );

		Matrix4x4<float> result = Matrix4x4Factory.CreateRotationFromRotor( rotor );

		Matrix4x4<float> expected = new(
			MathF.Cos( angle ), 0, MathF.Sin( angle ), 0,
			0, 1, 0, 0,
			-MathF.Sin( angle ), 0, MathF.Cos( angle ), 0,
			0, 0, 0, 1
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.0001 ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.0001 ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.0001 ) );
		Assert.That( result.M03, Is.EqualTo( expected.M03 ).Within( 0.0001 ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.0001 ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.0001 ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.0001 ) );
		Assert.That( result.M13, Is.EqualTo( expected.M13 ).Within( 0.0001 ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.0001 ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.0001 ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.0001 ) );
		Assert.That( result.M23, Is.EqualTo( expected.M23 ).Within( 0.0001 ) );
		Assert.That( result.M30, Is.EqualTo( expected.M30 ).Within( 0.0001 ) );
		Assert.That( result.M31, Is.EqualTo( expected.M31 ).Within( 0.0001 ) );
		Assert.That( result.M32, Is.EqualTo( expected.M32 ).Within( 0.0001 ) );
		Assert.That( result.M33, Is.EqualTo( expected.M33 ).Within( 0.0001 ) );
	}

	[Test]
	public void CreateRotationFromRotor_Z() {
		float angle = 90F / 180 * MathF.PI;
		Rotor3<float> rotor = Rotor3Factory.FromVectors<float>( new( 1, 0, 0 ), new( 0, -1, 0 ) );

		Matrix4x4<float> result = Matrix4x4Factory.CreateRotationFromRotor( rotor );

		Matrix4x4<float> expected = new(
			MathF.Cos( angle ), -MathF.Sin( angle ), 0, 0,
			MathF.Sin( angle ), MathF.Cos( angle ), 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);

		Assert.That( result.M00, Is.EqualTo( expected.M00 ).Within( 0.0001 ) );
		Assert.That( result.M01, Is.EqualTo( expected.M01 ).Within( 0.0001 ) );
		Assert.That( result.M02, Is.EqualTo( expected.M02 ).Within( 0.0001 ) );
		Assert.That( result.M03, Is.EqualTo( expected.M03 ).Within( 0.0001 ) );
		Assert.That( result.M10, Is.EqualTo( expected.M10 ).Within( 0.0001 ) );
		Assert.That( result.M11, Is.EqualTo( expected.M11 ).Within( 0.0001 ) );
		Assert.That( result.M12, Is.EqualTo( expected.M12 ).Within( 0.0001 ) );
		Assert.That( result.M13, Is.EqualTo( expected.M13 ).Within( 0.0001 ) );
		Assert.That( result.M20, Is.EqualTo( expected.M20 ).Within( 0.0001 ) );
		Assert.That( result.M21, Is.EqualTo( expected.M21 ).Within( 0.0001 ) );
		Assert.That( result.M22, Is.EqualTo( expected.M22 ).Within( 0.0001 ) );
		Assert.That( result.M23, Is.EqualTo( expected.M23 ).Within( 0.0001 ) );
		Assert.That( result.M30, Is.EqualTo( expected.M30 ).Within( 0.0001 ) );
		Assert.That( result.M31, Is.EqualTo( expected.M31 ).Within( 0.0001 ) );
		Assert.That( result.M32, Is.EqualTo( expected.M32 ).Within( 0.0001 ) );
		Assert.That( result.M33, Is.EqualTo( expected.M33 ).Within( 0.0001 ) );
	}

}
