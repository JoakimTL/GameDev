namespace Engine.LinearAlgebra {
	public static class Matrix4Factory {


		public static Matrix4 CreateFromRows( Vector4 Row0, Vector4 Row1, Vector4 Row2, Vector4 Row3 ) {
			return new Matrix4() { Row0 = Row0, Row1 = Row1, Row2 = Row2, Row3 = Row3 };
		}

		public static Matrix4 CreateFromColumns( Vector4 Col0, Vector4 Col1, Vector4 Col2, Vector4 Col3 ) {
			return new Matrix4() { Col0 = Col0, Col1 = Col1, Col2 = Col2, Col3 = Col3 };
		}

		public static void CreateFromAxisAngle( Vector3 unormalizedaxis, float angle, out Matrix4 result ) {
			Vector3 axis = unormalizedaxis.Normalized;
			float axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;
			
			float cos = (float) System.Math.Cos( -angle );
			float sin = (float) System.Math.Sin( -angle );
			float t = 1.0f - cos;
			
			float tXX = t * axisX * axisX,
			tXY = t * axisX * axisY,
			tXZ = t * axisX * axisZ,
			tYY = t * axisY * axisY,
			tYZ = t * axisY * axisZ,
			tZZ = t * axisZ * axisZ;

			float sinX = sin * axisX,
			sinY = sin * axisY,
			sinZ = sin * axisZ;

			result = new Matrix4() {
				Row0 = new Vector4( tXX + cos, tXY - sinZ, tXZ + sinY, 0 ),
				Row1 = new Vector4( tXY + sinZ, tYY + cos, tYZ - sinX, 0 ),
				Row2 = new Vector4( tXZ - sinY, tYZ + sinX, tZZ + cos, 0 ),
				Row3 = new Vector4( 0, 0, 0, 1 )
			};
		}

		public static Matrix4 CreateFromAxisAngle( Vector3 unormalizedaxis, float angle ) {
			CreateFromAxisAngle( unormalizedaxis, angle, out Matrix4 r );
			return r;
		}

		
		public static void CreateFromQuaternion( ref Quaternion q, out Matrix4 result ) {
			q.ToAxisAngle( out Vector3 axis, out float angle );
			CreateFromAxisAngle( axis, angle, out result );
		}

		public static Matrix4 CreateFromQuaternion( ref Quaternion q ) {
			CreateFromQuaternion( ref q, out Matrix4 r );
			return r;
		}

		public static Matrix4 CreateFromQuaternion( Quaternion q ) {
			CreateFromQuaternion( ref q, out Matrix4 r );
			return r;
		}

		public static void CreateRotationX( float angle, out Matrix4 result ) {
			float cos = (float) System.Math.Cos( angle );
			float sin = (float) System.Math.Sin( angle );

			result = Matrix4.Identity;
			result.M11 = cos;
			result.M12 = sin;
			result.M21 = -sin;
			result.M22 = cos;
		}

		public static Matrix4 CreateRotationX( float angle ) {
			CreateRotationX( angle, out Matrix4 r );
			return r;
		}

		public static void CreateRotationY( float angle, out Matrix4 result ) {
			float cos = (float) System.Math.Cos( angle );
			float sin = (float) System.Math.Sin( angle );

			result = Matrix4.Identity;
			result.M00 = cos;
			result.M02 = -sin;
			result.M20 = sin;
			result.M22 = cos;
		}

		public static Matrix4 CreateRotationY( float angle ) {
			CreateRotationY( angle, out Matrix4 r );
			return r;
		}

		public static void CreateRotationZ( float angle, out Matrix4 result ) {
			float cos = (float) System.Math.Cos( angle );
			float sin = (float) System.Math.Sin( angle );

			result = Matrix4.Identity;
			result.M00 = cos;
			result.M01 = sin;
			result.M10 = -sin;
			result.M11 = cos;
		}

		public static Matrix4 CreateRotationZ( float angle ) {
			CreateRotationZ( angle, out Matrix4 r );
			return r;
		}

		public static void CreateTranslation( float x, float y, float z, out Matrix4 result ) {
			result = Matrix4.Identity;
			result.M30 = x;
			result.M31 = y;
			result.M32 = z;
		}

		public static Matrix4 CreateTranslation( float x, float y, float z ) {
			CreateTranslation( x, y, z, out Matrix4 r );
			return r;
		}

		public static void CreateTranslation( ref Vector3 vector, out Matrix4 result ) {
			CreateTranslation( vector.X, vector.Y, vector.Z, out result );
		}

		public static Matrix4 CreateTranslation( ref Vector3 vector ) {
			CreateTranslation( ref vector, out Matrix4 r );
			return r;
		}

		public static void CreateTranslation( Vector3 vector, out Matrix4 result ) {
			CreateTranslation( vector.X, vector.Y, vector.Z, out result );
		}

		public static Matrix4 CreateTranslation( Vector3 vector ) {
			CreateTranslation( ref vector, out Matrix4 r );
			return r;
		}

		public static void CreateScale( float scale, out Matrix4 result ) {
			CreateScale( scale, scale, scale, out result );
		}

		public static Matrix4 CreateScale( float scale ) {
			CreateScale( scale, out Matrix4 r );
			return r;
		}

		public static void CreateScale( ref Vector3 scale, out Matrix4 result ) {
			CreateScale( scale.X, scale.Y, scale.Z, out result );
		}

		public static Matrix4 CreateScale( ref Vector3 scale ) {
			CreateScale( ref scale, out Matrix4 r );
			return r;
		}

		public static void CreateScale( Vector3 scale, out Matrix4 result ) {
			CreateScale( scale.X, scale.Y, scale.Z, out result );
		}

		public static Matrix4 CreateScale( Vector3 scale ) {
			CreateScale( scale, out Matrix4 r );
			return r;
		}

		public static void CreateScale( float x, float y, float z, out Matrix4 result ) {
			result = Matrix4.Identity;
			result.M00 = x;
			result.M11 = y;
			result.M22 = z;
		}

		public static Matrix4 CreateScale( float x, float y, float z ) {
			CreateScale( x, y, z, out Matrix4 r );
			return r;
		}

		public static void CreateOrthographic( float width, float height, float zNear, float zFar, out Matrix4 result ) {
			CreateOrthographicOffCenter( -width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result );
		}

		public static Matrix4 CreateOrthographic( float width, float height, float zNear, float zFar ) {
			CreateOrthographic( width, height, zNear, zFar, out Matrix4 r );
			return r;
		}

		public static void CreateOrthographicOffCenter( float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4 result ) {
			result = Matrix4.Identity;

			float invRL = 1.0f / ( right - left );
			float invTB = 1.0f / ( top - bottom );
			float invFN = 1.0f / ( zFar - zNear );

			result.M00 = 2 * invRL;
			result.M11 = 2 * invTB;
			result.M22 = -2 * invFN;

			result.M30 = -( right + left ) * invRL;
			result.M31 = -( top + bottom ) * invTB;
			result.M32 = -( zFar + zNear ) * invFN;
		}

		public static Matrix4 CreateOrthographicOffCenter( float left, float right, float bottom, float top, float zNear, float zFar ) {
			CreateOrthographicOffCenter( left, right, bottom, top, zNear, zFar, out Matrix4 r );
			return r;
		}

		public static void CreateEquirectangular( float fovy, float aspect, float zNear, float zFar, out Matrix4 result ) {
			if( fovy <= 0 || fovy > System.Math.PI ) {
				throw new System.ArgumentOutOfRangeException( "fovy" );
			}
			if( aspect <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "aspect" );
			}
			if( zNear <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "zNear" );
			}
			if( zFar <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "zFar" );
			}

			float top = zNear * (float) System.Math.Tan( 0.5f * fovy );
			float bottom = -top;
			float left = bottom * aspect;
			float right = top * aspect;
			float x = ( 2.0f * zNear ) / ( right - left );
			float y = ( 2.0f * zNear ) / ( top - bottom );
			float a = ( right + left ) / ( right - left );
			float b = ( top + bottom ) / ( top - bottom );
			float c = -( zFar + zNear ) / ( zFar - zNear );
			float d = -( 2.0f * zFar * zNear ) / ( zFar - zNear );

			result = new Matrix4() {
				M00 = x,
				M01 = 0,
				M02 = 0,
				M03 = 0,
				M10 = 0,
				M11 = y,
				M12 = 0,
				M13 = 0,
				M20 = a,
				M21 = b,
				M22 = c,
				M23 = -1,
				M30 = 0,
				M31 = 0,
				M32 = d,
				M33 = 0
			};
		}

		public static void CreatePerspectiveFieldOfView( float fovy, float aspect, float zNear, float zFar, out Matrix4 result ) {
			if( fovy <= 0 || fovy > System.Math.PI ) {
				throw new System.ArgumentOutOfRangeException( "fovy" );
			}
			if( aspect <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "aspect" );
			}
			if( zNear <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "zNear" );
			}
			if( zFar <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "zFar" );
			}

			float top = zNear * (float) System.Math.Tan( 0.5f * fovy );
			float bottom = -top;
			float left = bottom * aspect;
			float right = top * aspect;
			float x = ( 2.0f * zNear ) / ( right - left );
			float y = ( 2.0f * zNear ) / ( top - bottom );
			float a = ( right + left ) / ( right - left );
			float b = ( top + bottom ) / ( top - bottom );
			float c = -( zFar + zNear ) / ( zFar - zNear );
			float d = -( 2.0f * zFar * zNear ) / ( zFar - zNear );

			result = new Matrix4() {
				M00 = x,
				M01 = 0,
				M02 = 0,
				M03 = 0,
				M10 = 0,
				M11 = y,
				M12 = 0,
				M13 = 0,
				M20 = a,
				M21 = b,
				M22 = c,
				M23 = -1,
				M30 = 0,
				M31 = 0,
				M32 = d,
				M33 = 0
			};
		}

		public static Matrix4 CreatePerspectiveFieldOfView( float fovy, float aspect, float zNear, float zFar ) {
			CreatePerspectiveFieldOfView( fovy, aspect, zNear, zFar, out Matrix4 r );
			return r;
		}

		public static void CreatePerspectiveOffCenter( float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4 result ) {
			if( zNear <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "zNear" );
			}
			if( zFar <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "zFar" );
			}
			if( zNear >= zFar ) {
				throw new System.ArgumentOutOfRangeException( "zNear" );
			}

			float x = ( 2.0f * zNear ) / ( right - left );
			float y = ( 2.0f * zNear ) / ( top - bottom );
			float a = ( right + left ) / ( right - left );
			float b = ( top + bottom ) / ( top - bottom );
			float c = -( zFar + zNear ) / ( zFar - zNear );
			float d = -( 2.0f * zFar * zNear ) / ( zFar - zNear );

			result = new Matrix4() {
				M00 = x,
				M01 = 0,
				M02 = 0,
				M03 = 0,
				M10 = 0,
				M11 = y,
				M12 = 0,
				M13 = 0,
				M20 = a,
				M21 = b,
				M22 = c,
				M23 = -1,
				M30 = 0,
				M31 = 0,
				M32 = d,
				M33 = 0
			};
		}

		public static Matrix4 CreatePerspectiveOffCenter( float left, float right, float bottom, float top, float zNear, float zFar ) {
			CreatePerspectiveOffCenter( left, right, bottom, top, zNear, zFar, out Matrix4 r );
			return r;
		}

		public static void LookAt( ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Matrix4 result ) {
			Vector3 z = Vector3.Normalize( eye - target );
			Vector3 x = Vector3.Normalize( Vector3.Cross( up, z ) );
			Vector3 y = Vector3.Normalize( Vector3.Cross( z, x ) );

			result = new Matrix4 {
				M00 = x.X,
				M01 = y.X,
				M02 = z.X,
				M03 = 0,
				M10 = x.Y,
				M11 = y.Y,
				M12 = z.Y,
				M13 = 0,
				M20 = x.Z,
				M21 = y.Z,
				M22 = z.Z,
				M23 = 0,
				M30 = -( ( x.X * eye.X ) + ( x.Y * eye.Y ) + ( x.Z * eye.Z ) ),
				M31 = -( ( y.X * eye.X ) + ( y.Y * eye.Y ) + ( y.Z * eye.Z ) ),
				M32 = -( ( z.X * eye.X ) + ( z.Y * eye.Y ) + ( z.Z * eye.Z ) ),
				M33 = 1
			};
		}

		public static Matrix4 LookAt( ref Vector3 eye, ref Vector3 target, ref Vector3 up ) {
			LookAt( ref eye, ref target, ref up, out Matrix4 r );
			return r;
		}

		public static Matrix4 LookAt( Vector3 eye, Vector3 target, Vector3 up ) {
			LookAt( ref eye, ref target, ref up, out Matrix4 r );
			return r;
		}

	}
}
