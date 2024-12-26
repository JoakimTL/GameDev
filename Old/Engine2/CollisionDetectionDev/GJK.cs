using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionDev {
	class GJK {

		public static Vector2 TripleProduct( Vector2 a, Vector2 b, Vector2 c ) {
			float ac = a.X * c.X + a.Y * c.Y;
			float bc = b.X * c.X + b.Y * c.Y;
			return new Vector2( b.X * ac - a.X * bc, b.Y * ac - a.Y * bc );
		}

		//EPA:
		/*
		 * Polytope containing the LINES from the simplex
		 * Find closest line to origin
		 * Expand polytope
		 * Find closest line to origin
		 */

		//En måte å se om den overlapper
		//Fikse at den ikke klarer å velge en side å bruke hvis det eksisterer 2 sider like langt unna
		//Gjøre om fra linjer til components


		public static Result DoSingleStep( ref Simplex es, Simplex s, Shape a, Shape b, bool log ) {
			if( s.comps.Count == 0 ) {
				Vector2 dir = a.GetTransformed( 0 ) - b.GetTransformed( 0 );
				Simplex.Component comp = new Simplex.Component( a.GetFurthest( dir ), b.GetFurthest( -dir ) );
				s.comps.Add( comp );
				if( Vector2.Dot( comp.sum, dir ) < 0 ) {
					if( log )
						Console.WriteLine( "failed stage 0" );
					s.failed = true;
				}
				if( log )
					Console.WriteLine( "a" );
			} else if( s.comps.Count == 1 ) {
				Vector2 dir = -s.comps[ 0 ].sum;
				Simplex.Component comp = new Simplex.Component( a.GetFurthest( dir ), b.GetFurthest( -dir ) );
				s.comps.Add( comp );
				if( Vector2.Dot( comp.sum, dir ) < 0 ) {
					if( log )
						Console.WriteLine( "failed stage 1" );
					s.failed = true;
				}
				if( log )
					Console.WriteLine( "b" );
			} else if( s.comps.Count == 2 ) {
				Vector2 dir = ( s.comps[ 1 ].sum - s.comps[ 0 ].sum ).Perpendicular;
				if( Vector2.Dot( dir, s.comps[ 0 ].sum ) > 0 )
					dir = -dir;
				Simplex.Component comp = new Simplex.Component( a.GetFurthest( dir ), b.GetFurthest( -dir ) );
				s.comps.Add( comp );
				if( Vector2.Dot( comp.sum, dir ) < 0 ) {
					if( log )
						Console.WriteLine( "failed stage 2" );
					s.failed = true;
				}
				if( log )
					Console.WriteLine( "c" );
			} else {
				if( log )
					Console.WriteLine( "d" );
				if( s.failed ) {
					if( log )
						Console.WriteLine( "failed." );
					return FindClosestPoints( s, false );
				} else {
					Vector2 a0 = -s.comps[ 0 ].sum;
					Vector2 b0 = -s.comps[ 1 ].sum;
					Vector2 ab = s.comps[ 1 ].sum - s.comps[ 0 ].sum;
					Vector2 ac = s.comps[ 2 ].sum - s.comps[ 0 ].sum;
					Vector2 bc = s.comps[ 2 ].sum - s.comps[ 1 ].sum;
					Vector2 abP = TripleProduct( ac, ab, ab );
					Vector2 acP = TripleProduct( ab, ac, ac );
					Vector2 bcP = TripleProduct( -ab, bc, bc );
					bool baab = Vector2.Dot( a0, abP ) > 0;
					bool bbbc = Vector2.Dot( a0, acP ) > 0;
					bool abbc = Vector2.Dot( b0, bcP ) > 0;
					if( baab ) {
						if( log )
							Console.WriteLine( "remove c." );
						s.comps.RemoveAt( 2 );
					} else if( bbbc ) {
						if( log )
							Console.WriteLine( "remove b." );
						s.comps.RemoveAt( 1 );
					} else if( abbc ) {
						if( log )
							Console.WriteLine( "remove a." );
						s.comps.RemoveAt( 0 );
					} else {
						if( log )
							Console.WriteLine( "inside." );
						if( es is null ) {
							es = new Simplex();
							for( int i = 0; i < s.comps.Count; i++ )
								es.comps.Add( s.comps[ i ] );
						}
						return ExpandPolytope( es, s, a, b, log );
					}
				}
			}
			return null;
		}

		private static Result ExpandPolytope( Simplex es, Simplex s, Shape a, Shape b, bool log ) {
			if( log )
				Console.WriteLine( "stop!" );
			Edge e = GetClosest( es, out int index );
			if( index == -1 )
				return null;

			Vector2 dir = e.normal;
			Simplex.Component comp = new Simplex.Component( a.GetFurthest( dir ), b.GetFurthest( -dir ) );

			if( log )
				Console.WriteLine( comp.sum );
			double d = Vector2.Dot( comp.sum, e.normal );
			if( log )
				Console.WriteLine( d - e.distance );
			if( d - e.distance < 0.1f ) {
				List<Simplex.Component> returns = new List<Simplex.Component>();
				returns.Add( FindClosestOnLine( es.comps[ index ].a, es.comps[ ( index + 1 ) % es.comps.Count ].a, es.comps[ index ].b ) );
				returns.Add( FindClosestOnLine( es.comps[ index ].b, es.comps[ ( index + 1 ) % es.comps.Count ].b, es.comps[ index ].a ) );
				returns.Add( FindClosestOnLine( es.comps[ index ].a, es.comps[ ( index + 1 ) % es.comps.Count ].a, es.comps[ ( index + 1 ) % es.comps.Count ].b ) );
				returns.Add( FindClosestOnLine( es.comps[ index ].b, es.comps[ ( index + 1 ) % es.comps.Count ].b, es.comps[ ( index + 1 ) % es.comps.Count ].a ) );
				Simplex.Component clo = returns[ 0 ];
				float lowest = returns[ 0 ].sum.Length;
				for( int i = 1; i < returns.Count; i++ ) {
					if( returns[ i ].sum.Length < lowest ) {
						clo = returns[ i ];
						lowest = returns[ i ].sum.Length;
					}
				}

				return new Result( clo.a, clo.b, true );
			} else {
				if( es.comps.Contains( comp ) )
					Console.WriteLine( "double!" );
				es.comps.Insert( ( index + 1 ) % es.comps.Count, comp );
			}

			return null;
		}

		private struct Edge {
			public float distance;
			public Vector2 normal;
		}

		private static Edge GetClosest( Simplex p, out int index ) {
			Edge e = new Edge {
				distance = float.MaxValue
			};
			index = -1;
			if( p.comps.Count == 0 )
				return e;
			index = 0;
			for( int i = 0; i < p.comps.Count; i++ ) {
				Simplex.Component a = p.comps[ i ];
				Simplex.Component b = p.comps[ ( i + 1 ) % p.comps.Count ];

				Vector2 ab = b.sum - a.sum;
				Vector2 n = TripleProduct( ab, a.sum, ab ).Normalized;
				float d = Vector2.Dot( n, a.sum );
				if( d < e.distance ) {
					e.distance = d;
					e.normal = n;
					index = i;
				}
			}
			return e;
		}

		private static Result FindClosestPoints( Simplex s, bool success ) {
			float lowest = float.MaxValue;
			Simplex.Component ret = new Simplex.Component();

			/*for( int j = 0; j < s.comps.Count; j++ ) {
				int jp = ( j + 1 ) % s.comps.Count;
				Vector2 aA = s.comps[ j ].a;
				Vector2 aB = s.comps[ jp ].a;
				Vector2 bA = s.comps[ j ].b;
				Vector2 bB = s.comps[ jp ].b;
				for( int i = 0; i < s.comps.Count; i++ ) {
					Vector2 pA = s.comps[ i ].a;
					Vector2 pB = s.comps[ i ].b;
					{
						Simplex.Component nRet = FindClosestOnLine( aA, aB, pB );
						float nlowest = ( nRet.a - nRet.b ).LengthSquared;
						if( nlowest < lowest ) {
							ret = nRet;
							lowest = nlowest;
						}
					}
					{
						Simplex.Component nRet = FindClosestOnLine( bA, bB, pA );
						float nlowest = ( nRet.a - nRet.b ).LengthSquared;
						if( nlowest < lowest ) {
							ret = nRet;
							lowest = nlowest;
						}
					}
				}
			}*/
			for( int j = 0; j < s.comps.Count; j++ ) {
				int jp = ( j + 1 ) % s.comps.Count;
				Vector2 aA = s.comps[ j ].a;
				Vector2 aB = s.comps[ jp ].a;
				Vector2 bA = s.comps[ j ].b;
				Vector2 bB = s.comps[ jp ].b;
				for( int i = 0; i < s.comps.Count; i++ ) {
					Vector2 pA = s.comps[ i ].a;
					Vector2 pB = s.comps[ i ].b;
					if( i != j ) {
						Simplex.Component nRet = FindClosestOnLine( aA, aB, pB );
						float nlowest = ( nRet.a - nRet.b ).LengthSquared;
						if( nlowest < lowest ) {
							ret = nRet;
							lowest = nlowest;
						}
					}
					if( i != jp ) {
						Simplex.Component nRet = FindClosestOnLine( bA, bB, pA );
						float nlowest = ( nRet.a - nRet.b ).LengthSquared;
						if( nlowest < lowest ) {
							ret = new Simplex.Component( nRet.b, nRet.a );
							lowest = nlowest;
						}
					}
				}
			}
			return new Result( ret.a, ret.b, success );
		}

		/* Finding intersection between two lines
		float det = ( a.X - b.X ) * ( c.Y - d.Y ) - ( a.Y - b.Y ) * ( c.X - d.X );
		float detInv = 1f / det;
		float s = ( ( a.X - c.X ) * ( c.Y - d.Y ) - ( a.Y - c.Y ) * ( c.X - d.X ) ) * detInv;
		float u = -( ( a.X - b.X ) * ( a.Y - c.Y ) - ( a.Y - b.Y ) * ( a.X - c.X ) ) * detInv;*/

		private static Simplex.Component FindClosestOnLine( Vector2 a, Vector2 b, Vector2 p ) {
			Vector2 ab = b - a;
			float abL = ab.Length;
			Vector2 abN = ab / abL;

			float dot = Vector2.Dot( abN, ( p - a ) );
			Vector2 r = a;
			if( dot >= 0 )
				if( dot > abL ) {
					r = b;
				} else {
					r = a + abN * dot;
				}
			return new Simplex.Component( r, p );
		}
	}
}
