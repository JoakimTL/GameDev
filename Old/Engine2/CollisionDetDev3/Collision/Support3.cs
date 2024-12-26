using Engine.LinearAlgebra;

namespace CollisionDetDev3.Collision {

	public struct Support3 {

		public Vector3 A { get; private set; }
		public Vector3 B { get; private set; }
		public Vector3 Sum { get; private set; }

		public Support3( Vector3 a, Vector3 b ) {
			A = a;
			B = b;
			Sum = a - b;
		}

		public override string ToString() {
			return A + " - " + B + " = " + Sum;
		}

		public override int GetHashCode() {
			return Sum.GetHashCode();
		}

		public override bool Equals( object obj ) {
			if( obj is Support3 c )
				return EqualsInternal( c );
			return false;
		}

		private bool EqualsInternal( Support3 c ) {
			return c.A == A && c.B == B;
		}

	}
}