using System.Numerics;

namespace Engine;

public static partial class Matrix {
	public static class Create2x2 {
		public static Matrix2x2<T> Scaling<T>( T scaleX, T scaleY ) where T : unmanaged, INumber<T>
			=> Scaling( new Vector2<T>( scaleX, scaleY ) );

		public static Matrix2x2<T> Scaling<T>( in Vector2<T> scaling ) where T : unmanaged, INumber<T> {
			T m00 = scaling.X;
			T m01 = T.Zero;
			T m10 = T.Zero;
			T m11 = scaling.Y;

			return new(
				m00, m01,
				m10, m11
			);
		}

		public static Matrix2x2<T> Rotation<T>( T radians ) where T : unmanaged, IFloatingPointIeee754<T> {
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

		public static Matrix2x2<T> RotationFromRotor<T>( in Rotor2<T> rotor ) where T : unmanaged, INumber<T> {
			T two = T.One + T.One;

			T z = rotor.Bivector.XY;
			T w = rotor.Scalar;

			T m00 = T.One - (two * z * z);  //1-2(y^2+z^2)
			T m01 = -two * w * z;           //2(xy-wz)
			T m10 = two * w * z;            //2(xy+wz)
			T m11 = T.One - (two * z * z);  //1-2(x^2+z^2)

			return new(
				m00, m01,
				m10, m11
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
		/// <returns></returns>
		public static Matrix2x2<TScalar> Basis<TScalar>( in Vector2<TScalar> xBasis, in Vector2<TScalar> yBasis )
			where TScalar :
				unmanaged, INumber<TScalar>
			=> new( xBasis.X, yBasis.X, xBasis.Y, yBasis.Y );

		/// <summary>
		/// Basis matrices are used to transform vectors from one basis to another.<br/>
		/// To transform a vector from one basis to another using basis matrices simply multiply the matrix with the vector.<br/>
		/// To transform the transformed vector back to the original basis multiply the inverse of the matrix with the transformed vector.
		/// </summary>
		/// <typeparam name="TScalar"></typeparam>
		/// <param name="xBasis">The basis vector representing the x-axis on the vector.</param>
		/// <param name="yBasis">The basis vector representing the y-axis on the vector.</param>
		/// <returns></returns>
		public static Matrix2x2<TScalar> TransposedBasis<TScalar>( in Vector2<TScalar> xBasis, in Vector2<TScalar> yBasis )
			where TScalar :
				unmanaged, INumber<TScalar>
			=> new( xBasis.X, xBasis.Y, yBasis.X, yBasis.Y );
	}
}
