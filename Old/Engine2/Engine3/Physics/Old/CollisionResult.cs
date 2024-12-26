//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old {
//	public class CollisionResult<V> {

//		public bool Colliding { get; private set; }
//		public IReadOnlyList<CollisionData> Data { get; private set; }

//		public CollisionResult( bool collides, params CollisionData[] data ) {
//			Colliding = collides;
//			Data = new List<CollisionData>( data );
//		}

//		public static CollisionData GetDeepest( CollisionResult<V> data ) {
//			if( !data.Colliding || data.Data.Count == 0 )
//				return new CollisionData( 0, default, default, default );
//			CollisionData deepest = data.Data[ 0 ];
//			float dep = deepest.Depth;
//			for( int i = 0; i < data.Data.Count; i++ ) {
//				float nDep = data.Data[ i ].Depth;
//				if( nDep < dep ) {
//					dep = nDep;
//					deepest = data.Data[ i ];
//				}
//			}
//			return deepest;
//		}


//		public struct CollisionData {
//			public float Depth { get; private set; }
//			public V Normal { get; private set; }
//			public V A { get; private set; }
//			public V B { get; private set; }

//			internal CollisionData( float depth, V normal, V a, V b ) {
//				Depth = depth;
//				Normal = normal;
//				A = a;
//				B = b;
//			}

//			public override string ToString() {
//				return $"[{Normal}:{Depth}]/[{A},{B}]";
//			}
//		}
//	}
//}
