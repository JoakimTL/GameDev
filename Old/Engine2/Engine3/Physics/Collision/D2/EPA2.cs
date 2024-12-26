using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision.D2 {
	public static class EPA2 {
		/// <summary>
		/// Because of floating point errors, a sufficiently small bias is used to counteract the errors. <br/>
		/// If two shapes are very close to colliding and this bias triggers a collision check the human eye couldn't spot the difference anyways<br/>
		/// In this engine, this bias is <b>~0.061mm (2^-14)</b>
		/// </summary>
		public const float EPSILON = 0.00006103515f;

		public struct Edge {
			public int A;
			public int B;
			public float distance;
			public Vector2 normal;
		}

		public static bool PerformStep( Hull2 simplex, PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, ref CollisionResult<Vector2>.CollisionData res ) {
			Edge closest = GetClosest( simplex );
			if( closest.A == -1 || closest.B == -1 )
				return true;

			Support2 sup = simplex.GetSupport( shapeA, shapeB, closest.normal, out _ );
			double d = Vector2.Dot( sup.Sum, closest.normal );

			if( d - closest.distance < EPSILON ) {
				float interp = FindOnLine( simplex[ closest.A ].Sum, simplex[ closest.B ].Sum, 0 );
				Vector2 pointA = simplex[ closest.A ].A * ( 1 - interp ) + simplex[ closest.B ].A * interp;
				Vector2 pointB = simplex[ closest.A ].B * ( 1 - interp ) + simplex[ closest.B ].B * interp;

				Vector2 aAB = ( simplex[ closest.B ].A - simplex[ closest.A ].A );
				float aABlensq = aAB.LengthSquared;
				Vector2 bAB = ( simplex[ closest.B ].B - simplex[ closest.A ].B );
				float bABlensq = bAB.LengthSquared;

				bool parallel = false;
				if( !( aABlensq == 0 || bABlensq == 0 ) )
					parallel = ( 1 - Math.Abs( Vector2.Dot( aAB / (float) Math.Sqrt( aABlensq ), bAB / (float) Math.Sqrt( bABlensq ) ) ) ) < EPSILON;

				res = new CollisionResult<Vector2>.CollisionData( closest.distance, closest.normal, pointA, pointB, parallel );
				return true;
			} else {
				//The closest edge in the hull is not closest in the minkowski sum. We need to add this new point to the simplex.
				simplex.InsertSupport( closest.B, sup );
				MaintainConvexity( simplex, closest.B + 1 );
				MaintainConvexity( simplex, closest.B - 1 );
				return false;
			}
		}

		public static void MaintainConvexity( Hull2 simplex, int i ) {
			if( simplex.Count > 3 ) {
				if( i == -1 )
					i = simplex.Count - 1;
				int j = i + 1;
				int k = i - 1;
				if( j == simplex.Count )
					j = 0;
				if( k == -1 )
					k = simplex.Count - 1;

				Vector2 acP = ( simplex[ j ].Sum - simplex[ k ].Sum ).Perpendicular * simplex.Winding;
				Vector2 ab = ( simplex[ i ].Sum - simplex[ j ].Sum );
				if( simplex[ j ].Sum == simplex[ k ].Sum || Vector2.Dot( acP, ab ) < -EPSILON ) {
					//Concave point found!
					simplex.Remove( i );
				}
			}
		}

		private static float FindOnLine( Vector2 a, Vector2 b, Vector2 p ) {
			Vector2 ab = b - a;
			return Vector2.Dot( ab, p - a ) / ab.LengthSquared;
		}

		public static Edge GetClosest( Hull2 simplex ) {
			Edge e = new Edge {
				distance = float.MaxValue,
				A = -1,
				B = -1
			};

			if( simplex.Count == 0 )
				return e;

			if( simplex.Count == 1 ) {
				Support2 a = simplex[ 0 ];
				Vector2 n = -a.Sum.Normalized;
				float d = Vector2.Dot( n, a.Sum );
				if( d < e.distance ) {
					e.distance = d;
					e.normal = n;
					e.A = 0;
					e.B = 0;
				}
				return e;
			}

			for( int i = 0; i < simplex.Count; i++ ) {
				int j = i + 1;
				j = j == simplex.Count ? 0 : j;
				Support2 a = simplex[ i ];
				Support2 b = simplex[ j ];

				Vector2 ab = b.Sum - a.Sum;
				Vector2 n = ab.Perpendicular.Normalized * simplex.Winding;
				float d = Vector2.Dot( n, a.Sum );
				if( d < e.distance ) {
					e.distance = d;
					e.normal = n;
					e.A = i;
					e.B = j;
				}
			}
			return e;
		}
	}
}
