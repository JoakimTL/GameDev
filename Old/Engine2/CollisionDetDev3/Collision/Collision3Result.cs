using Engine.LinearAlgebra;

namespace CollisionDetDev3.Collision {
	public class Collision3Result {
		/// <summary>
		/// The amount of iterations required by the system to determine the collision result.
		/// </summary>
		public int IterationCount { get; private set; }

		/// <summary>
		/// The direction of the penetration.
		/// </summary>
		public Vector3 PenetrationDirection { get; private set; }

		/// <summary>
		/// Closest point of intersection on shape A.
		/// </summary>
		public Vector3 PointA { get; private set; }

		/// <summary>
		/// Closest point of intersection on shape B.
		/// </summary>
		public Vector3 PointB { get; private set; }

		/// <summary>
		/// The depth of penetration, or length between the two shapes in this collision result.
		/// </summary>
		public float Depth { get; private set; }

		public Collision3Result( int itCount, Vector3 penetrationDirection, float depth, Vector3 pointA, Vector3 pointB ) {
			IterationCount = itCount;
			Depth = depth;
			PointA = pointA;
			PointB = pointB;
			PenetrationDirection = penetrationDirection;
		}
	}
}