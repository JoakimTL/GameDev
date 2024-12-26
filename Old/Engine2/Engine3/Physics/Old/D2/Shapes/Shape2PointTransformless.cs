//using Engine.Graphics.Objects;
//using Engine.Graphics.Objects.Default.Transforms;
//using Engine.LinearAlgebra;
//using OpenGL;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old.D2.Shapes {
//	public class Shape2PointTransformless : BaseShape<Vector2> {

//		public Vector2 Point { get; private set; }

//		public Shape2PointTransformless( BaseModel<Vector2> model ) : base( model ) {
//			Point = new Vector2();
//		}

//		public void Set( Vector2 p ) {
//			Point = p;
//		}

//		public override void GetAABB( out Vector2 min, out Vector2 max ) {
//			min = max = Point;
//		}

//		public override Vector2 GetFurthest( Vector2 dir ) {
//			return Point;
//		}

//		public override void GetClosest( Vector2 target, out Vector2 closest, out Vector2 nextClosest ) {
//			closest = Point;
//			nextClosest = Point;
//		}

//		public override Vector2 GetInitial() {
//			return Point;
//		}

//		internal override void RemovedFromModel() {

//		}

//		internal override void UpdateShape() {

//		}

//		internal override void Dispose() { }
//	}
//}
