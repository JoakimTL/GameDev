using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision.D2 {
	public static class GJK2 {

		/// <summary>
		/// Because of floating point errors, a sufficiently small bias is used to counteract the errors. <br/>
		/// If two shapes are very close to colliding and this bias triggers a collision check the human eye couldn't spot the difference anyways<br/>
		/// In this engine, this bias is <b>~0.061mm (2^-14)</b>
		/// </summary>
		public const float EPSILON = 0.00006103515f;

		/// <summary>
		/// Completes one step in the GJK collision DETECTION. This method is usually called by <see cref="CollisionChecker" />.
		/// </summary>
		/// <param name="hull">The simplex the algorithm uses.</param>
		/// <returns>An <see cref="int"/> indicating the progress of the algorithm.<br/>
		/// Returns 1 if the shapes are colliding, -1 if they are not colliding and 0 if the algorithm hasn't concluded yet.
		/// </returns>
		public static int PerformStep( Hull2 hull, PhysicsShape<Transform2, Vector2> shape1, PhysicsShape<Transform2, Vector2> shape2 ) {
			if( hull.Count == 0 ) {
				hull.AddInitialSupport( shape1, shape2 );
				hull.SearchDirection = -hull[ 0 ].Sum;
			}

			//Happens if the initial support lands on the origin.
			if( hull.SearchDirection == 0 )
				return 1;

			Support2 newSupport = hull.GetSupport( shape1, shape2, hull.SearchDirection, out bool valid );

			if( !valid || !CheckDuplicates( hull, newSupport ) )
				return -1;

			hull.InsertSupport( 0, newSupport );

			ExamineSimplex( hull );

			//The simplex is a triangle after the gjk loop step, meaning the origin is encapsulated.
			if( hull.Count == 3 )
				return 1;
			return 0;
		}

		/// <returns>Returns true if the support isn't already present in the hull.</returns>
		private static bool CheckDuplicates( Hull2 hull, Support2 newSupport ) {
			for( int i = 0; i < hull.Count; i++ )
				if( hull[ i ].A == newSupport.A && hull[ i ].B == newSupport.B )
					return false;
			return true;
		}

		private static void ExamineSimplex( Hull2 hull ) {
			if( hull.Count == 3 ) {
				DoTriangleTest( hull );
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
							hull.SearchDirection = dir; // From the line towards the origin. (perpendicular to the line, facing the origin)
						} else {
							hull.SearchDirection = a0; // From point A towards the origin.
						}
						break;
					}
			}
		}


		private static void DoTriangleTest( Hull2 hull ) {
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

	}
}
