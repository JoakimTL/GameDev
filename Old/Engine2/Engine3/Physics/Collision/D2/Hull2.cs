using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision.D2 {
	public class Hull2 {

		public Vector2 SearchDirection { get; set; }

		public bool GJKDone { get; private set; }
		public bool Hit { get; private set; }

		public int Winding { get; private set; }

		public int Count { get => supportList.Count; }
		private List<Support2> supportList;

		public IReadOnlyList<Support2> Supports { get => supportList; }

		public Hull2() {
			supportList = new List<Support2>();
			Winding = 1;
		}

		public void AddInitialSupport( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB ) {
			Support2 s = new Support2( shapeA.GetInitial(), shapeB.GetInitial() );
			supportList.Add( s );
		}

		public Support2 GetSupport( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, Vector2 d, out bool valid ) {
			Support2 s = new Support2( shapeA.GetFurthest( d ), shapeB.GetFurthest( -d ) );
			valid = Vector2.Dot( s.Sum, d ) >= 0;
			return s;
		}

		public void InsertSupport( int index, Support2 s ) {
			supportList.Insert( index, s );
			if( Count > 2 ) {
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
				if( winding == 0 )
					winding++;
				Winding = winding;
			}
		}

		public void Remove( int index ) {
			supportList.RemoveAt( index );
		}

		internal void SetHit( bool hit ) {
			if( GJKDone )
				return;
			Hit = hit;
			GJKDone = true;
		}

		public Support2 this[ int i ] { get => supportList[ i ]; }
	}

	public struct Support2 {

		public Vector2 A { get; private set; }
		public Vector2 B { get; private set; }
		public Vector2 Sum { get; private set; }

		public Support2( Vector2 a, Vector2 b ) {
			A = a;
			B = b;
			Sum = a - b;
		}

		public override string ToString() {
			return A + " - " + B + " = " + Sum;
		}

		public override int GetHashCode() {
			return Sum.GetHashCode();
		}

		public override bool Equals( object obj ) {
			if( obj is Support2 c )
				return EqualsInternal( c );
			return false;
		}

		private bool EqualsInternal( Support2 c ) {
			return c.A == A && c.B == B;
		}

	}
}
