//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Engine.Physics.Old.Attempt1 {
//	public class Shape {

//		public Vector3 center;
//		public List<Vector3> vectors;

//		public Shape( params Vector3[] vectors ) {
//			this.vectors = vectors.ToList();
//			simplex = new List<Simpl>();
//			CalculateCenter();
//		}

//		private void CalculateCenter() {
//			center = 0;

//			for( int i = 0; i < vectors.Count; i++ ) {
//				center += vectors[ i ];
//			}
//			center /= vectors.Count;

//			Console.WriteLine( "center: " + center );
//		}

//		private Vector3 Furthest( Vector3 d ) {
//			if( vectors.Count == 0 )
//				return 0;
//			Vector3 f = vectors[ 0 ];
//			float f0 = Vector3.Dot( vectors[ 0 ], d );
//			for( int i = 1; i < vectors.Count; i++ ) {
//				float f1 = Vector3.Dot( vectors[ i ], d );
//				if( f1 > f0 ) {
//					f = vectors[ 0 ];
//					f0 = f1;
//				}
//			}
//			Console.WriteLine( "furth: " + d + " = " + f );
//			return f;
//		}

//		private List<Simpl> simplex;

//		public float CheckCollision( Shape b ) {
//			simplex.Clear();
//			while( simplex.Count < 4 ) {
//				AddToSimplex( b );
//			}



//			return 0;
//		}

//		public void AddToSimplex( Shape b ) {
//			if( simplex.Count == 0 ) {
//				simplex.Insert( 0, MinkowskiSum( this, b, center - b.center ) );
//			} else if( simplex.Count == 1 ) {
//				simplex.Insert( 0, MinkowskiSum( this, b, -simplex[ 0 ].simplexSum ) );
//			} else if( simplex.Count == 2 ) {
//				Vector3 h = simplex[ 1 ].simplexSum - simplex[ 0 ].simplexSum;
//				Vector3 hn = h.Normalized;
//				float d0 = Vector3.Dot( hn, -simplex[ 0 ].simplexSum );
//				Vector3 d = simplex[ 0 ].simplexSum + hn * d0;
//				//finds the point on the line closest to the origin, then uses that to find the next point.
//				simplex.Insert( 0, MinkowskiSum( this, b, -d ) );
//			} else {
//				//find normal of the triangle produced.
//				Vector3 d = Vector3.Cross( simplex[ 1 ].simplexSum - simplex[ 0 ].simplexSum , simplex[ 2 ].simplexSum - simplex[ 0 ].simplexSum );
//				simplex.Insert( 0, MinkowskiSum( this, b, -d ) );
//			}

//			for( int i = 0; i < simplex.Count; i++ ) {
//				Console.WriteLine( simplex[ i ] );
//			}
//		}

//		private static Simpl MinkowskiSum( Shape a, Shape b, Vector3 d ) {
//			Vector3 fa = a.Furthest( d );
//			Vector3 fb = b.Furthest( -d );
//			return new Simpl() { simplexSum = fa - fb, comp1 = fa, comp2 = fb };
//		}

//		public struct Simpl {
//			public Vector3 simplexSum;
//			public Vector3 comp1;
//			public Vector3 comp2;

//			public override string ToString() {
//				return $"[{simplexSum}][{comp1},{comp2}]";
//			}
//		}
//	}
//}
