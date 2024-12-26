//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old.Collision {
//	public class CollisionData<T> {

//		private List<CollisionDataNode> pointPairs;
//		public IReadOnlyList<CollisionDataNode> PointPairs { get => pointPairs; }
//		public bool Colliding { get; private set; }

//		public CollisionData( bool colliding ) {
//			pointPairs = new List<CollisionDataNode>();
//			Colliding = colliding;
//		}

//		internal void AddPair( CollisionDataNode pair ) {
//			pointPairs.Add( pair );
//		}

//		internal void AddPairs( List<CollisionDataNode> pairs ) {
//			pointPairs.AddRange( pairs );
//		}

//		public static CollisionData<Vector2>.CollisionDataNode GetDeepest( CollisionData<Vector2> data ) {
//			if( !data.Colliding || data.pointPairs.Count == 0 )
//				return new CollisionData<Vector2>.CollisionDataNode( 0, 0, 0, 0 );
//			CollisionData<Vector2>.CollisionDataNode deepest = data.pointPairs[ 0 ];
//			float dep = deepest.Depth;
//			for( int i = 0; i < data.pointPairs.Count; i++ ) {
//				float nDep = data.pointPairs[ i ].Depth;
//				if( nDep < dep ) {
//					dep = nDep;
//					deepest = data.pointPairs[ i ];
//				}
//			}
//			return deepest;
//		}

//		public static CollisionData<Vector3>.CollisionDataNode GetDeepest( CollisionData<Vector3> data ) {
//			if( !data.Colliding || data.pointPairs.Count == 0 )
//				return new CollisionData<Vector3>.CollisionDataNode( 0, 0, 0, 0 );
//			float dist = ( data.pointPairs[ 0 ].B - data.pointPairs[ 0 ].A ).Length;
//			CollisionData<Vector3>.CollisionDataNode deepest = data.pointPairs[ 0 ];
//			float dep = deepest.Depth;
//			for( int i = 0; i < data.pointPairs.Count; i++ ) {
//				float nDep = data.pointPairs[ i ].Depth;
//				if( nDep < dep ) {
//					dep = nDep;
//					deepest = data.pointPairs[ i ];
//				}
//			}
//			return deepest;
//		}

//		public struct CollisionDataNode {
//			public float Depth { get; private set; }
//			public T Normal { get; private set; }
//			public T A { get; private set; }
//			public T B { get; private set; }

//			internal CollisionDataNode( float depth, T normal, T a, T b ) {
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
