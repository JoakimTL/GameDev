
namespace Engine.LinearAlgebra {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
	public struct Vector3b : System.IEquatable<Vector3b> {

		[System.Runtime.InteropServices.FieldOffset( 0 )]
		public byte X;
		[System.Runtime.InteropServices.FieldOffset( 1 )]
		public byte Y;
		[System.Runtime.InteropServices.FieldOffset( 2 )]
		public byte Z;

		public Vector3 AsFloat => new Vector3( X, Y, Z );
		public Vector3i AsInt => new Vector3i( X, Y, Z );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z );
		public int LengthSquared => X * X + Y * Y + Z * Z;

		public Vector3b( byte x, byte y, byte z ) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3b( Vector2b a, byte z ) : this( a.X, a.Y, z ) { }

		public Vector3b( Vector3b a ) : this( a.X, a.Y, a.Z ) { }

		public Vector3b( byte s ) : this( s, s, s ) { }

		public static readonly Vector3b Zero = new Vector3b( 0 );
		public static readonly Vector3b R = new Vector3b( 255, 0, 0 );
		public static readonly Vector3b G = new Vector3b( 0, 255, 0 );
		public static readonly Vector3b B = new Vector3b( 0, 0, 255 );
		public static readonly Vector3b Byte = new Vector3b( 255 );

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector3b == false )
				return false;
			return Equals( (Vector3b) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		public bool Equals( Vector3b other ) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override string ToString() {
			return $"[{X},{Y},{Z}]";
		}

		#endregion

		#region Operators
		public static Vector3b operator +( Vector3b a, Vector3b b ) {
			return new Vector3b( (byte) ( a.X + b.X ), (byte) ( a.Y + b.Y ), (byte) ( a.Z + b.Z ) );
		}

		public static Vector3b operator *( Vector3b a, byte scalar ) {
			return new Vector3b( (byte) ( a.X * scalar ), (byte) ( a.Y * scalar ), (byte) ( a.Z * scalar ) );
		}

		public static Vector3b operator *( byte scalar, Vector3b a ) {
			return new Vector3b( (byte) ( a.X * scalar ), (byte) ( a.Y * scalar ), (byte) ( a.Z * scalar ) );
		}

		public static Vector3b operator *( Vector3b a, float scalar ) {
			return new Vector3( a.X * scalar, a.Y * scalar, a.Z * scalar ).IntFloored.AsByte;
		}

		public static Vector3b operator *( float scalar, Vector3b a ) {
			return new Vector3( a.X * scalar, a.Y * scalar, a.Z * scalar ).IntFloored.AsByte;
		}

		public static Vector3b operator *( Vector3b a, Vector3b b ) {
			return new Vector3b( (byte) ( a.X * b.X ), (byte) ( a.Y * b.Y ), (byte) ( a.Z * b.Z ));
		}

		public static Vector3b operator *( Vector3b a, Vector4 b ) {
			return new Vector3( a.X * b.X, a.Y * b.Y, a.Z * b.Z).IntFloored.AsByte;
		}

		public static Vector3b operator *( Vector4 a, Vector3b b ) {
			return new Vector3( a.X * b.X, a.Y * b.Y, a.Z * b.Z).IntFloored.AsByte;
		}

		public static Vector3b operator -( Vector3b a, Vector3b b ) {
			return new Vector3b( (byte) ( a.X - b.X ), (byte) ( a.Y - b.Y ), (byte) ( a.Z - b.Z ) );
		}

		public static Vector3b operator /( Vector3b a, int divident ) {
			return ( a * ( 1f / divident ) );
		}

		public static Vector3b operator /( Vector3b a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Vector3b a, Vector3b b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector3b a, Vector3b b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector3b( byte[] a ) {
			if( a.Length != 3 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector4b" );
			return new Vector3b( a[ 0 ], a[ 1 ], a[ 2 ] );
		}

		public static implicit operator Vector3b( byte a ) {
			return new Vector3b( a );
		}

		public static implicit operator byte[]( Vector3b a ) {
			return new byte[] { a.X, a.Y, a.Z };
		}
		#endregion
		#endregion
	}
}
