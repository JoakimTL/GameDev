//using Engine.Graphics.Objects;
//using Engine.Graphics.Objects.Default.Transforms;
//using Engine.LinearAlgebra;
//using OpenGL;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old.D2.Shapes {
//	public class Shape2Point : BaseShape<Vector2> {

//		public Vector2 Point { get; private set; }
//		public Vector2 PointTransformed { get; private set; }
//		private bool updateNeeded;

//		public Shape2Point( BaseModel<Vector2> model ) : base( model ) {
//			Point = new Vector2();
//			PointTransformed = Point;
//			updateNeeded = true;
//			model.Transform.OnAnyChangedEvent += TransformChanged;
//		}

//		public void Set( Vector2 p ) {
//			Point = p;
//			TransformChanged();
//		}

//		private void TransformChanged() {
//			updateNeeded = true;
//		}

//		public override void GetAABB( out Vector2 min, out Vector2 max ) {
//			min = max = PointTransformed;
//		}

//		public override Vector2 GetFurthest( Vector2 dir ) {
//			return PointTransformed;
//		}

//		public override void GetClosest( Vector2 target, out Vector2 closest, out Vector2 nextClosest ) {
//			closest = PointTransformed;
//			nextClosest = PointTransformed;
//		}

//		public override Vector2 GetInitial() {
//			return PointTransformed;
//		}

//		internal override void RemovedFromModel() {
//			model.Transform.OnAnyChangedEvent -= TransformChanged;
//		}

//		internal override void UpdateShape() {
//			if( !updateNeeded )
//				return;
//			updateNeeded = false;
//			PointTransformed = Vector3.Transform( new Vector3( Point, 0 ), model.Transform.Matrix ).XY;
//		}
//		internal override void Dispose() { }
//	}
//}
