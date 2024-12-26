using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.D2.Shapes {
	public class Shape2Point : PhysicsShape<Transform2, Vector2> {

		private PhysicsModel<Transform2, Vector2> model;

		public override Vector2 CenterOfMass { get => PointTransformed; }
		internal override int UniquePoints => 1;

		public Vector2 Point { get; private set; }
		public Vector2 PointTransformed { get; private set; }


		private bool updateNeeded;

		public Shape2Point( float massProportion, Vector2 point ) : base( massProportion ) {
			Point = point;
			PointTransformed = Point;
			updateNeeded = true;
		}

		internal override void Added( PhysicsModel<Transform2, Vector2> model ) {
			this.model = model;
			model.Transform.OnChangedEvent += TransformChanged;
		}

		internal override void Removed( PhysicsModel<Transform2, Vector2> model ) {
			this.model = null;
			model.Transform.OnChangedEvent -= TransformChanged;
		}

		private void TransformChanged() {
			updateNeeded = true;
		}

		internal override bool UpdateShape() {
			if( !updateNeeded )
				return false;
			updateNeeded = false;
			if( model is null ) {
				PointTransformed = Point;
			} else {
				PointTransformed = Vector2.Transform( Point, model.Transform.Matrix );
			}
			return true;
		}

		public override void GetAABB( out Vector2 min, out Vector2 max ) {
			min = PointTransformed;
			max = PointTransformed;
		}

		public override float GetInertia( Vector2 dir, Vector2 centerOfMass ) {
			return ( CenterOfMass - centerOfMass ).LengthSquared;
		}

		public override Vector2 GetFurthest( Vector2 dir ) {
			return PointTransformed;
		}

		/*public override void GetClosest( Vector2 target, out Vector2 closest, out Vector2 nextClosest ) {
			closest = nextClosest = PointTransformed;
		}*/

		public override Vector2 GetInitial() {
			return PointTransformed;
		}

		internal override Vector2 GetUniquePoint( int id ) {
			return PointTransformed;
		}
	}
}
