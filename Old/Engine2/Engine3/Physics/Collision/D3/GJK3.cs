using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;

namespace Engine.Physics.Collision.D3 {
	public static class GJK3 {

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
		public static int PerformStep( Hull3 hull, PhysicsShape<Transform3, Vector3> shape1, PhysicsShape<Transform3, Vector3> shape2 ) {
			if( hull.Count == 0 ) {
				//Setting up the hull.
				hull.AddInitialSupport( shape1, shape2 );
				hull.SearchDirection = -hull[ 0 ].Sum;
			}

			if( hull.SearchDirection.LengthSquared > 0 ) {
				//Finds a new support in the current direction
				Support3 newSupport = hull.GetSupport( shape1, shape2, hull.SearchDirection, out bool valid );

				if( !valid || !CheckDuplicates( hull, newSupport ) )
					return -1;

				//The new support is not a duplicate, and did pass the origin
				hull.InsertSupport( 0, newSupport );
			} else {
				//The search direction is close to or a zero vector, but the hull has been filled without failing, which means one of the edges or faces of the hull is intersecting the origin
				//We still need to fill out the hull however.
				int c = 1;
				//Takes a random support to add into the hull
				Support3 newSupport = hull.GetSupport( shape1, shape2, c++ );
				//Checks if it is a duplicate
				while( !CheckDuplicates( hull, newSupport ) && c < 10 )
					newSupport = hull.GetSupport( shape1, shape2, c++ );
				//If the count reached 10, that means something is very wrong. This should not happen, as there should only be 4 supports.
				if( c == 10 ) {
					Logging.Warning( "GJK failed." );
					return -1;
				}
				//Inserts the support into the hull.
				hull.InsertSupport( 0, newSupport );
			}

			ExamineSimplex( hull );

			//The simplex is a tetrahedron after the gjk loop step, meaning the origin is encapsulated.
			if( hull.Count == 4 )
				return 1;
			return 0;
		}

		/// <returns>Returns true if the support isn't already present in the hull.</returns>
		private static bool CheckDuplicates( Hull3 hull, Support3 newSupport ) {
			for( int i = 0; i < hull.Count; i++ )
				if( hull[ i ].A == newSupport.A && hull[ i ].B == newSupport.B )
					return false;
			return true;
		}

		private static void ExamineSimplex( Hull3 hull ) {
			if( hull.Count == 4 ) {
				//The hull is a tetrahedron, the maximum size for the hull during the GJK algortihm. We should check this tetrahedron and remove vertices if they are incorrect.
				DoTetrahedronTest( hull );
			}

			switch( hull.Count ) {
				case 1: {
						//Search should always point to the origin
						hull.SearchDirection = -hull[ 0 ].Sum;
						break;
					}
				case 2: {
						//Search should always point to the origin
						Vector3 ab = hull[ 1 ].Sum - hull[ 0 ].Sum;
						Vector3 a0 = -hull[ 0 ].Sum;
						//Checks whether the origin is in the line's voronoi region, at the vertex' voronoi region and decides the search direction from there.
						if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
							hull.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab ); // From the line towards the origin.
						} else {
							hull.SearchDirection = a0;
						}
						break;
					}
				case 3: {
						Vector3 a0 = -hull[ 0 ].Sum;
						Vector3 ab = hull[ 1 ].Sum - hull[ 0 ].Sum;
						Vector3 ac = hull[ 2 ].Sum - hull[ 0 ].Sum;
						Vector3 abc = Vector3.Cross( ab, ac );
						Vector3 abc_ac = Vector3.Cross( abc, ac );

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
						if( Vector3.Dot( abc_ac, a0 ) > -EPSILON ) {
							//O is outside the edge AC. This means we either have case 4, 3 or 2. (voronoi region not checked yet.)
							if( Vector3.Dot( ac, a0 ) > -EPSILON ) {
								//O is in the AC voronoi region (case 3)
								hull.SearchDirection = Vector3.Cross( Vector3.Cross( ac, a0 ), ac );
								hull.Remove( 1 );
							} else {
								//O is not in the AC voronoi. This means we either have case 2 or 4. (voronoi region not checked yet.)
								if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
									//O is in the AB voronoi region (case 2)
									hull.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab );
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
							Vector3 ab_abc = Vector3.Cross( ab, abc );
							if( Vector3.Dot( ab_abc, a0 ) > -EPSILON ) {
								//O is outside the edge AB. This means we either have case 2 or 4. (voronoi region not checked yet.)
								if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
									//O is in the AB voronoi region (case 2)
									hull.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab );
									hull.Remove( 2 );
								} else {
									//O is in the vertex A's voronoi region (case 4)
									hull.SearchDirection = a0;
									hull.Remove( 2 );
									hull.Remove( 1 );
								}
							} else {
								//O is inside the triangle (case 1), must determine which side of the triangle face the origin is on.
								if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
									//O is on the clockwise side of the triangle face, meaning we search from that face direction.
									hull.SearchDirection = abc;
								} else {
									//O is on the counter-clockwise side of the triangle face, meaning we search from the opposite direction of the face.
									hull.SearchDirection = -abc;
									//B and C must be switched to maintain the right-hand rule of the cross product as we are switching the winding the of the triangle by searching in the opposite direction of the facing of the triangle face.
									hull.Switch( 1, 2 );
								}
							}
						}
						break;
					}
			}
		}

		private static void DoTetrahedronTest( Hull3 hull ) {
			Vector3 a0 = -hull[ 0 ].Sum;
			Vector3 ab = hull[ 1 ].Sum - hull[ 0 ].Sum;
			Vector3 ac = hull[ 2 ].Sum - hull[ 0 ].Sum;
			Vector3 ad = hull[ 3 ].Sum - hull[ 0 ].Sum;
			Vector3 abc = Vector3.Cross( ab, ac );
			Vector3 acd = Vector3.Cross( ac, ad );
			Vector3 adb = Vector3.Cross( ad, ab );

			// Checks whether the origin (O) is outside one of the triangle faces of the tetrahedron
			// No search direction is set in this method, as the triangle check and line check comes right after and sets the proper search direction.

			if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
				// O is outside the tetrahedron, outside the triangle face ABC
				// Now, much like the triangle test from a couple of lines over, the triangle face's voronoi regions are being checked.
				// Unlike the triangle test, the origins position compared to the triangle face is not done here, as this is a 3d shape with the origin already, hopefully, encapsualted.
				//------------------x-------------------
				//--------------------------------------
				//--------------------------------------
				//-------------------B------------------
				//------------------/\------------------
				//-----------------/  \-----------------
				//----------------/    \----------------
				//---------------/      \---------------
				//--------------/        \-----x--------
				//-------1-----/          \-------------
				//------------/            \------------
				//-----------/              \-----------
				//----------/        4       \----------
				//---------/                  \---------
				//--------/                    \--------
				//-------/                      \-------
				//------/________________________\------
				//----A----------------------------C----
				//--3----------------2----------------x-
				//--------------------------------------
				// 1: The origin is inside the voronoi region of the AB edge, which means C must be removed and a search from AB to origin is needed
				// 2: The origin is inside the voronoi region of the AC edge, which means B must be removed and a search from AC to origin is needed
				// 3: The origin is inside the voronoi region of the A vertex, which means B and C must be removed and a search from A to origin is needed
				// 4: The origin is inside the the triangle, but outside the tetrahedron, meaning point D, which is behind the terahedron is wrong and must be removed.
				// x: Not checked, as it would be counter intuitive to use computing power on something that is ruled out by the algorithm
				// The other 2 triangle face checks are not commented, because the process is the same with different vertices.
				// Remember to uphold the right hand rule for the cross product!

				Vector3 abc_ac = Vector3.Cross( abc, ac );
				if( Vector3.Dot( abc_ac, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ac, a0 ) > -EPSILON ) {
						//Case 2
						hull.Remove( 1 );
						return;
					}
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						//Case 1
						hull.Remove( 2 );
						return;
					}
					//Case 3
					hull.Remove( 2 );
					hull.Remove( 1 );
					return;
				}
				Vector3 ab_abc = Vector3.Cross( ab, abc );
				if( Vector3.Dot( ab_abc, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						//Case 1
						hull.Remove( 2 );
						return;
					}
					//Case 3
					hull.Remove( 2 );
					hull.Remove( 1 );
					return;
				}
				if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
					//Case 4
					hull.Remove( 3 );
				}
				return;
			}
			if( Vector3.Dot( acd, a0 ) > -EPSILON ) {
				Vector3 ac_acd = Vector3.Cross( ac, acd );
				if( Vector3.Dot( ac_acd, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ac, a0 ) > -EPSILON ) {
						hull.Remove( 3 );
						return;
					}
					if( Vector3.Dot( ad, a0 ) > -EPSILON ) {
						hull.Remove( 2 );
						return;
					}
					hull.Remove( 3 );
					hull.Remove( 2 );
					return;
				}
				Vector3 acd_ad = Vector3.Cross( acd, ad );
				if( Vector3.Dot( acd_ad, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ad, a0 ) > -EPSILON ) {
						hull.Remove( 2 );
						return;
					}
					hull.Remove( 3 );
					hull.Remove( 2 );
					return;
				}
				if( Vector3.Dot( acd, a0 ) > -EPSILON ) {
					hull.Remove( 1 );
				}
				return;
			}
			if( Vector3.Dot( adb, a0 ) > -EPSILON ) {
				Vector3 ad_adb = Vector3.Cross( ad, adb );
				if( Vector3.Dot( ad_adb, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ad, a0 ) > -EPSILON ) {
						hull.Remove( 1 );
						return;
					}
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						hull.Remove( 3 );
						return;
					}
					hull.Remove( 3 );
					hull.Remove( 1 );
					return;
				}
				Vector3 adb_ab = Vector3.Cross( adb, ab );
				if( Vector3.Dot( adb_ab, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						hull.Remove( 3 );
						return;
					}
					hull.Remove( 3 );
					hull.Remove( 1 );
					return;
				}
				if( Vector3.Dot( adb, a0 ) > -EPSILON ) {
					hull.Remove( 2 );
				}
				return;
			}
		}

	}
}
