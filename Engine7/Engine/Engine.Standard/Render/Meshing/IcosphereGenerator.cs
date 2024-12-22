namespace Engine.Standard.Render.Meshing;

public static class IcosphereGenerator {
	public static void CreateIcosphere( out List<Vector3<double>> vectors, out List<List<uint>> indices ) {
		vectors = [];
		List<uint> rootIndices = [];
		indices = [ rootIndices ];

		vectors.Add( new Vector3<double>( 0, 1, 0 ) );

		double H_ANGLE = Math.PI / 180 * 72;    // 72 degree = 360 / 5
		double V_ANGLE = Math.Atan( 1f / 2 );  // elevation = 26.565 degree

		double hAngle1 = (-Math.PI / 2) - (H_ANGLE / 2);  // start from -126 deg at 1st row
		double hAngle2 = -Math.PI / 2;                // start from -90 deg at 2nd row

		for (int i = 1; i <= 5; ++i) {
			int i1 = i;

			double y = Math.Sin( V_ANGLE );
			double xz = Math.Cos( V_ANGLE );

			vectors.Add( new Vector3<double>( xz * Math.Cos( hAngle1 ), y, xz * Math.Sin( hAngle1 ) ) );

			hAngle1 += H_ANGLE;
		}

		for (int i = 1; i <= 5; ++i) {
			int i2 = i + 5;

			double y = Math.Sin( V_ANGLE );
			double xz = Math.Cos( V_ANGLE );

			vectors.Add( new Vector3<double>( xz * Math.Cos( hAngle2 ), -y, xz * Math.Sin( hAngle2 ) ) );

			hAngle2 += H_ANGLE;
		}

		vectors.Add( new Vector3<double>( 0, -1, 0 ) );

		for (uint i = 0; i < 5; i++) {
			rootIndices.Add( 0 );
			rootIndices.Add( ((i + 1) % 5) + 1 );
			rootIndices.Add( i + 1 );
		}

		for (uint i = 0; i < 5; i++) {
			rootIndices.Add( ((i + 1) % 5) + 1 );
			rootIndices.Add( ((i + 1) % 5) + 6 );
			rootIndices.Add( i + 6 );
		}

		for (uint i = 0; i < 5; i++) {
			rootIndices.Add( i + 6 );
			rootIndices.Add( i + 1 );
			rootIndices.Add( ((i + 1) % 5) + 1 );
		}

		for (uint i = 0; i < 5; i++) {
			rootIndices.Add( 11 );
			rootIndices.Add( 5 + i + 1 );
			rootIndices.Add( 5 + ((i + 1) % 5) + 1 );
		}
	}

	public static void GenerateIcosphereVectors( int subdivisions, out List<Vector3<double>> vectors, out List<List<uint>> indices ) {
		CreateIcosphere( out vectors, out indices );
		for (int i = 0; i < subdivisions; i++)
			Subdivide( vectors, indices );
		NormalizeVectors( vectors );
	}

	private static void Subdivide( List<Vector3<double>> vectors, List<List<uint>> indices ) {
		List<uint> preSubdivisionIndices = indices[ ^1 ];
		List<uint> subDivisionIndices = [];
		for (int ind = 0; ind < preSubdivisionIndices.Count; ind += 3) {

			uint ia = preSubdivisionIndices[ ind ];
			Vector3<double> a = vectors[ (int) ia ];
			uint ib = preSubdivisionIndices[ ind + 1 ];
			Vector3<double> b = vectors[ (int) ib ];
			uint ic = preSubdivisionIndices[ ind + 2 ];
			Vector3<double> c = vectors[ (int) ic ];

			Vector3<double> ab = (a + b) / 2;
			Vector3<double> bc = (b + c) / 2;
			Vector3<double> ca = (c + a) / 2;

			uint iab = (uint) vectors.Count;
			vectors.Add( ab );
			uint ibc = (uint) vectors.Count;
			vectors.Add( bc );
			uint ica = (uint) vectors.Count;
			vectors.Add( ca );

			subDivisionIndices.Add( ia );
			subDivisionIndices.Add( iab );
			subDivisionIndices.Add( ica );

			subDivisionIndices.Add( ib );
			subDivisionIndices.Add( ibc );
			subDivisionIndices.Add( iab );

			subDivisionIndices.Add( ic );
			subDivisionIndices.Add( ica );
			subDivisionIndices.Add( ibc );

			subDivisionIndices.Add( iab );
			subDivisionIndices.Add( ibc );
			subDivisionIndices.Add( ica );

		}
		indices.Add( subDivisionIndices );
	}

	private static void NormalizeVectors( List<Vector3<double>> vectors ) {
		for (int i = 0; i < vectors.Count; i++) {
			vectors[ i ] = vectors[ i ].Normalize<Vector3<double>, double>();
		}
	}

	//Project centers of triangles to polar coordinates. Use subdivision 2 or 3 here, as these will be the baseline for the icosphere anyways.
	//Place them in an octree.
	//When trying to figure out which triangles should be most subdivided use a projection of the camera view to the octree. This means take the coordinates of the camera, and see where the projection edges (near to far each corner) intersects the sphere (line segment - sphere intersection (and find the intersection closest to the camera per edge)).
	//Now travel the octree to find which triangles should be rendered at what LOD. (not sure how to do this yet)
	//When hovering over a tile, the tile should bounce out and have an outline under to prevent holes in the earth
}
