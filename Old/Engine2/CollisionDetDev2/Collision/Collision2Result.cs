using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public class Collision2Result {

		/// <summary>
		/// The direction of the penetration.
		/// </summary>
		public Vector2 PenetrationDirection { get; private set; }

		/// <summary>
		/// The depth of penetration, or length between the two shapes in this collision result.
		/// </summary>
		public float Depth { get; private set; }

		/// <summary>
		/// The point of collision on shape A.
		/// </summary>
		public Vector2 PointA { get; private set; }

		/// <summary>
		/// The point of collision on shape A.
		/// </summary>
		public Vector2 PointB { get; private set; }

		public Collision2Result( float depth, Vector2 penetrationDirection, Vector2 pointA, Vector2 pointB ) {
			Depth = depth;
			PenetrationDirection = penetrationDirection;
			PointA = pointA;
			PointB = pointB;
		}
	}
}
