//using Engine.LinearAlgebra;

//namespace Engine.Physics.Old.Collision.D2 {
//	internal struct Support {
//		internal Vector2 a, b, sum;

//		internal Support( Vector2 a, Vector2 b ) {
//			this.a = a;
//			this.b = b;
//			sum = a - b;
//		}

//		public override string ToString() {
//			return a + ", " + b + " = " + sum;
//		}

//		public override int GetHashCode() {
//			return sum.GetHashCode();
//		}

//		public override bool Equals( object obj ) {
//			if( obj is Support c )
//				return EqualsInternal( c );
//			return false;
//		}

//		private bool EqualsInternal( Support c ) {
//			return c.a == a && c.b == b;
//		}
//	}
//}
