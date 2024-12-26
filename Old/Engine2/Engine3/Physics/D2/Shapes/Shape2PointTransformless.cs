using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.D2.Shapes {
	public class Shape2PointTransformless : PhysicsShape<Transform2, Vector2> {

		public override Vector2 CenterOfMass { get => Point; }
		internal override int UniquePoints => 1;
		public Vector2 Point { get; private set; }
		private bool updated;

		public Shape2PointTransformless( float massProportion, Vector2 point ) : base( massProportion ) {
			Point = point;
			updated = true;
		}

		public void Set( Vector2 p ) {
			Point = p;
			updated = true;
		}

		public override void GetAABB( out Vector2 min, out Vector2 max ) {
			min = max = Point;
		}

		public override Vector2 GetFurthest( Vector2 dir ) {
			return Point;
		}

		/*public override void GetClosest( Vector2 target, out Vector2 closest, out Vector2 nextClosest ) {
			closest = Point;
			nextClosest = Point;
		}*/

		public override Vector2 GetInitial() {
			return Point;
		}

		internal override Vector2 GetUniquePoint( int id ) {
			return Point;
		}

		public override float GetInertia( Vector2 dir, Vector2 centerOfMass ) {
			return ( CenterOfMass - centerOfMass ).LengthSquared;
		}

		internal override bool UpdateShape() {
			if( updated ) {
				updated = false;
				return true;
			}
			return false;
		}

		internal override void Added( PhysicsModel<Transform2, Vector2> model ) { }

		internal override void Removed( PhysicsModel<Transform2, Vector2> model ) { }
	}
}
