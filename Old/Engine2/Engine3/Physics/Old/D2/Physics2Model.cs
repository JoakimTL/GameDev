//using Engine.Graphics.Objects;
//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old.D2 {
//	public class Physics2Model : BaseModel<PointMass<Vector2>, Vector2> {

//		private bool CheckAABBCollisionInternal( Physics2Model other ) {
//			GetAABB( out Vector2 minA, out Vector2 maxA );
//			other.GetAABB( out Vector2 minB, out Vector2 maxB );
//			return ( maxA.X >= minB.X && maxB.X >= minA.X && maxA.Y >= minB.Y && maxB.Y >= minA.Y );
//		}

//		public bool CheckAABBCollision( Physics2Model other ) {
//			if( ShapeCount == 0 || other.ShapeCount == 0 )
//				return false;
//			UpdateShapes();
//			other.UpdateShapes();
//			return CheckAABBCollisionInternal(other);
//		}

//		public CollisionResult<Vector2> CheckCollision( Physics2Model other, bool epa, bool aabb, bool dist ) {
//			if( ShapeCount == 0 || other.ShapeCount == 0 )
//				return new CollisionResult<Vector2>( false );

//			UpdateShapes();
//			other.UpdateShapes();
//			if( aabb )
//				if( !CheckAABBCollisionInternal( other ) )
//					return new CollisionResult<Vector2>( false );


//			List<CollisionResult<Vector2>.CollisionData> pairs = new List<CollisionResult<Vector2>.CollisionData>();
//			bool colliding = false;

//			/*if( ShapeCount == 1 && other.ShapeCount == 1 ) {
//				return GJK2.CheckCollision( Shapes[ 0 ], other.Shapes[ 0 ], epa, dist );
//			} else
//				for( int i = 0; i < ShapeCount; i++ )
//					for( int j = 0; j < other.ShapeCount; j++ ) {
//						var v = GJK2.CheckCollision( Shapes[ i ], other.Shapes[ j ], epa, dist );
//						if( (int) v.Collision > (int) mode ) {
//							pairs.Clear();
//							mode = v.Collision;
//						}
//						pairs.AddRange( v.PointPairs );
//					}
//			*/
//			CollisionResult<Vector2> data = new CollisionResult<Vector2>( colliding, pairs.ToArray() );
//			return data;
//		}

//		public void GetAABB( out Vector2 min, out Vector2 max ) {
//			min = max = 0;
//			if( ShapeCount == 0 )
//				return;
//			Shapes[ 0 ].GetAABB( out min, out max );
//			for( int i = 1; i < ShapeCount; i++ ) {
//				Shapes[ i ].GetAABB( out Vector2 nmin, out Vector2 nmax );
//				min.X = Math.Min( min.X, nmin.X );
//				min.Y = Math.Min( min.Y, nmin.Y );
//				max.X = Math.Max( max.X, nmax.X );
//				max.Y = Math.Max( max.Y, nmax.Y );
//			}
//		}
//	}
//}
