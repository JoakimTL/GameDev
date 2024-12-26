using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public class Simplex3Faces {

		public Vector3 SearchDirection { get; set; }
		public Vector3 SearchOrigin { get; set; }


		public int Count { get => faceList.Count; }
		private List<Face3> faceList;
		public IReadOnlyList<Face3> Faces { get => faceList; }

		public Simplex3Faces( Simplex3 simplex ) {
			faceList = new List<Face3>();

			AddFace( new Face3( 0, 1, 2 ) );
			AddFace( new Face3( 0, 2, 3 ) );
			AddFace( new Face3( 0, 3, 1 ) );
			AddFace( new Face3( 1, 3, 2 ) );
		}

		public Simplex3Faces( Simplex3Faces faces ) {
			faceList = new List<Face3>();
			for( int i = 0; i < faces.Count; i++ ) {
				faceList.Add( faces[ i ] );
			}
		}

		public void AddFace( Face3 f ) {
			faceList.Add( f );
		}


		public void RemoveFace( int i ) {
			faceList.RemoveAt( i );
		}

		public Face3 this[ int i ] { get => faceList[ i ]; }

	}
}
