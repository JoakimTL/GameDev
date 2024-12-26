using Engine.LMath;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace CollisionDetectionDev {
	public class Result {
		public Vector2 a, b, sum;
		public bool inside;

		public Result( Vector2 a, Vector2 b, bool inside ) {
			this.a = a;
			this.b = b;
			sum = a - b;
			this.inside = inside;
		}

		public override string ToString() {
			return a + ", " + b + " = " + sum;
		}
	}
}