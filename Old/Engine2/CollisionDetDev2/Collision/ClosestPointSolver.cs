using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	/*public static class ClosestPointSolver {
		// Modified GJK to find the closest edge to origo in minkowski space.
		// The loop terminates when the algorithm is no longer capable of adding a vertex tahn produces an edge closer than previous
		// The loop maintains a triangle, and adds new supports from the current closest edge (GJK & EPA combined)
		public static Support FindClosestOnLine( Vector2 a, Vector2 b, Vector2 p ) {
			Vector2 ab = b - a;
			Vector2 ap = p - a;
			float abLS = ab.LengthSquared;
			float product = Vector2.Dot( ap, ab );
			float distance = product / abLS;
			if( distance > 0 ) {
				if( distance < 1 ) {
					return new Support( a + ab * distance, p );
				} else {
					return new Support( b, p );
				}
			} else {
				return new Support( a, p );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="simplex"></param>
		/// <param name="shapeA"></param>
		/// <param name="shapeB"></param>
		/// <param name="iteration"></param>
		/// <param name="result"></param>
		/// <param name="log"></param>
		/// <returns>A number based on the success of the iteration. If the return is -1 the loop should be terminates, 0 means this iteration didn't find any result, but the loop should continue and 1 means a resuløt was found.</returns>
		public static int PerformStep( Simplex simplex, Shape<Vector2> shapeA, Shape<Vector2> shapeB, int iteration, out CollisionResult result, bool log ) {
			if( log )
				Console.WriteLine( "ClosestPointSolver step..." );
			result = null;
			if( simplex.Collides )
				return -1;

			shapeA.GetClosest( shapeB.GetInitial(), out Vector2 clA, out Vector2 nclA );
			shapeB.GetClosest( shapeA.GetInitial(), out Vector2 clB, out Vector2 nclB );

			Support s1 = new Support( clA, clB );
			Support s2 = new Support( nclA, nclB );

			/*if( simplex.Count == 0 ) {
				simplex.AddInitialSupport( shapeA, shapeB );
			}

			if( simplex.Count == 1 ) {
				Vector2 dir = -simplex[ 0 ].Sum;
				simplex.AddSupport( shapeA, shapeB, dir );
				if( simplex.RemoveSurplus() ) {
					if( log )
						Console.WriteLine( "failed 1" );
					GetClosest( simplex, log, out float distance, out Vector2 normal, out _ );
					result = new CollisionResult( iteration, normal, distance );
					return 1;
				}
				return 0;
			}

			if( simplex.Count == 2 ) {
				Vector2 b0 = -simplex[ 1 ].Sum;
				Vector2 ba = simplex[ 0 ].Sum + b0;
				Vector2 dir = ba.Perpendicular;
				if( Vector2.Dot( dir, b0 ) < 0 )
					dir = -dir;
				simplex.AddSupport( shapeA, shapeB, dir );
				if( simplex.RemoveSurplus() ) {
					if( log )
						Console.WriteLine( "failed 2" );
					GetClosest( simplex, log, out float distance, out Vector2 normal, out _ );
					result = new CollisionResult( iteration, normal, distance );
					return 1;
				}
				return 0;
			}

			/*{
				if( !GetClosest( simplex, log, out float distance, out Vector2 normal, out int index ) )
					return -1;
				//Compare closest edge to a new potential closest edge.
				Support sup = simplex.GetSupport( shapeA, shapeB, normal );

				double d = Vector2.Dot( sup.Sum, normal );
				//if( log )
				//	Console.WriteLine( "d-d: " + d + " - " + closest.distance + " = " + ( d - closest.distance ) );
				if( d - distance < 0.001 ) { //0.01 is a sufficiently small enough number to counteract the precision errors present with floating point numbers. If d is lower than 0.01 then the edge we're looking at is closest to the origin.
											 //if( log )
											 //	Console.WriteLine( "Found CLOSEST!" );
					result = new CollisionResult( iteration, normal, distance );
					return 1;
				} else {
					//The closest edge in the simplex is not closest in the minkowski sum. We need to add this new point to the simplex.
					simplex.InsertSupport( index, sup );
					//simplex.MaintainConvexity( log );
					return 0;
				}
			}*/
			//Simplex is now at least a line, which means we can find the line closest to the origo in the simplex.

			//Determine if the points all encapsulate the origin or not.
			/*Vector2 a0 = -simplex[ 0 ].Sum;
			Vector2 b0 = -simplex[ 1 ].Sum;
			Vector2 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
			Vector2 ac = simplex[ 2 ].Sum - simplex[ 0 ].Sum;
			Vector2 bc = simplex[ 2 ].Sum - simplex[ 1 ].Sum;
			Vector2 abP = Vector2.TripleProduct( ac, ab, ab );
			Vector2 acP = Vector2.TripleProduct( ab, ac, ac );
			Vector2 bcP = Vector2.TripleProduct( -ab, bc, bc );
			if( Vector2.Dot( a0, abP ) > 0 ) {
				simplex.Remove( 2 );
			} else if( Vector2.Dot( a0, acP ) > 0 ) {
				simplex.Remove( 1 );
			} else if( Vector2.Dot( b0, bcP ) > 0 ) {
				simplex.Remove( 0 );
			} else {
				if( log )
					Console.WriteLine( "failed 3" );
				simplex.RemoveSurplus();
				result = FindClosest( simplex, log );
				return 1;
			}*/

			/*Vector2 a = simplex[ 0 ].Sum;
			Vector2 b = simplex[ 1 ].Sum;
			Vector2 c = simplex[ 2 ].Sum;

			Vector2 ab = b - a;
			Vector2 ac = c - a;
			Vector2 bc = c - b;
			Vector2 abP = Vector2.TripleProduct( ac, ab, ab );
			Vector2 acP = Vector2.TripleProduct( ab, ac, ac );
			Vector2 bcP = Vector2.TripleProduct( -ab, bc, bc );

			float dAB = Vector2.Dot( -simplex[ 0 ].Sum, abP );
			float dBC = Vector2.Dot( -simplex[ 1 ].Sum, bcP );
			float dAC = Vector2.Dot( -simplex[ 2 ].Sum, acP );

			if( log ) {
				Console.WriteLine( "dab: " + dAB );
				Console.WriteLine( "dac: " + dAC );
				Console.WriteLine( "dbc: " + dBC );
			}

			if( dAB < dBC && dAB < dAC ) {
				float lenA = a.LengthSquared * -dAC; // * dAB
				float lenB = b.LengthSquared * -dBC; // * dAB, but both contain so it's equalled out and not neccessary
				if( lenB < lenA ) {
					simplex.Remove( 0 );
				} else {
					simplex.Remove( 1 );
				}
			} else if( dBC < dAB && dBC < dAC ) {
				float lenB = b.LengthSquared * -dAB;
				float lenC = c.LengthSquared * -dAC;
				if( lenB < lenC ) {
					simplex.Remove( 1 );
				} else {
					simplex.Remove( 2 );
				}
			} else if( dAC < dAB && dAC < dAB ) {
				float lenA = a.LengthSquared * -dAB;
				float lenC = c.LengthSquared * -dBC;
				if( lenA < lenC ) {
					simplex.Remove( 2 );
				} else {
					simplex.Remove( 0 );
				}
			}
			return 0;
		}

		public static bool GetClosest( Simplex simplex, bool log, out float distance, out Vector2 direction, out int index ) {
			//if( log )
			//	Console.WriteLine( "Getting closest!" );

			distance = float.MaxValue;
			direction = 0;
			index = -1;

			if( simplex.Count == 0 )
				return false;

			for( int i = 0; i < simplex.Count; i++ ) {
				int j = i + 1;
				j = j == simplex.Count ? 0 : j;
				Support a = simplex[ i ];
				Support b = simplex[ j ];

				Vector2 ab = b.Sum - a.Sum;
				Vector2 n = Vector2.TripleProduct( ab, -a.Sum, ab ).Normalized;
				float d = Vector2.Dot( n, a.Sum );
				if( d < distance ) {
					distance = d;
					direction = n;
					index = j;
				}
			}
			return true;
		}
	}*/
}
