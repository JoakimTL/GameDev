using Engine.LMath;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace CollisionDetectionDev {
	internal class Simplex {

		public List<Component> comps;
		public bool failed;

		public Simplex() {
			comps = new List<Component>();
			failed = false;
		}


		public struct Component {
			public Vector2 a, b, sum;

			public Component( Vector2 a, Vector2 b ) {
				this.a = a;
				this.b = b;
				sum = a - b;
			}

			public override string ToString() {
				return a + ", " + b + " = " + sum;
			}

			public override int GetHashCode() {
				return sum.GetHashCode();
			}

			public override bool Equals( object obj ) {
				if( obj is Component c )
					return EqualsInternal( c );
				return false;
			}

			private bool EqualsInternal( Component c ) {
				return c.a == a && c.b == b;
			}
		}
	}
}