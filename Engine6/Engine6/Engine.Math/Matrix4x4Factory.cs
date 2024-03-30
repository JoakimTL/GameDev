using System.Numerics;

namespace Engine.Math;

public static class Matrix4x4Factory {
	public static Matrix4x4<T> CreateTranslation<T>( T translationX, T translationY ) where T : unmanaged, INumber<T>
		=> CreateTranslation( new Vector2<T>( translationX, translationY ) );

	public static Matrix4x4<T> CreateTranslation<T>( in Vector2<T> translation ) where T : unmanaged, INumber<T> {
		T m00 = T.One;
		T m01 = T.Zero;
		T m02 = T.Zero;
		T m03 = translation.X;
		T m10 = T.Zero;
		T m11 = T.One;
		T m12 = T.Zero;
		T m13 = translation.Y;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = T.One;
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateTranslation<T>( T translationX, T translationY, T translationZ ) where T : unmanaged, INumber<T> 
		=> CreateTranslation( new Vector3<T>( translationX, translationY, translationZ ) );

	public static Matrix4x4<T> CreateTranslation<T>( in Vector3<T> translation ) where T : unmanaged, INumber<T> {
		T m00 = T.One;
		T m01 = T.Zero;
		T m02 = T.Zero;
		T m03 = translation.X;
		T m10 = T.Zero;
		T m11 = T.One;
		T m12 = T.Zero;
		T m13 = translation.Y;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = T.One;
		T m23 = translation.Z;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateScaling<T>( T scaleX, T scaleY ) where T : unmanaged, INumber<T>
		=> CreateScaling( new Vector2<T>( scaleX, scaleY ) );

	public static Matrix4x4<T> CreateScaling<T>( in Vector2<T> scaling ) where T : unmanaged, INumber<T> {
		T m00 = scaling.X;
		T m01 = T.Zero;
		T m02 = T.Zero;
		T m03 = T.Zero;
		T m10 = T.Zero;
		T m11 = scaling.Y;
		T m12 = T.Zero;
		T m13 = T.Zero;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = T.One;
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}


	public static Matrix4x4<T> CreateScaling<T>( T scaleX, T scaleY, T scaleZ ) where T : unmanaged, INumber<T>
		=> CreateScaling( new Vector3<T>( scaleX, scaleY, scaleZ ) );

	public static Matrix4x4<T> CreateScaling<T>( in Vector3<T> scaling ) where T : unmanaged, INumber<T> {
		T m00 = scaling.X;
		T m01 = T.Zero;
		T m02 = T.Zero;
		T m03 = T.Zero;
		T m10 = T.Zero;
		T m11 = scaling.Y;
		T m12 = T.Zero;
		T m13 = T.Zero;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = scaling.Z;
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateRotationX<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
		T cos = T.Cos( radians );
		T sin = T.Sin( radians );

		T m00 = T.One;
		T m01 = T.Zero;
		T m02 = T.Zero;
		T m03 = T.Zero;
		T m10 = T.Zero;
		T m11 = cos;
		T m12 = -sin;
		T m13 = T.Zero;
		T m20 = T.Zero;
		T m21 = sin;
		T m22 = cos;
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateRotationY<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
		T cos = T.Cos( radians );
		T sin = T.Sin( radians );

		T m00 = cos;
		T m01 = T.Zero;
		T m02 = sin;
		T m03 = T.Zero;
		T m10 = T.Zero;
		T m11 = T.One;
		T m12 = T.Zero;
		T m13 = T.Zero;
		T m20 = -sin;
		T m21 = T.Zero;
		T m22 = cos;
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateRotationZ<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
		T cos = T.Cos( radians );
		T sin = T.Sin( radians );

		T m00 = cos;
		T m01 = -sin;
		T m02 = T.Zero;
		T m03 = T.Zero;
		T m10 = sin;
		T m11 = cos;
		T m12 = T.Zero;
		T m13 = T.Zero;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = T.One;
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateRotationAroundAxis<T>( in Vector3<T> axis, T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
		T cos = T.Cos( radians );
		T oneMinusCos = T.One - cos;
		T sin = T.Sin( radians );

		T xy = axis.X * axis.Y;
		T xz = axis.X * axis.Z;
		T yz = axis.Y * axis.Z;
		T xx = axis.X * axis.X;
		T yy = axis.Y * axis.Y;
		T zz = axis.Z * axis.Z;

		T xSin = axis.X * sin;
		T ySin = axis.Y * sin;
		T zSin = axis.Z * sin;

		T m00 = cos + (xx * oneMinusCos);
		T m01 = (xy * oneMinusCos) - zSin;
		T m02 = (xz * oneMinusCos) + ySin;
		T m03 = T.Zero;
		T m10 = (xy * oneMinusCos) + zSin;
		T m11 = cos + (yy * oneMinusCos);
		T m12 = (yz * oneMinusCos) - xSin;
		T m13 = T.Zero;
		T m20 = (xz * oneMinusCos) - ySin;
		T m21 = (yz * oneMinusCos) + xSin;
		T m22 = cos + (zz * oneMinusCos);
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateRotationFromRotor<T>( in Rotor3<T> rotor ) where T : unmanaged, INumber<T> {
		T two = T.One + T.One;

		T x = rotor.Bivector.YZ;
		T y = rotor.Bivector.ZX;
		T z = rotor.Bivector.XY;
		T w = rotor.Scalar;

		T m00 = T.One - (two * ((y * y) + (z * z)));  //1-2(y^2+z^2)
		T m01 = two * ((x * y) - (w * z));          //2(xy-wz)
		T m02 = two * ((x * z) + (w * y));          //2(xz+wy)
		T m03 = T.Zero;
		T m10 = two * ((x * y) + (w * z));          //2(xy+wz)
		T m11 = T.One - (two * ((x * x) + (z * z)));  //1-2(x^2+z^2)
		T m12 = two * ((y * z) - (w * x));          //2(yz-wx)
		T m13 = T.Zero;
		T m20 = two * ((x * z) - (w * y));          //2(xz-wy)
		T m21 = two * ((y * z) + (w * x));          //2(yz+wx)
		T m22 = T.One - (two * ((x * x) + (y * y)));  //1-2(x^2+y^2)
		T m23 = T.Zero;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreatePerspectiveFieldOfView<T>( T fovRadians, T aspectRatio, T near, T far, bool leftHanded = false ) where T : unmanaged, IFloatingPointIeee754<T> {
		if (fovRadians <= T.Zero)
			throw new ArgumentException( $"{nameof( fovRadians )} must be greater than zero" );

		if (fovRadians >= T.Pi)
			throw new ArgumentException( $"{nameof( fovRadians )} must be less than pi" );

		if (aspectRatio <= T.Zero)
			throw new ArgumentException( $"{nameof( aspectRatio )} must be greater than zero" );

		T two = T.One + T.One;
		T top = near / T.Tan( fovRadians / two );
		T right = top * aspectRatio;

		return CreatePerspective( -right, top, right, -top, near, far, leftHanded );
	}

	public static Matrix4x4<T> CreatePerspective<T>( T left, T top, T right, T bottom, T near, T far, bool leftHanded = false ) where T : unmanaged, INumber<T> {
		if (right <= left)
			throw new ArgumentException( $"{nameof( right )} must be greater than {nameof( left )}" );

		if (bottom >= top)
			throw new ArgumentException( $"{nameof( top )} must be greater than {nameof( bottom )}" );

		if (near >= far)
			throw new ArgumentException( $"{nameof( near )} must be less than {nameof( far )}" );

		T two = T.One + T.One;
		T horInv = T.One / (right - left);
		T verInv = T.One / (top - bottom);
		T depInv = T.One / (far - near);

		T m00 = two * near * horInv;
		T m01 = T.Zero;
		T m02 = (right + left) * horInv;
		T m03 = T.Zero;
		T m10 = T.Zero;
		T m11 = two * near * verInv;
		T m12 = (top + bottom) * verInv;
		T m13 = T.Zero;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = -(far + near) * depInv;
		T m23 = (leftHanded ? two : -two) * far * near * depInv;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = leftHanded ? T.One : -T.One;
		T m33 = T.Zero;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}

	public static Matrix4x4<T> CreateOrthographic<T>( T left, T top, T right, T bottom, T near, T far, bool leftHanded = false ) where T : unmanaged, INumber<T> {
		if (right <= left)
			throw new ArgumentException( $"{nameof( right )} must be greater than {nameof( left )}" );

		if (bottom >= top)
			throw new ArgumentException( $"{nameof( top )} must be greater than {nameof( bottom )}" );

		if (near >= far)
			throw new ArgumentException( $"{nameof( near )} must be less than {nameof( far )}" );

		T two = T.One + T.One;
		T horInv = T.One / (right - left);
		T verInv = T.One / (top - bottom);
		T depInv = T.One / (far - near);

		T m00 = two * horInv;
		T m01 = T.Zero;
		T m02 = T.Zero;
		T m03 = -(right + left) * horInv;
		T m10 = T.Zero;
		T m11 = two * verInv;
		T m12 = T.Zero;
		T m13 = -(top + bottom) * verInv;
		T m20 = T.Zero;
		T m21 = T.Zero;
		T m22 = (leftHanded ? -two : two) * depInv;
		T m23 = -(far + near) * depInv;
		T m30 = T.Zero;
		T m31 = T.Zero;
		T m32 = T.Zero;
		T m33 = T.One;

		return new(
			m00, m01, m02, m03,
			m10, m11, m12, m13,
			m20, m21, m22, m23,
			m30, m31, m32, m33
		);
	}
}