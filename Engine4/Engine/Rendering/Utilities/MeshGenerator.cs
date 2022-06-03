using System.Numerics;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Utilities;
public class MeshGenerator {
	public static VertexMesh<Vertex2> GenerateCircle( uint npoints ) {
		if ( npoints == 0 )
			throw new ArgumentOutOfRangeException( nameof( npoints ), "must be greater than 0." );
		float step = 360f / npoints;

		Vertex2[] vertices = new Vertex2[ npoints + 1 ];
		Vector2 currentPoint;

		for ( int i = 0; i < npoints; i++ ) {
			float deg = i * step;
			float x = MathF.Cos( deg / 180f * MathF.PI );
			float y = MathF.Sin( deg / 180f * MathF.PI );
			currentPoint = new Vector2( x, y );
			float m = MathF.Min( 0.5f / x, 0.5f / y );
			vertices[ i ] = new Vertex2( currentPoint, new Vector2( 0.5f ) + ( currentPoint * m ), Color16x4.White );
		}

		vertices[ npoints ] = new Vertex2( Vector2.Zero, new Vector2( 0.5f ), Color16x4.White );

		uint[] indices = new uint[ npoints * 3 ];

		for ( uint i = 0; i < npoints; i++ ) {
			indices[ i * 3 ] = npoints;
			indices[ ( i * 3 ) + 1 ] = i;
			indices[ ( i * 3 ) + 2 ] = ( i + 1 ) % npoints;
		}
		return new VertexMesh<Vertex2>( $"circle#{npoints}", vertices, indices );

	}

	/// <summary>
	/// Generates a normalized unit icosphere using the subdivisions parameter to determine the detail.
	/// </summary>
	/// <param name="subdivisions">The amount of subdivisions</param>
	/// <param name="normalizedByMethod">Should the mesh handle normalizing the vertices, or the method? The method one is most accurate.</param>
	/// <returns></returns>
	public static VertexMesh<Vertex3> GenerateIcosphere( int subdivisions ) {

		GenerateIcosphereVectors( subdivisions, out List<Vector3> vectors, out List<uint> indices );

		List<Vertex3> vertices = new();

		for ( int i = 0; i < vectors.Count; i++ ) {
			Vertex3 v = new Vertex3( vectors[ i ] ).SetNormal( vectors[ i ] );
			vertices.Add( v );
		}

		return new VertexMesh<Vertex3>( $"icosphere#{subdivisions}", vertices.ToArray(), indices.ToArray() );
	}

	public static void GenerateIcosphereVectors( int subdivisions, out List<Vector3> vectors, out List<uint> indices ) {
		vectors = new List<Vector3>();
		indices = new List<uint>();

		vectors.Add( new Vector3( 0, 1, 0 ) );

		float H_ANGLE = MathF.PI / 180 * 72;    // 72 degree = 360 / 5
		float V_ANGLE = MathF.Atan( 1f / 2 );  // elevation = 26.565 degree

		float hAngle1 = ( -MathF.PI / 2 ) - ( H_ANGLE / 2 );  // start from -126 deg at 1st row
		float hAngle2 = -MathF.PI / 2;                // start from -90 deg at 2nd row

		for ( int i = 1; i <= 5; ++i ) {
			int i1 = i;

			float y = MathF.Sin( V_ANGLE );
			float xz = MathF.Cos( V_ANGLE );

			vectors.Add( new Vector3( xz * MathF.Cos( hAngle1 ), y, xz * MathF.Sin( hAngle1 ) ) );

			hAngle1 += H_ANGLE;
		}

		for ( int i = 1; i <= 5; ++i ) {
			int i2 = i + 5;

			float y = (float) Math.Sin( V_ANGLE );
			float xz = (float) Math.Cos( V_ANGLE );

			vectors.Add( new Vector3( xz * MathF.Cos( hAngle2 ), -y, xz * MathF.Sin( hAngle2 ) ) );

			hAngle2 += H_ANGLE;
		}

		vectors.Add( new Vector3( 0, -1, 0 ) );

		for ( uint i = 0; i < 5; i++ ) {
			indices.Add( 0 );
			indices.Add( ( ( i + 1 ) % 5 ) + 1 );
			indices.Add( i + 1 );
		}

		for ( uint i = 0; i < 5; i++ ) {
			indices.Add( ( ( i + 1 ) % 5 ) + 1 );
			indices.Add( ( ( i + 1 ) % 5 ) + 6 );
			indices.Add( i + 6 );
		}

		for ( uint i = 0; i < 5; i++ ) {
			indices.Add( i + 6 );
			indices.Add( i + 1 );
			indices.Add( ( ( i + 1 ) % 5 ) + 1 );
		}

		for ( uint i = 0; i < 5; i++ ) {
			indices.Add( 11 );
			indices.Add( 5 + i + 1 );
			indices.Add( 5 + ( ( i + 1 ) % 5 ) + 1 );
		}

		List<uint> subIndices = new();
		for ( int i = 0; i < subdivisions; i++ ) {
			subIndices.Clear();

			for ( int ind = 0; ind < indices.Count; ind += 3 ) {

				uint ia = indices[ ind ];
				Vector3 a = vectors[ (int) ia ];
				uint ib = indices[ ind + 1 ];
				Vector3 b = vectors[ (int) ib ];
				uint ic = indices[ ind + 2 ];
				Vector3 c = vectors[ (int) ic ];

				Vector3 ab = FindHalfway( a, b );
				Vector3 bc = FindHalfway( b, c );
				Vector3 ca = FindHalfway( c, a );

				uint iab = (uint) vectors.Count;
				vectors.Add( ab );
				uint ibc = (uint) vectors.Count;
				vectors.Add( bc );
				uint ica = (uint) vectors.Count;
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
		return Vector3.Normalize( h );
	}
}
