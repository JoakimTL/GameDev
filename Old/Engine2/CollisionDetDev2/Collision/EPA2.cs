using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public static class EPA2 {

		public const float EPSILON = 0.00006103515f; //2^-14

		public struct Edge {
			public int supA, supB;
			public float distance;
			public Vector2 normal;
		}

		public static int PerformStep( Hull2 simplex, PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, int iteration, out Collision2Result result, bool log ) {
			if( log )
				Console.WriteLine( "Performing EPA Collision step..." );
			result = null;

			//if( !simplex.Collides )
			//	return -1;
			//Find the closest edge to origin in minkowski space
			Edge closest = GetClosest( simplex, log );
			if( closest.supA == -1 || closest.supB == -1 )
				return -1;
			//Compare closest edge to a new potential closest edge.
			Support sup = simplex.GetSupport( shapeA, shapeB, closest.normal, out _ );

			double d = Vector2.Dot( sup.Sum, closest.normal );
			if( log )
				Console.WriteLine( "s-s: " + closest.supA + " -> " + closest.supB );
			if( log )
				Console.WriteLine( "n: " + closest.normal );
			if( log )
				Console.WriteLine( "d-d: " + d + " - " + closest.distance + " = " + ( d - closest.distance ) );
			if( log )
				Console.WriteLine( "w: " + simplex.GetWinding() );
			if( d - closest.distance < EPSILON ) {
				//0.01 is a sufficiently small enough number to counteract the precision errors present with floating point numbers. If d is lower than 0.01 then the edge we're looking at is closest to the origin.

				float interp = FindOnLine( simplex[ closest.supA ].Sum, simplex[ closest.supB ].Sum, 0 );
				Vector2 pointA = simplex[ closest.supA ].A * ( 1 - interp ) + simplex[ closest.supB ].A * interp;
				Vector2 pointB = simplex[ closest.supA ].B * ( 1 - interp ) + simplex[ closest.supB ].B * interp;

				result = new Collision2Result( closest.distance, closest.normal, pointA, pointB );
				return 1;
			} else {
				//The closest edge in the simplex is not closest in the minkowski sum. We need to add this new point to the simplex.
				simplex.InsertSupport( closest.supB, sup );
				MaintainConvexity( simplex, closest.supB + 1, log );
				//MaintainConvexity( simplex, closest.supB, log );
				MaintainConvexity( simplex, closest.supB - 1, log );
				return 0;
			}
		}

		public static void MaintainConvexity( Hull2 simplex, int i, bool log ) {
			if( simplex.Count > 3 ) {
				int winding = simplex.GetWinding();

				if( i == -1 )
					i = simplex.Count - 1;
				int j = i + 1;
				int k = i - 1;
				if( j == simplex.Count )
					j = 0;
				if( k == -1 )
					k = simplex.Count - 1;

				Vector2 acP = ( simplex[ j ].Sum - simplex[ k ].Sum ).Perpendicular * winding;
				Vector2 ab = ( simplex[ i ].Sum - simplex[ j ].Sum );
				//Uses the a -> center to determine if the perpendicular vector between ac is facing toward the center or away.
				bool concave = simplex[ j ].Sum == simplex[ k ].Sum || Vector2.Dot( acP, ab ) < -EPSILON;
				if( log )
					Console.WriteLine( i + ":" + acP + ":" + ab + " -> " + winding + ":" + Vector2.Dot( acP, ab ) * winding );
				if( concave ) {
					//Concave point found!
					simplex.Remove( i );
					if( log )
						Console.WriteLine( "concave: " + i );
				}

			}
		}

		private static float FindOnLine( Vector2 a, Vector2 b, Vector2 p ) {
			Vector2 ab = b - a;
			return Vector2.Dot( ab, p - a ) / ab.LengthSquared;
		}

		public static Edge GetClosest( Hull2 simplex, bool log ) {
			//if( log )
			//	Console.WriteLine( "Getting closest!" );

			Edge e = new Edge {
				distance = float.MaxValue,
				supA = -1,
				supB = -1
			};

			if( simplex.Count == 0 )
				return e;

			int winding = simplex.GetWinding();

			for( int i = 0; i < simplex.Count; i++ ) {
				int j = i + 1;
				j = j == simplex.Count ? 0 : j;
				Support a = simplex[ i ];
				Support b = simplex[ j ];

				Vector2 ab = b.Sum - a.Sum;
				Vector2 n = ab.Perpendicular.Normalized * winding;
				float d = Vector2.Dot( n, a.Sum );
				if( d < e.distance ) {
					e.distance = d;
					e.normal = n;
					e.supA = i;
					e.supB = j;
				}
			}
			return e;
		}

	}
}
