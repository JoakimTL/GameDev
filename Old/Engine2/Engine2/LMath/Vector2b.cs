namespace Engine.LMath {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector2b : System.IEquatable<Vector2b>, ILAMeasurable {

		public byte X, Y;

		public Vector2 AsFloat => new Vector2( X, Y );
		public Vector2i AsInt => new Vector2i( X, Y );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y );
		public float LengthSquared => X * X + Y * Y;

		public Vector2b( byte x, byte y ) {
			X = x;
			Y = y;
		}

		public Vector2b( Vector2b a ) : this( a.X, a.Y ) { }

		public Vector2b( byte s ) : this( s, s ) { }

		public static readonly Vector2b Zero = new Vector2b( 0 );

		#region Override and Equals

		public override bool Equals( object other ) {
			if( !( other is Vector2b ) )
				return false;
			return Equals( (Vector2b) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public bool Equals( Vector2b other ) {
			return X == other.X && Y == other.Y;
		}

		public override string ToString() {
			return $"[{X},{Y}]";
		}

		#endregion

	}
}
