using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision {
	public class CollisionResult<V> {

		public bool Colliding { get; private set; }
		private List<CollisionData> data;
		public IReadOnlyList<CollisionData> Data { get => data; }

		public CollisionResult( bool collides, params CollisionData[] data ) {
			Colliding = collides;
			this.data = new List<CollisionData>( data );
		}

		public CollisionResult() {
			Colliding = false;
			data = new List<CollisionData>();
		}

		internal void SetCollisionState(bool state) {
			Colliding = state;
		}

		internal void Clear() {
			data.Clear();
			Colliding = false;
		}

		internal void Add( CollisionData dataPoint ) {
			data.Add( dataPoint );
		}

		public CollisionData GetDeepest() {
			return GetDeepest(this);
		}

		public static CollisionData GetDeepest( CollisionResult<V> data ) {
			if( !data.Colliding || data.Data.Count == 0 )
				return new CollisionData( 0, default, default, default, false );
			CollisionData deepest = data.Data[ 0 ];
			float dep = deepest.Depth;
			for( int i = 0; i < data.Data.Count; i++ ) {
				float nDep = data.Data[ i ].Depth;
				if( nDep < dep ) {
					dep = nDep;
					deepest = data.Data[ i ];
				}
			}
			return deepest;
		}

		public struct CollisionData {
			public float Depth { get; private set; }
			public V Normal { get; private set; }
			public V A { get; private set; }
			public V B { get; private set; }
			public bool Parallel { get; private set; }

			internal CollisionData( float depth, V normal, V a, V b, bool parallel ) {
				Depth = depth;
				Normal = normal;
				A = a;
				B = b;
				Parallel = parallel;
			}

			public override string ToString() {
				return $"[{Normal}:{Depth}]/[{A},{B}]";
			}
		}
	}
}
