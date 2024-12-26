using System.Runtime.InteropServices;

namespace Engine.LinearAlgebra {
	[System.Serializable]
	[StructLayout( LayoutKind.Explicit )]
	public struct Matrix3 : System.IEquatable<Matrix3> {

		[FieldOffset( 0 )]
		public float M00;
		[FieldOffset( 4 )]
		public float M01;
		[FieldOffset( 8 )]
		public float M02;
		[FieldOffset( 12 )]
		public float M10;
		[FieldOffset( 16 )]
		public float M11;
		[FieldOffset( 20 )]
		public float M12;
		[FieldOffset( 24 )]
		public float M20;
		[FieldOffset( 28 )]
		public float M21;
		[FieldOffset( 32 )]
		public float M22;

		#region Accessors
		public Vector3 Row0 {
			get => new Vector3( M00, M01, M02 );
			set {
				M00 = value.X;
				M01 = value.Y;
				M02 = value.Z;
			}
		}
		public Vector3 Row1 {
			get => new Vector3( M10, M11, M12 );
			set {
				M10 = value.X;
				M11 = value.Y;
				M12 = value.Z;
			}
		}
		public Vector3 Row2 {
			get => new Vector3( M20, M21, M22 );
			set {
				M20 = value.X;
				M21 = value.Y;
				M22 = value.Z;
			}
		}

		public Vector3 Col0 {
			get => new Vector3( M00, M10, M20 );
			set {
				M00 = value.X;
				M10 = value.Y;
				M20 = value.Z;
			}
		}
		public Vector3 Col1 {
			get => new Vector3( M01, M11, M21 );
			set {
				M01 = value.X;
				M11 = value.Y;
				M21 = value.Z;
			}
		}
		public Vector3 Col2 {
			get => new Vector3( M02, M12, M22 );
			set {
				M02 = value.X;
				M12 = value.Y;
				M22 = value.Z;
			}
		}
		#endregion

		public Matrix3(
			float M00, float M01, float M02,
			float M10, float M11, float M12,
			float M20, float M21, float M22
		) {
			this.M00 = M00;
			this.M01 = M01;
			this.M02 = M02;
			this.M10 = M10;
			this.M11 = M11;
			this.M12 = M12;
			this.M20 = M20;
			this.M21 = M21;
			this.M22 = M22;
		}

		public static readonly Matrix3 Identity = new Matrix3( 1, 0, 0, 0, 1, 0, 0, 0, 1 );
		public static readonly Matrix3 Zero = new Matrix3( 0, 0, 0, 0, 0, 0, 0, 0, 0 );

		#region Determinant
		public float Determinant {
			get {
				// /---------\
				// | a  b  c |
				// | d  e  f |
				// | g  h  i |
				// \---------/
				// a( ei − fh ) − b( di − fg ) + c( dh − eg )
				// BECOMES
				// /-------------------\
				// | M00    M01    M02 |
				// |                   |
				// | M10    M11    M12 |
				// |                   |
				// | M20    M21    M22 |
				// \-------------------/
				// M00 * (M11 * M22 - M12 * M21) - M01 * (M01 * M22 - M20 * M12) + M02 * (M10 * M21 - M20 * M11)
				return M00 * ( M11 * M22 - M12 * M21 ) - M01 * ( M01 * M22 - M20 * M12 ) + M02 * ( M10 * M21 - M20 * M11 );
			}
		}
		#endregion

		#region Multiply

		public static void Multiply( ref Matrix3 l, ref Matrix3 r, out Matrix3 result ) {
			result = new Matrix3 {
				M00 = ( ( ( l.M00 * r.M00 ) + ( l.M01 * r.M10 ) ) + ( l.M02 * r.M20 ) ),
				M01 = ( ( ( l.M00 * r.M01 ) + ( l.M01 * r.M11 ) ) + ( l.M02 * r.M21 ) ),
				M02 = ( ( ( l.M00 * r.M02 ) + ( l.M01 * r.M12 ) ) + ( l.M02 * r.M22 ) ),
				M10 = ( ( ( l.M10 * r.M00 ) + ( l.M11 * r.M10 ) ) + ( l.M12 * r.M20 ) ),
				M11 = ( ( ( l.M10 * r.M01 ) + ( l.M11 * r.M11 ) ) + ( l.M12 * r.M21 ) ),
				M12 = ( ( ( l.M10 * r.M02 ) + ( l.M11 * r.M12 ) ) + ( l.M12 * r.M22 ) ),
				M20 = ( ( ( l.M20 * r.M00 ) + ( l.M21 * r.M10 ) ) + ( l.M22 * r.M20 ) ),
				M21 = ( ( ( l.M20 * r.M01 ) + ( l.M21 * r.M11 ) ) + ( l.M22 * r.M21 ) ),
				M22 = ( ( ( l.M20 * r.M02 ) + ( l.M21 * r.M12 ) ) + ( l.M22 * r.M22 ) )
			};
		}

		public static Matrix3 Multiply( Matrix3 left, Matrix3 right ) {
			Matrix3 result;
			Multiply( ref left, ref right, out result );
			return result;
		}

		public static Matrix3 operator *( Matrix3 left, Matrix3 right ) {
			return Multiply( left, right );
		}

		public static Vector3 Multiply( Vector3 l, Matrix3 r ) {
			return new Vector3(
				l.X * r.M00 + l.Y * r.M01 + l.Z * r.M02,
				l.X * r.M10 + l.Y * r.M11 + l.Z * r.M12,
				l.X * r.M20 + l.Y * r.M21 + l.Z * r.M22
			);
		}

		public static Vector3 operator *( Vector3 left, Matrix3 right ) {
			return Multiply( left, right );
		}

		public static Vector3 Multiply( Matrix3 l, Vector3 r ) {
			return new Vector3(
				r.X * l.M00 + r.Y * l.M10 + r.Z * l.M20,
				r.X * l.M01 + r.Y * l.M11 + r.Z * l.M21,
				r.X * l.M02 + r.Y * l.M12 + r.Z * l.M22
			);
		}

		public static Vector3 operator *( Matrix3 left, Vector3 right ) {
			return Multiply( left, right );
		}

		#endregion

		#region Addition
		public static void Add( ref Matrix3 l, ref Matrix3 r, out Matrix3 result ) {
			result = new Matrix3 {
				M00 = ( l.M00 + r.M00 ),
				M01 = ( l.M01 + r.M01 ),
				M02 = ( l.M02 + r.M02 ),
				M10 = ( l.M10 + r.M10 ),
				M11 = ( l.M11 + r.M11 ),
				M12 = ( l.M12 + r.M12 ),
				M20 = ( l.M20 + r.M20 ),
				M21 = ( l.M21 + r.M21 ),
				M22 = ( l.M22 + r.M22 ),
			};
		}

		public static Matrix3 Add( Matrix3 left, Matrix3 right ) {
			Matrix3 result;
			Add( ref left, ref right, out result );
			return result;
		}

		public static Matrix3 operator +( Matrix3 left, Matrix3 right ) {
			return Add( left, right );
		}
		#endregion

		#region Invert

		public static void Invert( ref Matrix3 mat, out Matrix3 result ) {
			// https://stackoverflow.com/questions/983999/simple-3x3-matrix-inverse-code-c
			// /---------\
			// | a  b  c |
			// | d  e  f |
			// | g  h  i |
			// \---------/
			// a( ei − fh ) − b( di − fg ) + c( dh − eg )
			//            /-------------------\
			//            | |e f| |d f| |d e| |
			//            | |h i| |g i| |g h| |
			//            |                   |    /----------\   /-------\
			//   -1    1  | |b c| |a c| |a b| |    | s0 s1 s2 |	  | + - + |
			//  A  =   -  | |h i| |g i| |g h| | -> | s3 s4 s5 | * | - + - | 
			//        |A| |                   |	   | s6 s7 s8 |	  | + - + |
			//            | |b c| |a c| |a b| |	   \----------/	  \-------/
			//            | |e f| |d f| |d e| |
			//            \-------------------/
			// s0 = e * i - h * f;
			// s1 = d * i - g * f;
			// s2 = d * h - g * e;
			// s3 = b * i - h * c;
			// s4 = a * i - g * c;
			// s5 = a * h - g * b;
			// s6 = b * f - e * c;
			// s7 = a * f - d * c;
			// s8 = a * e - d * b;
			// /---------------------\
			// | a=M00  b=M01  c=M02 |
			// |                     |
			// | d=M10  e=M11  f=M12 |
			// |                     |
			// | g=M20  h=M21  i=M22 |
			// \---------------------/
			// s0 = M11 * M22 - M21 * M12;
			// s1 = M10 * M22 - M20 * M12;
			// s2 = M10 * M21 - M20 * M11;
			// s3 = M01 * M22 - M21 * M02;
			// s4 = M00 * M22 - M20 * M02;
			// s5 = M00 * M21 - M20 * M01;
			// s6 = M01 * M12 - M11 * M02;
			// s7 = M00 * M12 - M10 * M02;
			// s8 = M00 * M11 - M10 * M01;

			float s0 = mat.M11 * mat.M22 - mat.M21 * mat.M12;
			float s1 = mat.M10 * mat.M22 - mat.M20 * mat.M12;
			float s2 = mat.M10 * mat.M21 - mat.M20 * mat.M11;
			float det = mat.M00 * s0 - mat.M01 * s1 + mat.M02 * s2;
			if( det == 0 ) {
				result = Zero;
				return;
			}

			float inverseDeterminant = 1.0f / mat.Determinant;

			float s3 = mat.M01 * mat.M22 - mat.M21 * mat.M02;
			float s4 = mat.M00 * mat.M22 - mat.M20 * mat.M02;
			float s5 = mat.M00 * mat.M21 - mat.M20 * mat.M01;
			float s6 = mat.M01 * mat.M12 - mat.M11 * mat.M02;
			float s7 = mat.M00 * mat.M12 - mat.M10 * mat.M02;
			float s8 = mat.M00 * mat.M11 - mat.M10 * mat.M01;

			result = new Matrix3 {
				M00 = s0 * inverseDeterminant,
				M01 = -s1 * inverseDeterminant,
				M02 = s2 * inverseDeterminant,
				M10 = -s3 * inverseDeterminant,
				M11 = s4 * inverseDeterminant,
				M12 = -s5 * inverseDeterminant,
				M20 = s6 * inverseDeterminant,
				M21 = -s7 * inverseDeterminant,
				M22 = s8 * inverseDeterminant,
			};
		}

		public static Matrix3 Invert( ref Matrix3 mat ) {
			Invert( ref mat, out Matrix3 r );
			return r;
		}

		public static Matrix3 Invert( Matrix3 mat ) {
			Invert( ref mat, out Matrix3 r );
			return r;
		}

		#endregion

		#region Transpose
		public static void Transpose( ref Matrix3 mat, out Matrix3 result ) {
			result = new Matrix3() {
				Row0 = mat.Col0,
				Row1 = mat.Col1,
				Row2 = mat.Col2
			};
		}

		public static Matrix3 Transpose( Matrix3 mat ) {
			Transpose( ref mat, out Matrix3 result );
			return result;
		}

		public Matrix3 Transposed() {
			return Transpose( this );
		}
		#endregion

		#region Overides and Equals

		public override string ToString() {
			return $"{Row0}\n{Row1}\n{Row2}";
		}

		public override int GetHashCode() {
			return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode();
		}

		public override bool Equals( object obj ) {
			if( !( obj is Matrix3 ) ) {
				return false;
			}

			return this.Equals( (Matrix3) obj );
		}

		public bool Equals( Matrix3 other ) {
			return
				M00 == other.M00 &&
				M01 == other.M01 &&
				M02 == other.M02 &&
				M10 == other.M10 &&
				M11 == other.M11 &&
				M12 == other.M12 &&
				M20 == other.M20 &&
				M21 == other.M21 &&
				M22 == other.M22;
		}

		#endregion

		#region Operators and Implicit

		public static bool operator ==( Matrix3 left, Matrix3 right ) {
			return left.Equals( right );
		}

		public static bool operator !=( Matrix3 left, Matrix3 right ) {
			return !left.Equals( right );
		}


		#region Implicit
		public static implicit operator Matrix3( float[] a ) {
			if( a.Length != 9 )
				throw new System.ArgumentException( "Length of array is not compatible with Matrix3x3" );
			return new Matrix3(
				a[ 0 ], a[ 1 ], a[ 2 ],
				a[ 3 ], a[ 4 ], a[ 5 ],
				a[ 6 ], a[ 7 ], a[ 8 ] );
		}

		public static implicit operator float[]( Matrix3 a ) {
			return new float[] {
				a.M00, a.M01, a.M02,
				a.M10, a.M11, a.M12,
				a.M20, a.M21, a.M22
			};
		}
		#endregion

		#endregion

	}
}
