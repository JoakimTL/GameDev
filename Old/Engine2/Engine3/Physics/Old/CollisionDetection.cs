//using Engine.LinearAlgebra;
//using Engine.Physics.D2;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old {
//	public static class CollisionDetection {

//		/// <summary>
//		/// Simply returns wether the two models overlap
//		/// </summary>
//		/// <typeparam name="P"></typeparam>
//		/// <typeparam name="V"></typeparam>
//		/// <param name="modelA"></param>
//		/// <param name="modelB"></param>
//		/// <param name="maxIterations"></param>
//		/// <returns></returns>
//		public static bool CheckColliding<P, V>( BaseModel<P, V> modelA, BaseModel<P, V> modelB, int maxIterations = 60 ) where P : PointMass<V> {
//			return false;
//		}

//		/// <summary>
//		/// Returns all the data in case of a collision. 
//		/// </summary>
//		/// <typeparam name="P"></typeparam>
//		/// <typeparam name="V"></typeparam>
//		/// <param name="modelA"></param>
//		/// <param name="modelB"></param>
//		/// <param name="maxIterations"></param>
//		/// <returns></returns>
//		public static CollisionResult<V> AnalyzeCollision<P, V>( BaseModel<P, V> modelA, BaseModel<P, V> modelB, int maxIterations = 60 ) where P : PointMass<V> {
//			return new CollisionResult<V>( false );
//		}

//		public static bool CheckAABBOverlap( BaseModel<PointMass<Vector2>, Vector2> modelA, BaseModel<PointMass<Vector2>, Vector2> modelB ) {
//			return false;
//		}

//	}
//}
