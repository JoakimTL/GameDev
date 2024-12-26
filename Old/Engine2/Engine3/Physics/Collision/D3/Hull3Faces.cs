using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision.D3 {
	public class Hull3Faces {
		public Vector3 SearchDirection { get; set; }
		public Vector3 SearchOrigin { get; set; }

		public int Count { get => faceList.Count; }
		private List<Hull3Face> faceList;
		public IReadOnlyList<Hull3Face> Faces { get => faceList; }

		public Hull3Faces() {
			faceList = new List<Hull3Face>();

			AddFace( new Hull3Face( 0, 1, 2 ) );
			AddFace( new Hull3Face( 0, 2, 3 ) );
			AddFace( new Hull3Face( 0, 3, 1 ) );
			AddFace( new Hull3Face( 1, 3, 2 ) );
		}

		public Hull3Faces( Hull3 hull ) {
			faceList = new List<Hull3Face>();
			//Creates the faces such that they all point outward
			AddFace( new Hull3Face( 0, 1, 2 ) );
			AddFace( new Hull3Face( 0, 2, 3 ) );
			AddFace( new Hull3Face( 0, 3, 1 ) );
			AddFace( new Hull3Face( 1, 3, 2 ) );
		}

		public void AddFace( Hull3Face f ) {
			faceList.Add( f );
		}

		public void RemoveFace( int i ) {
			faceList.RemoveAt( i );
		}

		public Hull3Face this[ int i ] { get => faceList[ i ]; }
	}

	public struct Hull3Face {

		public int A { get; private set; }
		public int B { get; private set; }
		public int C { get; private set; }

		public Hull3Face( int a, int b, int c ) {
			A = a;
			B = b;
			C = c;
		}

	}
}
