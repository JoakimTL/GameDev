//using Engine.LinearAlgebra;
//using Engine.Utilities.Time;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices.ComTypes;
//using System.Text;
//using System.Threading;

//namespace Engine.Physics.Old.Collision.D2 {
//	internal static class GJK2 {

//		#region GJK
//		internal static CollisionData<Vector2> CheckCollision( BaseShape<Vector2> a, BaseShape<Vector2> b, bool epa = true, bool dist = true, int iterations = 50 ) {
//			Simplex s = new Simplex();
//			int iteration = 0;
//			bool valid = true;

//			while( iteration++ < iterations ) {
//				if( s.vertices.Count == 0 ) {
//					Vector2 dir = a.GetInitial() - b.GetInitial();
//					valid &= GetSupport( a, b, dir, out Support sup );
//					s.vertices.Add( sup );
//				} else if( s.vertices.Count == 1 ) {
//					Vector2 dir = -s.vertices[ 0 ].sum;
//					valid &= GetSupport( a, b, dir, out Support sup );
//					s.vertices.Add( sup );
//				} else if( s.vertices.Count == 2 ) {
//					Vector2 b0 = -s.vertices[ 1 ].sum;
//					Vector2 ba = s.vertices[ 0 ].sum + b0;
//					Vector2 dir = Vector2.TripleProduct( ba, b0, ba );
//					valid &= GetSupport( a, b, dir, out Support sup );
//					s.vertices.Add( sup );
//				} else if( s.vertices.Count == 3 ) {
//					if( !valid ) {
//						CollisionData<Vector2> data = new CollisionData<Vector2>( false );
//						if( dist )
//							data.AddPair( FindClosestPoints( s ) );
//						return data;
//					} else {
//						Vector2 a0 = -s.vertices[ 0 ].sum;
//						Vector2 b0 = -s.vertices[ 1 ].sum;
//						Vector2 ab = s.vertices[ 1 ].sum - s.vertices[ 0 ].sum;
//						Vector2 ac = s.vertices[ 2 ].sum - s.vertices[ 0 ].sum;
//						Vector2 bc = s.vertices[ 2 ].sum - s.vertices[ 1 ].sum;
//						Vector2 abP = Vector2.TripleProduct( ac, ab, ab );
//						Vector2 acP = Vector2.TripleProduct( ab, ac, ac );
//						Vector2 bcP = Vector2.TripleProduct( -ab, bc, bc );
//						if( Vector2.Dot( a0, abP ) > 0 ) {
//							s.vertices.RemoveAt( 2 );
//						} else if( Vector2.Dot( a0, acP ) > 0 ) {
//							s.vertices.RemoveAt( 1 );
//						} else if( Vector2.Dot( b0, bcP ) > 0 ) {
//							s.vertices.RemoveAt( 0 );
//						} else {
//							CollisionData<Vector2> data = new CollisionData<Vector2>( true );
//							if( epa ) {
//								Simplex polytope = new Simplex();
//								for( int i = 0; i < s.vertices.Count; i++ )
//									polytope.vertices.Add( s.vertices[ i ] );
//								data.AddPairs( ExpandPolytope( polytope, a, b ) );
//							}
//							return data;
//						}
//					}
//				}
//			}

//			return new CollisionData<Vector2>( false );
//		}

//		private static bool GetSupport( BaseShape<Vector2> a, BaseShape<Vector2> b, Vector2 dir, out Support s ) {
//			s = new Support( a.GetFurthest( dir ), b.GetFurthest( -dir ) );
//			return Vector2.Dot( s.sum, dir ) >= 0;
//		}
//		#endregion

//		#region EPA
//		private static List<CollisionData<Vector2>.CollisionDataNode> ExpandPolytope( Simplex polytope, BaseShape<Vector2> a, BaseShape<Vector2> b, int iterations = 20 ) {
//			List<CollisionData<Vector2>.CollisionDataNode> pointPairs = new List<CollisionData<Vector2>.CollisionDataNode>();
//			int iteration = 0;
//			while( iteration++ < iterations ) {
//				Edge e = GetClosest( polytope, out int index );
//				if( index == -1 )
//					return null;

//				Vector2 dir = e.normal;
//				Support sup = new Support( a.GetFurthest( dir ), b.GetFurthest( -dir ) );

//				double d = Vector2.Dot( sup.sum, e.normal );
//				if( d - e.distance < 0.1f ) {
//					List<Support> samples = new List<Support>();
//					List<Support> returns = new List<Support>();
//					int iplus = ( index + 1 ) % polytope.vertices.Count;
//					samples.Add( FindClosestOnLine( polytope.vertices[ index ].a, polytope.vertices[ iplus ].a, polytope.vertices[ index ].b ) );
//					samples.Add( FindClosestOnLine( polytope.vertices[ index ].b, polytope.vertices[ iplus ].b, polytope.vertices[ index ].a ) );
//					samples.Add( FindClosestOnLine( polytope.vertices[ index ].a, polytope.vertices[ iplus ].a, polytope.vertices[ iplus ].b ) );
//					samples.Add( FindClosestOnLine( polytope.vertices[ index ].b, polytope.vertices[ iplus ].b, polytope.vertices[ iplus ].a ) );
//					returns.Add( samples[ 0 ] );
//					float lowest = samples[ 0 ].sum.Length;
//					for( int i = 1; i < samples.Count; i++ ) {
//						float l = samples[ i ].sum.Length;
//						if( l < lowest ) {
//							returns.Clear();
//							returns.Add( samples[ i ] );
//							lowest = l;
//						} else if( l == lowest ) {
//							returns.Add( samples[ i ] );
//						}
//					}

//					for( int i = 1; i < returns.Count; i++ )
//						pointPairs.Add( new CollisionData<Vector2>.CollisionDataNode( e.distance, e.normal, returns[ i ].a, returns[ i ].b ) );
//					break;
//				} else {
//					polytope.vertices.Insert( ( index + 1 ) % polytope.vertices.Count, sup );
//				}
//			}
//			return pointPairs;
//		}

//		private struct Edge {
//			public float distance;
//			public Vector2 normal;
//		}

//		private static Edge GetClosest( Simplex p, out int index ) {
//			Edge e = new Edge {
//				distance = float.MaxValue
//			};
//			index = -1;
//			if( p.vertices.Count == 0 )
//				return e;
//			index = 0;
//			for( int i = 0; i < p.vertices.Count; i++ ) {
//				Support a = p.vertices[ i ];
//				Support b = p.vertices[ ( i + 1 ) % p.vertices.Count ];

//				Vector2 ab = b.sum - a.sum;
//				Vector2 n = Vector2.TripleProduct( ab, a.sum, ab ).Normalized;
//				float d = Vector2.Dot( n, a.sum );
//				if( d < e.distance ) {
//					e.distance = d;
//					e.normal = n;
//					index = i;
//				}
//			}
//			return e;
//		}
//		#endregion

//		private static CollisionData<Vector2>.CollisionDataNode FindClosestPoints( Simplex s ) {
//			float lowest = float.MaxValue;
//			Support ret = new Support();

//			for( int j = 0; j < s.vertices.Count; j++ ) {
//				int jp = ( j + 1 ) % s.vertices.Count;
//				Vector2 aA = s.vertices[ j ].a;
//				Vector2 aB = s.vertices[ jp ].a;
//				Vector2 bA = s.vertices[ j ].b;
//				Vector2 bB = s.vertices[ jp ].b;
//				for( int i = 0; i < s.vertices.Count; i++ ) {
//					Vector2 pA = s.vertices[ i ].a;
//					Vector2 pB = s.vertices[ i ].b;
//					{
//						Support nRet = FindClosestOnLine( aA, aB, pB );
//						float nlowest = ( nRet.a - nRet.b ).LengthSquared;
//						if( nlowest < lowest ) {
//							ret = nRet;
//							lowest = nlowest;
//						}
//					}
//					{
//						Support nRet = FindClosestOnLine( bA, bB, pA );
//						float nlowest = ( nRet.a - nRet.b ).LengthSquared;
//						if( nlowest < lowest ) {
//							ret = new Support( nRet.b, nRet.a );
//							lowest = nlowest;
//						}
//					}
//				}
//			}
//			return new CollisionData<Vector2>.CollisionDataNode( ( ret.a - ret.b ).Length, ( ret.b - ret.a ).Normalized, ret.a, ret.b );
//		}

//		private static Support FindClosestOnLine( Vector2 a, Vector2 b, Vector2 p ) {
//			Vector2 ab = b - a;
//			Vector2 ap = p - a;
//			float abLS = ab.LengthSquared;
//			float product = Vector2.Dot( ap, ab );
//			float distance = product / abLS;
//			if( distance > 0 ) {
//				if( distance < 1 ) {
//					return new Support( a + ab * distance, p );
//				} else {
//					return new Support( b, p );
//				}
//			} else {
//				return new Support( a, p );
//			}

//		}
//	}
//}
