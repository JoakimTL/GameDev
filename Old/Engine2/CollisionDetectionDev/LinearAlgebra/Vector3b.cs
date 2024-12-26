using System;

namespace Engine.LMath {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector3b : System.IEquatable<Vector3b>, ILAMeasurable {

		public byte X, Y, Z;

		public Vector3 AsFloat => new Vector3( X, Y, Z );
		public Vector3i AsInt => new Vector3i( X, Y, Z );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z );
		public float LengthSquared => X * X + Y * Y + Z * Z;

		public Vector3b( byte x, byte y, byte z ) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3b( Vector2b a, byte z ) : this( a.X, a.Y, z ) { }

		public Vector3b( Vector3b a ) : this( a.X, a.Y, a.Z ) { }

		public Vector3b( byte s ) : this( s, s, s ) { }

		public static readonly Vector3b Zero = new Vector3b( 0 );

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

	}
}
