using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision.D3 {
	public class Hull3 {

		public Vector3 SearchDirection { get; set; }
		public bool Hit { get; private set; }

		public int Count { get => supportList.Count; }
		private List<Support3> supportList;
		public IReadOnlyList<Support3> Supports { get => supportList; }

		public Hull3() {
			supportList = new List<Support3>();
		}

		public void AddInitialSupport( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB ) {
			Support3 s = new Support3( shapeA.GetInitial(), shapeB.GetInitial() );
			supportList.Add( s );
		}


		public void AddSupport( Support3 s ) {
			supportList.Add( s );
		}

		public Support3 GetSupport( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB, Vector3 d, out bool valid ) {
			Support3 s = new Support3( shapeA.GetFurthest( d ), shapeB.GetFurthest( -d ) );
			valid = Vector3.Dot( s.Sum, d ) >= 0;
			return s;
		}

		public Support3 GetSupport( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB, int c ) {
			return new Support3( shapeA.GetUniquePoint( c ), shapeB.GetUniquePoint( c ) );
		}

		public void Switch( int i1, int i2 ) {
			Support3 s1 = supportList[ i1 ];
			supportList[ i1 ] = supportList[ i2 ];
			supportList[ i2 ] = s1;
		}

		public void InsertSupport( int index, Support3 s ) {
			supportList.Insert( index, s );
		}

		public void Remove( int i ) {
			supportList.RemoveAt( i );
		}

		public void SetHit() {
			Hit = true;
		}

		/// <summary>
		/// Fills the hull to the point of it being a tetrahedron.
		/// </summary>
		/// <param name="shapeA"></param>
		/// <param name="shapeB"></param>
		public void FillHull( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB, out Hull3Faces faces ) {
			while( Count < 4 ) {
				switch( Count ) {
					case 0: {
							AddInitialSupport( shapeA, shapeB );
							break;
						}
					case 1: {
							break;
						}
					case 2: {
							break;
						}
					case 3: {
							break;
						}
				}
			}

			faces = new Hull3Faces( this );
		}

		public Support3 this[ int i ] { get => supportList[ i ]; }

	}

	public struct Support3 {

		public Vector3 A { get; private set; }
		public Vector3 B { get; private set; }
		public Vector3 Sum { get; private set; }

		public Support3( Vector3 a, Vector3 b ) {
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
			if( obj is Support3 c )
				return EqualsInternal( c );
			return false;
		}

		private bool EqualsInternal( Support3 c ) {
			return c.A == A && c.B == B;
		}

	}
}
