using System.Numerics;

namespace Engine.Math.NewVectors;

public static partial class Matrix {
	public static class Create3x3 {
		public static Matrix3x3<T> Scaling<T>( T scaleX, T scaleY ) where T : unmanaged, INumber<T>
		=> Scaling( new Vector2<T>( scaleX, scaleY ) );

		public static Matrix3x3<T> Scaling<T>( in Vector2<T> scaling ) where T : unmanaged, INumber<T> {
			T m00 = scaling.X;
			T m01 = T.Zero;
			T m10 = T.Zero;
			T m11 = scaling.Y;

			return new(
				m00, m01,
				m10, m11
			);
		}

		public static Matrix3x3<T> Scaling<T>( T scaleX, T scaleY, T scaleZ ) where T : unmanaged, INumber<T>
			=> Scaling( new Vector3<T>( scaleX, scaleY, scaleZ ) );

		public static Matrix3x3<T> Scaling<T>( in Vector3<T> scaling ) where T : unmanaged, INumber<T> {
			T m00 = scaling.X;
			T m01 = T.Zero;
			T m02 = T.Zero;
			T m10 = T.Zero;
			T m11 = scaling.Y;
			T m12 = T.Zero;
			T m20 = T.Zero;
			T m21 = T.Zero;
			T m22 = scaling.Z;

			return new(
				m00, m01, m02,
				m10, m11, m12,
				m20, m21, m22
			);
		}

		public static Matrix3x3<T> RotationX<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
			T cos = T.Cos( radians );
			T sin = T.Sin( radians );

			T m00 = T.One;
			T m01 = T.Zero;
			T m02 = T.Zero;
			T m10 = T.Zero;
			T m11 = cos;
			T m12 = -sin;
			T m20 = T.Zero;
			T m21 = sin;
			T m22 = cos;

			return new(
				m00, m01, m02,
				m10, m11, m12,
				m20, m21, m22
			);
		}

		public static Matrix3x3<T> RotationY<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
			T cos = T.Cos( radians );
			T sin = T.Sin( radians );

			T m00 = cos;
			T m01 = T.Zero;
			T m02 = sin;
			T m10 = T.Zero;
			T m11 = T.One;
			T m12 = T.Zero;
			T m20 = -sin;
			T m21 = T.Zero;
			T m22 = cos;

			return new(
				m00, m01, m02,
				m10, m11, m12,
				m20, m21, m22
			);
		}

		public static Matrix3x3<T> RotationZ<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
			T cos = T.Cos( radians );
			T sin = T.Sin( radians );

			T m00 = cos;
			T m01 = -sin;
			T m10 = sin;
			T m11 = cos;

			return new(
				m00, m01,
				m10, m11
			);
		}

		public static Matrix3x3<T> RotationAroundAxis<T>( in Vector3<T> axis, T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
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
			T m10 = (xy * oneMinusCos) + zSin;
			T m11 = cos + (yy * oneMinusCos);
			T m12 = (yz * oneMinusCos) - xSin;
			T m20 = (xz * oneMinusCos) - ySin;
			T m21 = (yz * oneMinusCos) + xSin;
			T m22 = cos + (zz * oneMinusCos);

			return new(
				m00, m01, m02,
				m10, m11, m12,
				m20, m21, m22
			);
		}

		public static Matrix3x3<T> RotationFromRotor<T>( in Rotor3<T> rotor ) where T : unmanaged, INumber<T> {
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

		/// <summary>
		/// Basis matrices are used to transform vectors from one basis to another.<br/>
		/// To transform a vector from one basis to another using basis matrices simply multiply the matrix with the vector.<br/>
		/// To transform the transformed vector back to the original basis multiply the inverse of the matrix with the transformed vector.
		/// </summary>
		/// <typeparam name="TScalar"></typeparam>
		/// <param name="xBasis">The basis vector representing the x-axis on the vector.</param>
		/// <param name="yBasis">The basis vector representing the y-axis on the vector.</param>
		/// <param name="zBasis">The basis vector representing the z-axis on the vector.</param>
		/// <returns></returns>
		public static Matrix3x3<TScalar> Basis<TScalar>( in Vector3<TScalar> xBasis, in Vector3<TScalar> yBasis, in Vector3<TScalar> zBasis )
			where TScalar :
				unmanaged, IFloatingPointIeee754<TScalar>
			=> new( xBasis.X, yBasis.X, zBasis.X, xBasis.Y, yBasis.Y, zBasis.Y, xBasis.Z, yBasis.Z, zBasis.Z );
	}
}