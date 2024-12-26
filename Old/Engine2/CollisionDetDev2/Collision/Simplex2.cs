using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public class Hull2 {

		public Vector2 SearchDirection { get; set; }

		public bool GJKDone { get; private set; }
		public bool Hit { get; private set; }

		public int Count { get => supportList.Count; }
		private List<Support> supportList;

		public IReadOnlyList<Support> Supports { get => supportList; }

		public Hull2() {
			supportList = new List<Support>();
		}

		public void AddInitialSupport( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB ) {
			Support s = new Support( shapeA.GetInitial(), shapeB.GetInitial() );
			supportList.Add( s );
		}

		public bool AddSupport( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, Vector2 d ) {
			Support s = new Support( shapeA.GetFurthest( d ), shapeB.GetFurthest( -d ) );
			supportList.Add( s );
			return Vector2.Dot( s.Sum, d ) >= 0;
		}

		public Support GetSupport( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, Vector2 d, out bool valid ) {
			Support s = new Support( shapeA.GetFurthest( d ), shapeB.GetFurthest( -d ) );
			valid = Vector2.Dot( s.Sum, d ) >= 0;
			return s;
		}

		public void InsertSupport( int index, Support s ) {
			supportList.Insert( index, s );
		}

		public void Remove( int i ) {
			supportList.RemoveAt( i );
		}

		public void Switch( int i1, int i2 ) {
			Support s1 = supportList[ i1 ];
			supportList[ i1 ] = supportList[ i2 ];
			supportList[ i2 ] = s1;
		}

		internal void SetHit( bool hit ) {
			if( GJKDone )
				return;
			Hit = hit;
			GJKDone = true;
		}

		public int GetWinding() {
			float signedArea = 0;
			for( int i = 0; i < Count; i++ ) {
				//( x2 − x1)( y2 + y1 )
				Vector2 a = supportList[ i ].Sum;
				int j = i + 1;
				if( j == Count )
					j = 0;
				Vector2 b = supportList[ j ].Sum;
				signedArea += ( b.X - a.X ) * ( b.Y + a.Y );
			}
			int winding = Math.Sign( signedArea );
			if( winding == 0 ) {
				winding++;
			}
			return winding;
		}

		internal bool RemoveSurplus() {
			bool excessFound = false;
			for( int i = Count - 1; i >= 0; i-- ) {
				Vector2 a = supportList[ i ].Sum;
				for( int j = i - 1; j >= 0; j-- ) {
					if( a == supportList[ j ].Sum ) {
						Remove( i );
						excessFound = true;
						break;
					}
				}
			}
			return excessFound;
		}

		public Support this[ int i ] { get => supportList[ i ]; }

	}
}
