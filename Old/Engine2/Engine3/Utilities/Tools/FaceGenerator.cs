using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Tools {
	public class FaceGenerator {

		public static IReadOnlyList<(int, int, int)> GetFaces( List<Vector3> vertices ) {
			if( vertices.Count < 3 ) {
				Logging.Warning( "Tried to find faces for a vertex list with 2 or less elements. 3 elements are minimum to form a face." );
				return null;
			}
			if( vertices.Count == 3 ) {
				return new List<(int, int, int)>() { (0, 1, 2) };
			}
			Vector3 minAABB = vertices[ 0 ];
			Vector3 maxAABB = vertices[ 0 ];
			for( int i = 0; i < vertices.Count; i++ ) {
				minAABB = Vector3.Min( vertices[ i ], minAABB );
				maxAABB = Vector3.Max( vertices[ i ], maxAABB );
			}

			Vector3 center = ( maxAABB + minAABB ) * .5f;
			Console.WriteLine( center );

			List<(int, int, int)> faces = new List<(int, int, int)>() { (1, 3, 2), (0, 3, 1), (0, 2, 3), (0, 1, 3) };
			List<(int, int, int)> hullFaces = new List<(int, int, int)>();

			for( int i = 0; i < faces.Count; i++ ) {
				(int, int, int) fA = faces[ i ];
				Vector3 oa = vertices[ fA.Item1 ] - center;
				Vector3 ab = vertices[ fA.Item2 ] - vertices[ fA.Item1 ];
				Vector3 ac = vertices[ fA.Item3 ] - vertices[ fA.Item1 ];
				Vector3 cross = Vector3.Cross( ab, ac );
				if( Vector3.Dot( cross, oa ) > 0 ) {
					faces[ i ] = (fA.Item1, fA.Item3, fA.Item2);
				}
			}

			//Counts down whenever prevCount remains the same as faces.Count
			//If it reaches 0, the while loop exits, because it is stuck.
			int it = 5;
			int prevCount = -1;
			while( faces.Count > 0 && prevCount != faces.Count && it > 0) {

			}

			return faces;
		}

	}
}
