using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public static class GJK2 {

		public const float EPSILON = 0.00006103515f; //2^-14

		public static int PerformStep( Hull2 hull, PhysicsShape<Transform2, Vector2> shape1, PhysicsShape<Transform2, Vector2> shape2, bool log ) {
			if( hull.Count == 0 ) {
				hull.AddInitialSupport( shape1, shape2 );
				hull.SearchDirection = -hull[ 0 ].Sum;
			}

			//Happens if the initial support lands on the origin.
			if( hull.SearchDirection == 0 )
				return 1;

			Support newSupport = hull.GetSupport( shape1, shape2, hull.SearchDirection, out bool valid );

			if( !valid || !CheckDuplicates( hull, newSupport ) )
				return -1;

			hull.InsertSupport( 0, newSupport );

			ExamineSimplex( hull, log );
			if (log)
			Console.WriteLine("w:" + hull.GetWinding());

			//The simplex is a triangle after the gjk loop step, meaning the origin is encapsulated.
			if( hull.Count == 3 )
				return 1;
			return 0;
		}

		private static bool CheckDuplicates( Hull2 hull, Support newSupport ) {
			for( int i = 0; i < hull.Count; i++ )
				if( hull[ i ].A == newSupport.A && hull[ i ].B == newSupport.B )
					return false;
			return true;
		}

		private static void ExamineSimplex( Hull2 hull, bool log ) {
			if( hull.Count == 3 ) {
				DoTriangleTest( hull, log );
			}

			switch( hull.Count ) {
				case 1: {
						hull.SearchDirection = -hull[ 0 ].Sum;
						break;
					}
				case 2: {
						Vector2 ab = hull[ 1 ].Sum - hull[ 0 ].Sum;
						Vector2 a0 = -hull[ 0 ].Sum;
						if( Vector2.Dot( ab, a0 ) > -EPSILON ) {
							Vector2 dir = ab.Perpendicular;
							if( Vector2.Dot( dir, a0 ) < 0 )
								dir = -dir;
							hull.SearchDirection = dir; // From the line towards the origin.
						} else {
							hull.SearchDirection = a0;
						}
						break;
					}
			}
		}


		private static void DoTriangleTest( Hull2 hull, bool log ) {
			Vector2 a0 = -hull[ 0 ].Sum;
			Vector2 ab = hull[ 1 ].Sum - hull[ 0 ].Sum;
			Vector2 ac = hull[ 2 ].Sum - hull[ 0 ].Sum;
			Vector2 bc = hull[ 2 ].Sum - hull[ 1 ].Sum;

			Vector2 abP = ab.Perpendicular;
			//pcab = Dot( abP, ( B - C ) )
			if( Vector2.Dot( abP, -bc ) < 0 )
				//Checking if the perpendicular is going with or against -bc
				//If it goes with -bc, this means the perpendular is facing the wrong way.
				abP = -abP;

			Vector2 acP = ac.Perpendicular;
			//pcac = Dot( acP, ( C - B ) )
			if( Vector2.Dot( acP, bc ) < 0 )
				//Checking if the perpendicular is going with or against bc
				//If it goes with bc, this means the perpendular is facing the wrong way.
				acP = -acP;

			//------------------x-------------------
			//--------------------------------------
			//--------------------------------------
			//-------------------B------------------
			//------------------/\------------------
			//-----------------/  \-----------------
			//----------------/    \----------------
			//---------------/      \---------------
			//--------------/        \-----x--------
			//-------2-----/          \-------------
			//------------/            \------------
			//-----------/      1       \-----------
			//----------/                \----------
			//---------/                  \---------
			//--------/                    \--------
			//-------/                      \-------
			//----A-/________________________\------
			//--------------------------------C-----
			//--4----------------3----------------x-
			//--------------------------------------
			// Checks if the origin is one one of the 6 voronoi regions
			// 1: The origin is inside the triangle, but this is a 3d check, which means the origin can be under or over the triangle.
			// 2,3: The origin is inside the voronoi region of the (AB)(AC) edge, which means (C)(B) must be removed and a search from (AB)(AC) to origin is needed
			// 4: The origin is inside the voronoi region of the A vertex, which means B and C must be removed and a search from A to origin is needed
			// x: This shouldn't be applicable, as the triangle is built up from searching towards the origin. BC, B or C thus can't be the new search direction as that would counter the latest search direction

			//O = the origin
			if( Vector2.Dot( acP, a0 ) > -EPSILON ) {
				//O is outside the edge AC. This means we either have case 4, 3 or 2. (voronoi region not checked yet.)
				if( Vector2.Dot( ac, a0 ) > -EPSILON ) {
					//O is in the AC voronoi region (case 3)
					hull.SearchDirection = acP;
					hull.Remove( 1 );
				} else {
					//O is not in the AC voronoi. This means we either have case 2 or 4. (voronoi region not checked yet.)
					if( Vector2.Dot( ab, a0 ) > -EPSILON ) {
						//O is in the AB voronoi region (case 2)
						hull.SearchDirection = abP;
						hull.Remove( 2 );
					} else {
						//O is in the vertex A's voronoi region (case 4)
						hull.SearchDirection = a0;
						hull.Remove( 2 );
						hull.Remove( 1 );
					}
				}
			} else {
				//O is NOT outside the edge AC. This means we either have case 1, 2 or 4.
				if( Vector2.Dot( abP, a0 ) > -EPSILON ) {
					//O is not in the AC voronoi. This means we either have case 2 or 4. (voronoi region not checked yet.)
					if( Vector2.Dot( ab, a0 ) > -EPSILON ) {
						//O is in the AB voronoi region (case 2)
						hull.SearchDirection = abP;
						hull.Remove( 2 );
					} else {
						//O is in the vertex A's voronoi region (case 4)
						hull.SearchDirection = a0;
						hull.Remove( 2 );
						hull.Remove( 1 );
					}
				}
			}
		}

		/*public static int PerformStep( Simplex2 simplex, PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, bool log ) {
			if( log )
				Console.WriteLine( "Performing GJK step..." );
			switch( simplex.Count ) {
				case 0: {
						simplex.AddInitialSupport( shapeA, shapeB );
						break;
					}
				case 1: {
						//Find minkowski sum oppposite the one point already present.
						Vector2 dir = -simplex[ 0 ].Sum;
						if( !simplex.AddSupport( shapeA, shapeB, dir ) ) {
							return -1;
							//Expand polytope to find closest point to minkowski origin!
						}
						break;
					}
				case 2: {
						//Find minkowski sum perpendicular to the two points already present.
						Vector2 b0 = -simplex[ 1 ].Sum;
						Vector2 ba = simplex[ 0 ].Sum + b0;
						Vector2 dir = ba.Perpendicular;
						if( Vector2.Dot( dir, b0 ) < 0 ) {
							dir = -dir;
							simplex.Switch( 0, 1 );//Simplex is now winding correctly.
						}
						if( !simplex.AddSupport( shapeA, shapeB, dir ) ) {
							return -1;
							//Expand polytope to find closest point to minkowski origin!
						}
						break;
					}
				case 3: {
						//Determine if the points all encapsulate the origin or not.
						Vector2 a0 = -simplex[ 0 ].Sum;
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
							return 1;
							//Expand polytope to find closest point to minkowski origin!
						}

						break;
					}
				default:
					Console.WriteLine( $"failure, Count[{simplex.Count}] is not recognized!" );
					break;
			}
			return 0;
		}*/
	}
}
