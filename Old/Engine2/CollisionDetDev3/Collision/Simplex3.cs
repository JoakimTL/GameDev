using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public class Simplex3 {
		
		public Vector3 SearchDirection { get; set; }
		public Vector3 SearchOrigin { get; set; }

		public bool Hit { get; private set; }

		public int Count { get => supportList.Count; }
		private List<Support3> supportList;
		public IReadOnlyList<Support3> Supports { get => supportList; }

		public Simplex3() {
			supportList = new List<Support3>();
		}

		public void AddInitialSupport( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB ) {
			Support3 s = new Support3( shapeA.GetInitial(), shapeB.GetInitial() );
			supportList.Add( s );
		}

		public bool AddSupport( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB, Vector3 d ) {
			Support3 s = new Support3( shapeA.GetFurthest( d ), shapeB.GetFurthest( -d ) );
			supportList.Add( s );
			return Vector3.Dot( s.Sum, d ) >= 0;
		}

		public void AddSupport( Support3 s ) {
			supportList.Add( s );
		}

		public Support3 GetSupport( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB, Vector3 d, out bool valid ) {
			Support3 s = new Support3( shapeA.GetFurthest( d ), shapeB.GetFurthest( -d ) );
			valid = Vector3.Dot( s.Sum, d ) >= 0;
			return s;
		}

		public void Switch(int i1, int i2 ) {
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

		public Support3 this[ int i ] { get => supportList[ i ]; }
	}
}
