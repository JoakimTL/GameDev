using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public static class GJK3 {

		public const float EPSILON = 0.00006103515f; //2^-14

		//https://gist.github.com/vurtun/29727217c269a2fbf4c0ed9a1d11cb40
		//https://www.youtube.com/watch?v=XIavUJ848Mk
		public static int PerformStep( Simplex3 simplex, PhysicsShape<Transform3, Vector3> shape1, PhysicsShape<Transform3, Vector3> shape2, bool log, int it, out Simplex3 preExamination ) {
			if( log )
				Console.WriteLine( "i: " + it + ", c: " + simplex.Count );
			if( simplex.Count == 0 ) {
				simplex.AddInitialSupport( shape1, shape2 );
				simplex.SearchDirection = -simplex[ 0 ].Sum;
			}

			preExamination = new Simplex3();
			if( log )
				Console.WriteLine( "sd: " + preExamination.SearchDirection );
			if( log )
				Console.WriteLine( "so: " + preExamination.SearchOrigin );

			if( simplex.SearchDirection != 0 ) {
				Support3 newSupport = simplex.GetSupport( shape1, shape2, simplex.SearchDirection, out bool valid );

				preExamination.AddSupport( newSupport );
				for( int i = 0; i < simplex.Count; i++ ) {
					preExamination.AddSupport( simplex[ i ] );
				}

				if( !valid || !CheckDuplicates( simplex, newSupport ) ) {
					if( log )
						Console.WriteLine( "nc: " + valid );
					return -1;
				}
				simplex.InsertSupport( 0, newSupport );
			} else {
				for( int i = 0; i < simplex.Count; i++ ) {
					preExamination.AddSupport( simplex[ i ] );
				}
				return 1;
			}

			preExamination = new Simplex3();
			for( int i = 0; i < simplex.Count; i++ ) {
				preExamination.AddSupport( simplex[ i ] );
			}

			ExamineSimplex( simplex, log );

			preExamination.SearchDirection = simplex.SearchDirection;
			preExamination.SearchOrigin = simplex.SearchOrigin;

			//The simplex is a tetrahedron after the gjk loop step, meaning the origin is encapsulated.
			if( simplex.Count == 4 )
				return 1;
			return 0;
		}

		private static void ExamineSimplex( Simplex3 simplex, bool log ) {
			if( simplex.Count == 4 ) {
				DoTetrahedronTest( simplex, log );
			}

			switch( simplex.Count ) {
				case 1: {
						simplex.SearchDirection = -simplex[ 0 ].Sum;
						simplex.SearchOrigin = simplex[ 0 ].Sum;
						break;
					}
				case 2: {
						Vector3 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
						Vector3 a0 = -simplex[ 0 ].Sum;
						if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
							simplex.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab ); // From the line towards the origin.
							simplex.SearchOrigin = ( simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 2;
						} else {
							simplex.SearchDirection = a0;
							simplex.SearchOrigin = simplex[ 0 ].Sum;
						}
						break;
					}
				case 3: {
						DoTriangleTest( simplex, log );
						break;
					}
			}
		}

		private static bool CheckDuplicates( Simplex3 simplex, Support3 newSupport ) {
			for( int i = 0; i < simplex.Count; i++ ) {
				if( simplex[ i ].A == newSupport.A && simplex[ i ].B == newSupport.B ) {
					return false;
				}
			}
			return true;
		}

		private static void DoTetrahedronTest( Simplex3 simplex, bool log ) {
			Vector3 a0 = -simplex[ 0 ].Sum;
			Vector3 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
			Vector3 ac = simplex[ 2 ].Sum - simplex[ 0 ].Sum;
			Vector3 ad = simplex[ 3 ].Sum - simplex[ 0 ].Sum;
			Vector3 abc = Vector3.Cross( ab, ac );
			Vector3 acd = Vector3.Cross( ac, ad );
			Vector3 adb = Vector3.Cross( ad, ab );

			if( log ) {
				Console.WriteLine( "abc: " + Vector3.Dot( abc, a0 ) );
				Console.WriteLine( "acd: " + Vector3.Dot( acd, a0 ) );
				Console.WriteLine( "adb: " + Vector3.Dot( adb, a0 ) );
			}
			if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
				if( log )
					Console.WriteLine( "abc" );
				Vector3 abc_ac = Vector3.Cross( abc, ac );
				if( log )
					Console.WriteLine( "abc_ac: " + Vector3.Dot( abc_ac, a0 ) );
				if( Vector3.Dot( abc_ac, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ac, a0 ) > -EPSILON ) {
						simplex.Remove( 1 );
						return;
					}
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						simplex.Remove( 2 );
						return;
					}
					simplex.Remove( 2 );
					simplex.Remove( 1 );
					return;
				}
				Vector3 ab_abc = Vector3.Cross( ab, abc );
				if( log )
					Console.WriteLine( "ab_abc: " + Vector3.Dot( ab_abc, a0 ) );
				if( Vector3.Dot( ab_abc, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						simplex.Remove( 2 );
						return;
					}
					simplex.Remove( 2 );
					simplex.Remove( 1 );
					return;
				}
				if( log )
					Console.WriteLine( "abc, a0: " + Vector3.Dot( abc, a0 ) );
				if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
					simplex.Remove( 3 );
				}
				return;
			}
			if( Vector3.Dot( acd, a0 ) > -EPSILON ) {
				if( log )
					Console.WriteLine( "acd" );
				Vector3 ac_acd = Vector3.Cross( ac, acd );
				if( log )
					Console.WriteLine( "ac_acd: " + Vector3.Dot( ac_acd, a0 ) );
				if( Vector3.Dot( ac_acd, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ac, a0 ) > -EPSILON ) {
						simplex.Remove( 3 );
						return;
					}
					if( Vector3.Dot( ad, a0 ) > -EPSILON ) {
						simplex.Remove( 2 );
						return;
					}
					simplex.Remove( 3 );
					simplex.Remove( 2 );
					return;
				}
				Vector3 acd_ad = Vector3.Cross( acd, ad );
				if( log )
					Console.WriteLine( "acd_ad: " + Vector3.Dot( acd_ad, a0 ) );
				if( Vector3.Dot( acd_ad, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ad, a0 ) > -EPSILON ) {
						simplex.Remove( 2 );
						return;
					}
					simplex.Remove( 3 );
					simplex.Remove( 2 );
					return;
				}
				if( log )
					Console.WriteLine( "acd, a0: " + Vector3.Dot( acd, a0 ) );
				if( Vector3.Dot( acd, a0 ) > -EPSILON ) {
					simplex.Remove( 1 );
				}
				return;
			}
			if( Vector3.Dot( adb, a0 ) > -EPSILON ) {
				if( log )
					Console.WriteLine( "adb" );
				Vector3 ad_adb = Vector3.Cross( ad, adb );
				if( log )
					Console.WriteLine( "ad_adb: " + Vector3.Dot( ad_adb, a0 ) );
				if( Vector3.Dot( ad_adb, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ad, a0 ) > -EPSILON ) {
						simplex.Remove( 1 );
						return;
					}
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						simplex.Remove( 3 );
						return;
					}
					simplex.Remove( 3 );
					simplex.Remove( 1 );
					return;
				}
				Vector3 adb_ab = Vector3.Cross( adb, ab );
				if( log )
					Console.WriteLine( "adb_ab: " + Vector3.Dot( adb_ab, a0 ) );
				if( Vector3.Dot( adb_ab, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						simplex.Remove( 3 );
						return;
					}
					simplex.Remove( 3 );
					simplex.Remove( 1 );
					return;
				}
				if( log )
					Console.WriteLine( "adb, a0: " + Vector3.Dot( adb, a0 ) );
				if( Vector3.Dot( adb, a0 ) > -EPSILON ) {
					simplex.Remove( 2 );
				}
				return;
				//DoTetrahedronTriangleTest( simplex, 3, 1, a0, ad, ab, adb );
				//return;
			}
			if( log )
				Console.WriteLine( "wat----------------------------------" );
			/*Vector3 b0 = -simplex[ 1 ].Sum;
			Vector3 bc = simplex[ 2 ].Sum - simplex[ 1 ].Sum;
			Vector3 bd = simplex[ 3 ].Sum - simplex[ 1 ].Sum;
			Vector3 bdc = Vector3.Cross( bd, bc );
			Vector3 bdc_bc = Vector3.Cross( bdc, bc );
			if( log )
				Console.WriteLine( "bdc_bc: " + Vector3.Dot( bdc_bc, b0 ) );
			if( Vector3.Dot( bdc_bc, b0 ) > -EPSILON ) {
				if( Vector3.Dot( bc, b0 ) > -EPSILON ) {
					simplex.Remove( 3 );
					simplex.SearchDirection = Vector3.Cross( Vector3.Cross( bc, b0 ), bc );
					return;
				}
				if( Vector3.Dot( bd, b0 ) > -EPSILON ) {
					simplex.Remove( 2 );
					simplex.SearchDirection = Vector3.Cross( Vector3.Cross( ab, b0 ), ab );
					return;
				}
				simplex.Remove( 3 );
				simplex.Remove( 2 );
				simplex.SearchDirection = b0;
				return;
			}
			Vector3 bd_bdc = Vector3.Cross( bd, bdc );
			if( log )
				Console.WriteLine( "bd_bdc: " + Vector3.Dot( bd_bdc, b0 ) );
			if( Vector3.Dot( bd_bdc, b0 ) > -EPSILON ) {
				if( Vector3.Dot( bd, b0 ) > -EPSILON ) {
					simplex.Remove( 2 );
					simplex.SearchDirection = Vector3.Cross( Vector3.Cross( bd, b0 ), bd );
					return;
				}
				simplex.Remove( 3 );
				simplex.Remove( 2 );
				simplex.SearchDirection = b0;
				return;
			}
			if( log )
				Console.WriteLine( "bdc, b0: " + Vector3.Dot( bdc, b0 ) );
			if( Vector3.Dot( bdc, b0 ) > -EPSILON ) {
				simplex.SearchDirection = bdc;
				simplex.Remove( 0 );
			}*/
		}

		private static void DoTriangleTest( Simplex3 simplex, bool log ) {
			if( log )
				Console.WriteLine( "tri" );
			Vector3 a0 = -simplex[ 0 ].Sum;
			Vector3 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
			Vector3 ac = simplex[ 2 ].Sum - simplex[ 0 ].Sum;
			Vector3 abc = Vector3.Cross( ab, ac );
			Vector3 abc_ac = Vector3.Cross( abc, ac );

			if( log )
				Console.WriteLine( "abc_ac: " + Vector3.Dot( abc_ac, a0 ) );
			if( Vector3.Dot( abc_ac, a0 ) > -EPSILON ) {
				if( Vector3.Dot( ac, a0 ) > -EPSILON ) {
					simplex.SearchDirection = Vector3.Cross( Vector3.Cross( ac, a0 ), ac );
					simplex.SearchOrigin = ( simplex[ 2 ].Sum + simplex[ 0 ].Sum ) / 2;
					simplex.Remove( 1 );
				} else {
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						simplex.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab );
						simplex.SearchOrigin = ( simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 2;
						simplex.Remove( 2 );
					} else {
						simplex.SearchDirection = a0;
						simplex.SearchOrigin = simplex[ 0 ].Sum;
						simplex.Remove( 2 );
						simplex.Remove( 1 );
					}
				}
			} else {
				Vector3 ab_abc = Vector3.Cross( ab, abc );
				if( log )
					Console.WriteLine( "ab_abc: " + Vector3.Dot( ab_abc, a0 ) );
				if( Vector3.Dot( ab_abc, a0 ) > -EPSILON ) {
					if( Vector3.Dot( ab, a0 ) > -EPSILON ) {
						simplex.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab );
						simplex.SearchOrigin = ( simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 2;
						simplex.Remove( 2 );
					} else {
						simplex.SearchDirection = a0;
						simplex.SearchOrigin = simplex[ 0 ].Sum;
						simplex.Remove( 2 );
						simplex.Remove( 1 );
					}
				} else {
					if( log )
						Console.WriteLine( "abc, a0: " + Vector3.Dot( abc, a0 ) );
					if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
						simplex.SearchDirection = abc;
						simplex.SearchOrigin = ( simplex[ 2 ].Sum + simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 3;
					} else {
						simplex.SearchDirection = -abc;
						simplex.SearchOrigin = ( simplex[ 2 ].Sum + simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 3;
						simplex.Switch( 1, 2 );
					}
				}
			}
		}
	}
}
