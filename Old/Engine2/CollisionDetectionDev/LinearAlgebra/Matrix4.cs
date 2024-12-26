namespace Engine.LMath {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Matrix4 : System.IEquatable<Matrix4> {

		public float
			M00, M01, M02, M03,
			M10, M11, M12, M13,
			M20, M21, M22, M23,
			M30, M31, M32, M33;

		#region Accessors

		public Vector4 Row0 {
			get => new Vector4( M00, M01, M02, M03 );
			set {
				M00 = value.X;
				M01 = value.Y;
				M02 = value.Z;
				M03 = value.W;
			}
		}
		public Vector4 Row1 {
			get => new Vector4( M10, M11, M12, M13 );
			set {
				M10 = value.X;
				M11 = value.Y;
				M12 = value.Z;
				M13 = value.W;
			}
		}
		public Vector4 Row2 {
			get => new Vector4( M20, M21, M22, M23 );
			set {
				M20 = value.X;
				M21 = value.Y;
				M22 = value.Z;
				M23 = value.W;
			}
		}
		public Vector4 Row3 {
			get => new Vector4( M30, M31, M32, M33 );
			set {
				M30 = value.X;
				M31 = value.Y;
				M32 = value.Z;
				M33 = value.W;
			}
		}

		public Vector4 Col0 {
			get => new Vector4( M00, M10, M20, M30 );
			set {
				M00 = value.X;
				M10 = value.Y;
				M20 = value.Z;
				M30 = value.W;
			}
		}
		public Vector4 Col1 {
			get => new Vector4( M01, M11, M21, M31 );
			set {
				M01 = value.X;
				M11 = value.Y;
				M21 = value.Z;
				M31 = value.W;
			}
		}
		public Vector4 Col2 {
			get => new Vector4( M02, M12, M22, M32 );
			set {
				M02 = value.X;
				M12 = value.Y;
				M22 = value.Z;
				M32 = value.W;
			}
		}
		public Vector4 Col3 {
			get => new Vector4( M03, M13, M23, M33 );
			set {
				M03 = value.X;
				M13 = value.Y;
				M23 = value.Z;
				M33 = value.W;
			}
		}

		#endregion

		public Matrix4(
			float M00, float M01, float M02, float M03,
			float M10, float M11, float M12, float M13,
			float M20, float M21, float M22, float M23,
			float M30, float M31, float M32, float M33
		) {
			this.M00 = M00;
			this.M01 = M01;
			this.M02 = M02;
			this.M03 = M03;
			this.M10 = M10;
			this.M11 = M11;
			this.M12 = M12;
			this.M13 = M13;
			this.M20 = M20;
			this.M21 = M21;
			this.M22 = M22;
			this.M23 = M23;
			this.M30 = M30;
			this.M31 = M31;
			this.M32 = M32;
			this.M33 = M33;
		}

		public static readonly Matrix4 Identity = new Matrix4( 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 );
		public static readonly Matrix4 Zero = new Matrix4( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );

		#region Multiply

		public static void Multiply( ref Matrix4 l, ref Matrix4 r, out Matrix4 result ) {
			result = new Matrix4 {
				M00 = ( ( ( l.M00 * r.M00 ) + ( l.M01 * r.M10 ) ) + ( l.M02 * r.M20 ) ) + ( l.M03 * r.M30 ),
				M01 = ( ( ( l.M00 * r.M01 ) + ( l.M01 * r.M11 ) ) + ( l.M02 * r.M21 ) ) + ( l.M03 * r.M31 ),
				M02 = ( ( ( l.M00 * r.M02 ) + ( l.M01 * r.M12 ) ) + ( l.M02 * r.M22 ) ) + ( l.M03 * r.M32 ),
				M03 = ( ( ( l.M00 * r.M03 ) + ( l.M01 * r.M13 ) ) + ( l.M02 * r.M23 ) ) + ( l.M03 * r.M33 ),
				M10 = ( ( ( l.M10 * r.M00 ) + ( l.M11 * r.M10 ) ) + ( l.M12 * r.M20 ) ) + ( l.M13 * r.M30 ),
				M11 = ( ( ( l.M10 * r.M01 ) + ( l.M11 * r.M11 ) ) + ( l.M12 * r.M21 ) ) + ( l.M13 * r.M31 ),
				M12 = ( ( ( l.M10 * r.M02 ) + ( l.M11 * r.M12 ) ) + ( l.M12 * r.M22 ) ) + ( l.M13 * r.M32 ),
				M13 = ( ( ( l.M10 * r.M03 ) + ( l.M11 * r.M13 ) ) + ( l.M12 * r.M23 ) ) + ( l.M13 * r.M33 ),
				M20 = ( ( ( l.M20 * r.M00 ) + ( l.M21 * r.M10 ) ) + ( l.M22 * r.M20 ) ) + ( l.M23 * r.M30 ),
				M21 = ( ( ( l.M20 * r.M01 ) + ( l.M21 * r.M11 ) ) + ( l.M22 * r.M21 ) ) + ( l.M23 * r.M31 ),
				M22 = ( ( ( l.M20 * r.M02 ) + ( l.M21 * r.M12 ) ) + ( l.M22 * r.M22 ) ) + ( l.M23 * r.M32 ),
				M23 = ( ( ( l.M20 * r.M03 ) + ( l.M21 * r.M13 ) ) + ( l.M22 * r.M23 ) ) + ( l.M23 * r.M33 ),
				M30 = ( ( ( l.M30 * r.M00 ) + ( l.M31 * r.M10 ) ) + ( l.M32 * r.M20 ) ) + ( l.M33 * r.M30 ),
				M31 = ( ( ( l.M30 * r.M01 ) + ( l.M31 * r.M11 ) ) + ( l.M32 * r.M21 ) ) + ( l.M33 * r.M31 ),
				M32 = ( ( ( l.M30 * r.M02 ) + ( l.M31 * r.M12 ) ) + ( l.M32 * r.M22 ) ) + ( l.M33 * r.M32 ),
				M33 = ( ( ( l.M30 * r.M03 ) + ( l.M31 * r.M13 ) ) + ( l.M32 * r.M23 ) ) + ( l.M33 * r.M33 )
			};
		}

		public static Matrix4 Multiply( Matrix4 left, Matrix4 right ) {
			Matrix4 result;
			Multiply( ref left, ref right, out result );
			return result;
		}

		public static Matrix4 operator *( Matrix4 left, Matrix4 right ) {
			return Multiply( left, right );
		}

		public static Vector4 Multiply( Vector4 left, Matrix4 right ) {
			return new Vector4(
				left.X * right.M00 + left.Y * right.M10 + left.Z * right.M20 + left.W * right.M30,
				left.X * right.M01 + left.Y * right.M11 + left.Z * right.M21 + left.W * right.M31,
				left.X * right.M02 + left.Y * right.M12 + left.Z * right.M22 + left.W * right.M32,
				left.X * right.M03 + left.Y * right.M13 + left.Z * right.M23 + left.W * right.M33
			);
		}

		public static Vector4 operator *( Vector4 left, Matrix4 right ) {
			return Multiply( left, right );
		}

		public static Vector4 Multiply( Matrix4 left, Vector4 right ) {
			return new Vector4(
				right.X * left.M00 + right.X * left.M10 + right.X * left.M20 + right.X * left.M30,
				right.Y * left.M01 + right.Y * left.M11 + right.Y * left.M21 + right.Y * left.M31,
				right.Z * left.M02 + right.Z * left.M12 + right.Z * left.M22 + right.Z * left.M32,
				right.W * left.M03 + right.W * left.M13 + right.W * left.M23 + right.W * left.M33
			);
		}

		public static Vector4 operator *( Matrix4 left, Vector4 right ) {
			return Multiply( left, right );
		}

		#endregion

		#region Invert

		public static void Invert( ref Matrix4 mat, out Matrix4 result ) {
			float s0 = mat.M00 * mat.M11 - mat.M10 * mat.M01;
			float s1 = mat.M00 * mat.M12 - mat.M10 * mat.M02;
			float s2 = mat.M00 * mat.M13 - mat.M10 * mat.M03;
			float s3 = mat.M01 * mat.M12 - mat.M11 * mat.M02;
			float s4 = mat.M01 * mat.M13 - mat.M11 * mat.M03;
			float s5 = mat.M02 * mat.M13 - mat.M12 * mat.M03;

			float c5 = mat.M22 * mat.M33 - mat.M32 * mat.M23;
			float c4 = mat.M21 * mat.M33 - mat.M31 * mat.M23;
			float c3 = mat.M21 * mat.M32 - mat.M31 * mat.M22;
			float c2 = mat.M20 * mat.M33 - mat.M30 * mat.M23;
			float c1 = mat.M20 * mat.M32 - mat.M30 * mat.M22;
			float c0 = mat.M20 * mat.M31 - mat.M30 * mat.M21;

			float inverseDeterminant = 1.0f / ( s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0 );

			float m00 = mat.M00;
			float m01 = mat.M01;
			float m02 = mat.M02;
			float m03 = mat.M03;
			float m10 = mat.M10;
			float m11 = mat.M11;
			float m12 = mat.M12;
			float m20 = mat.M20;
			float m21 = mat.M21;
			float m22 = mat.M22;
			float m30 = mat.M30;
			float m31 = mat.M31;

			result = new Matrix4 {
				M00 = ( mat.M11 * c5 - mat.M12 * c4 + mat.M13 * c3 ) * inverseDeterminant,
				M01 = ( -mat.M01 * c5 + mat.M02 * c4 - mat.M03 * c3 ) * inverseDeterminant,
				M02 = ( mat.M31 * s5 - mat.M32 * s4 + mat.M33 * s3 ) * inverseDeterminant,
				M03 = ( -mat.M21 * s5 + mat.M22 * s4 - mat.M23 * s3 ) * inverseDeterminant,
				M10 = ( -mat.M10 * c5 + mat.M12 * c2 - mat.M13 * c1 ) * inverseDeterminant,
				M11 = ( m00 * c5 - m02 * c2 + m03 * c1 ) * inverseDeterminant,
				M12 = ( -mat.M30 * s5 + mat.M32 * s2 - mat.M33 * s1 ) * inverseDeterminant,
				M13 = ( mat.M20 * s5 - mat.M22 * s2 + mat.M23 * s1 ) * inverseDeterminant,
				M20 = ( m10 * c4 - m11 * c2 + mat.M13 * c0 ) * inverseDeterminant,
				M21 = ( -m00 * c4 + m01 * c2 - m03 * c0 ) * inverseDeterminant,
				M22 = ( mat.M30 * s4 - mat.M31 * s2 + mat.M33 * s0 ) * inverseDeterminant,
				M23 = ( -m20 * s4 + m21 * s2 - mat.M23 * s0 ) * inverseDeterminant,
				M30 = ( -m10 * c3 + m11 * c1 - m12 * c0 ) * inverseDeterminant,
				M31 = ( m00 * c3 - m01 * c1 + m02 * c0 ) * inverseDeterminant,
				M32 = ( -m30 * s3 + m31 * s1 - mat.M32 * s0 ) * inverseDeterminant,
				M33 = ( m20 * s3 - m21 * s1 + m22 * s0 ) * inverseDeterminant
			};
		}

		public static Matrix4 Invert( ref Matrix4 mat ) {
			Invert( ref mat, out Matrix4 r );
			return r;
		}

		public static Matrix4 Invert( Matrix4 mat ) {
			Invert( ref mat, out Matrix4 r );
			return r;
		}

		#endregion

		#region Transpose

		public Matrix4 Transposed() {
			return Transpose( this );
		}


		public static Matrix4 Transpose( Matrix4 mat ) {
			return Matrix4Factory.CreateFromColumns( mat.Row0, mat.Row1, mat.Row2, mat.Row3 );
		}

		public static void Transpose( ref Matrix4 mat, out Matrix4 result ) {
			result = new Matrix4() {
				Row0 = mat.Col0,
				Row1 = mat.Col1,
				Row2 = mat.Col2,
				Row3 = mat.Col3
			};
		}

		#endregion

		#region Overides and Equals

		public override string ToString() {
			return $"{Row0}\n{Row1}\n{Row2}\n{Row3}";
		}

		public override int GetHashCode() {
			return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
		}

		public override bool Equals( object obj ) {
			if( !( obj is Matrix4 ) ) {
				return false;
			}

			return this.Equals( (Matrix4) obj );
		}

		public bool Equals( Matrix4 other ) {
			return
				M00 == other.M00 &&
				M01 == other.M01 &&
				M02 == other.M02 &&
				M03 == other.M03 &&
				M10 == other.M10 &&
				M11 == other.M11 &&
				M12 == other.M12 &&
				M13 == other.M13 &&
				M20 == other.M20 &&
				M21 == other.M21 &&
				M22 == other.M22 &&
				M23 == other.M23 &&
				M30 == other.M30 &&
				M31 == other.M31 &&
				M32 == other.M32 &&
				M33 == other.M33;
		}

		#endregion

		#region Operators and Implicit

		public static bool operator ==( Matrix4 left, Matrix4 right ) {
			return left.Equals( right );
		}

		public static bool operator !=( Matrix4 left, Matrix4 right ) {
			return !left.Equals( right );
		}


		#region Implicit
		public static implicit operator Matrix4( float[] a ) {
			if( a.Length != 16 )
				throw new System.ArgumentException( "Length of array is not compatible with Matrix4x4" );
			return new Matrix4(
				a[ 0 ], a[ 1 ], a[ 2 ], a[ 3 ],
				a[ 4 ], a[ 5 ], a[ 6 ], a[ 7 ],
				a[ 8 ], a[ 9 ], a[ 10 ], a[ 11 ],
				a[ 12 ], a[ 13 ], a[ 14 ], a[ 15 ] );
		}

		public static implicit operator float[] ( Matrix4 a ) {
			return new float[] {
				a.M00, a.M01, a.M02, a.M03,
				a.M10, a.M11, a.M12, a.M13,
				a.M20, a.M21, a.M22, a.M23,
				a.M30, a.M31, a.M32, a.M33
			};
		}
		#endregion

		#endregion

	}
}
