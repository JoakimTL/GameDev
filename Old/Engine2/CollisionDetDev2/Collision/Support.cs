using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public struct Support {

		public Vector2 A { get; private set; }
		public Vector2 B { get; private set; }
		public Vector2 Sum { get; private set; }

		public Support( Vector2 a, Vector2 b ) {
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
			if( obj is Support c )
				return EqualsInternal( c );
			return false;
		}

		private bool EqualsInternal( Support c ) {
			return c.A == A && c.B == B;
		}

	}
}
