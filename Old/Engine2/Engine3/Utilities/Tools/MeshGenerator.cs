using Engine.Graphics.Objects.Default.Meshes.VertexMeshing;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Tools {
	public static class MeshGenerator {

		public static Mesh2 GenerateCircle( int npoints ) {
			if( npoints <= 0 )
				throw new ArgumentOutOfRangeException( "npoints must be higher than 0." );
			float step = 360f / npoints;

			Vertex2[] vertices = new Vertex2[ npoints + 1 ];
			Vector2 currentPoint = 0;

			for( int i = 0; i < npoints; i++ ) {
				float deg = i * step;
				float x = (float) Math.Cos( deg / 180f * Math.PI );
				float y = (float) Math.Sin( deg / 180f * Math.PI );
				currentPoint = new Vector2( x, y );
				float m = Math.Min( 0.5f / x, 0.5f / y );
				vertices[ i ] = new Vertex2( currentPoint, 0.5f + currentPoint * m );
			}

			vertices[ npoints ] = new Vertex2( 0, 0.5f );

			int[] indices = new int[ npoints * 3 ];

			for( int i = 0; i < npoints; i++ ) {
				indices[ i * 3 ] = npoints;
				indices[ i * 3 + 1 ] = i;
				indices[ i * 3 + 2 ] = ( i + 1 ) % npoints;
			}
			return new Mesh2( "circle", vertices, indices );

		}

		/*internal static Mesh3Simple GenerateIcosphereSimple( int subdivisions ) {

			GenerateIcosphereVectors( subdivisions, out List<Vector3> vectors, out List<int> indices );

			List<Vertex3Simple> vertices = new List<Vertex3Simple>();

			Random rand = new Random();
			for( int i = 0; i < vectors.Count; i++ ) {
				Vertex3Simple v = new Vertex3Simple() {
					Position = vectors[ i ]
				};
				vertices.Add( v );
			}

			return new Mesh3Simple( "icosphere", OpenGL.BufferUsage.DynamicDraw, vertices, indices );
		}*/

		/// <summary>
		/// Generates a normalized unit icosphere using the subdivisions parameter to determine the detail.
		/// </summary>
		/// <param name="subdivisions">The amount of subdivisions</param>
		/// <param name="normalizedByMethod">Should the mesh handle normalizing the vertices, or the method? The method one is most accurate.</param>
		/// <returns></returns>
		public static Mesh3 GenerateIcosphere( int subdivisions, bool normalizedByMethod ) {

			GenerateIcosphereVectors( subdivisions, out List<Vector3> vectors, out List<int> indices );

			List<Vertex3> vertices = new List<Vertex3>();

			for( int i = 0; i < vectors.Count; i++ ) {
				Vertex3 v = new Vertex3() {
					Position = vectors[ i ],
					Normal = vectors[ i ],
					Color = 255
					//UV = new Vector2((float) Math.Atan())
				};
				vertices.Add( v );
			}

			return new Mesh3( "icosphere", vertices, indices, !normalizedByMethod );
		}

		public static void GenerateIcosphereVectors( int subdivisions, out List<Vector3> vectors, out List<int> indices ) {
			vectors = new List<Vector3>();
			indices = new List<int>();

			vectors.Add( new Vector3( 0, 1, 0 ) );

			float H_ANGLE = (float) Math.PI / 180 * 72;    // 72 degree = 360 / 5
			float V_ANGLE = (float) Math.Atan( 1f / 2 );  // elevation = 26.565 degree

			float hAngle1 = -(float) Math.PI / 2 - H_ANGLE / 2;  // start from -126 deg at 1st row
			float hAngle2 = -(float) Math.PI / 2;                // start from -90 deg at 2nd row

			for( int i = 1; i <= 5; ++i ) {
				int i1 = i;

				float y = (float) Math.Sin( V_ANGLE );
				float xz = (float) Math.Cos( V_ANGLE );

				vectors.Add( new Vector3( xz * (float) Math.Cos( hAngle1 ), y, xz * (float) Math.Sin( hAngle1 ) ) );

				hAngle1 += H_ANGLE;
			}

			for( int i = 1; i <= 5; ++i ) {
				int i2 = ( i + 5 );

				float y = (float) Math.Sin( V_ANGLE );
				float xz = (float) Math.Cos( V_ANGLE );

				vectors.Add( new Vector3( xz * (float) Math.Cos( hAngle2 ), -y, xz * (float) Math.Sin( hAngle2 ) ) );

				hAngle2 += H_ANGLE;
			}

			vectors.Add( new Vector3( 0, -1, 0 ) );

			for( int i = 0; i < 5; i++ ) {
				indices.Add( 0 );
				indices.Add( ( i + 1 ) % 5 + 1 );
				indices.Add( i + 1 );
			}

			for( int i = 0; i < 5; i++ ) {
				indices.Add( ( i + 1 ) % 5 + 1 );
				indices.Add( ( i + 1 ) % 5 + 6 );
				indices.Add( i + 6 );
			}

			for( int i = 0; i < 5; i++ ) {
				indices.Add( i + 6 );
				indices.Add( i + 1 );
				indices.Add( ( i + 1 ) % 5 + 1 );
			}

			for( int i = 0; i < 5; i++ ) {
				indices.Add( 11 );
				indices.Add( 5 + i + 1 );
				indices.Add( 5 + ( i + 1 ) % 5 + 1 );
			}

			List<int> subIndices = new List<int>();
			for( int i = 0; i < subdivisions; i++ ) {
				subIndices.Clear();

				for( int ind = 0; ind < indices.Count; ind += 3 ) {

					int ia = indices[ ind ];
					Vector3 a = vectors[ ia ];
					int ib = indices[ ind + 1 ];
					Vector3 b = vectors[ ib ];
					int ic = indices[ ind + 2 ];
					Vector3 c = vectors[ ic ];

					Vector3 ab = FindHalfway( a, b );
					Vector3 bc = FindHalfway( b, c );
					Vector3 ca = FindHalfway( c, a );

					int iab = vectors.Count;
					vectors.Add( ab );
					int ibc = vectors.Count;
					vectors.Add( bc );
					int ica = vectors.Count;
					vectors.Add( ca );

					subIndices.Add( ia );
					subIndices.Add( iab );
					subIndices.Add( ica );

					subIndices.Add( ib );
					subIndices.Add( ibc );
					subIndices.Add( iab );

					subIndices.Add( ic );
					subIndices.Add( ica );
					subIndices.Add( ibc );

					subIndices.Add( iab );
					subIndices.Add( ibc );
					subIndices.Add( ica );

				}

				indices.Clear();
				indices.AddRange( subIndices );
			}
		}

		private static Vector3 FindHalfway( Vector3 a, Vector3 b ) {
			Vector3 h = a + b;
			return h.Normalized;
		}

	}
}
