

namespace CollisionDetDev3.Collision {
	public struct Face3 {

		public int A { get; private set; }
		public int B { get; private set; }
		public int C { get; private set; }

		public Face3( int a, int b, int c ) {
			A = a;
			B = b;
			C = c;
		}

	}
}