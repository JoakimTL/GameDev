//using Engine.Graphics.Objects;
//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old.D3 {
//	public class Physics3Model : BaseModel<PointMass<Vector3>, Vector3> {
//		private bool CheckAABBCollisionInternal( Physics3Model other ) {
//			GetAABB( out Vector3 minA, out Vector3 maxA );
//			other.GetAABB( out Vector3 minB, out Vector3 maxB );
//			return ( maxA.X >= minB.X && maxB.X >= minA.X && maxA.Y >= minB.Y && maxB.Y >= minA.Y && maxA.Z >= minB.Z && maxB.Z >= minA.Z );
//		}

//		public bool CheckAABBCollision( Physics3Model other ) {
//			if( ShapeCount == 0 || other.ShapeCount == 0 )
//				return false;
//			UpdateShapes();
//			other.UpdateShapes();
//			return CheckAABBCollisionInternal( other );
//		}

//		public CollisionResult<Vector3> CheckCollision( BaseModel<PointMass<Vector3>, Vector3> other, bool epa, bool aabb, bool dist ) {
//			CollisionResult<Vector3> data = new CollisionResult<Vector3>( false );
//			//data.AddPairs( pairs );
//			return data;
//		}

//		public void GetAABB( out Vector3 min, out Vector3 max ) {
//			min = max = 0;
//			if( ShapeCount == 0 )
//				return;
//			Shapes[ 0 ].GetAABB( out min, out max );
//			for( int i = 1; i < ShapeCount; i++ ) {
//				Shapes[ i ].GetAABB( out Vector3 nmin, out Vector3 nmax );
//				min.X = Math.Min( min.X, nmin.X );
//				min.Y = Math.Min( min.Y, nmin.Y );
//				min.Z = Math.Min( min.Z, nmin.Z );
//				max.X = Math.Max( max.X, nmax.X );
//				max.Y = Math.Max( max.Y, nmax.Y );
//				max.Z = Math.Max( max.Z, nmax.Z );
//			}
//		}
//	}
//}
