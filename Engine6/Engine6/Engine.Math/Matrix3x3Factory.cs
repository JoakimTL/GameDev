using System.Numerics;

namespace Engine.Math;

public static class Matrix3x3Factory {
	public static Matrix3x3<T> CreateRotationFromRotor<T>( in Rotor3<T> rotor ) where T : unmanaged, INumber<T> {
		T two = T.One + T.One;

		T x = rotor.Bivector.YZ;
		T y = rotor.Bivector.ZX;
		T z = rotor.Bivector.XY;
		T w = rotor.Scalar;

		T m00 = T.One - (two * ((y * y) + (z * z)));  //1-2(y^2+z^2)
		T m01 = two * ((x * y) - (w * z));            //2(xy-wz)
		T m02 = two * ((x * z) + (w * y));            //2(xz+wy)
		T m10 = two * ((x * y) + (w * z));            //2(xy+wz)
		T m11 = T.One - (two * ((x * x) + (z * z)));  //1-2(x^2+z^2)
		T m12 = two * ((y * z) - (w * x));            //2(yz-wx)
		T m20 = two * ((x * z) - (w * y));            //2(xz-wy)
		T m21 = two * ((y * z) + (w * x));            //2(yz+wx)
		T m22 = T.One - (two * ((x * x) + (y * y)));  //1-2(x^2+y^2)

		return new(
			m00, m01, m02,
			m10, m11, m12,
			m20, m21, m22
		);
	}
}