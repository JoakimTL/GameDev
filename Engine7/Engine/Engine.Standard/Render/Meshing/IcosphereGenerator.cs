using System.Numerics;

namespace Engine.Standard.Render.Meshing;

public static class IcosphereGenerator {
	public static void CreateIcosphere<TScalar>( out List<Vector3<TScalar>> vectors, out List<List<uint>> indices )
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		vectors = [];
		List<uint> rootIndices = [];
		indices = [ rootIndices ];

		vectors.Add( new Vector3<TScalar>( TScalar.CreateSaturating( 0 ), TScalar.CreateSaturating( 1 ), TScalar.CreateSaturating( 0 ) ) );

		TScalar H_ANGLE = TScalar.Pi / TScalar.CreateSaturating( 180 ) * TScalar.CreateSaturating( 72 );    // 72 degree = 360 / 5
		TScalar V_ANGLE = TScalar.Atan( TScalar.CreateSaturating( 1 ) / TScalar.CreateSaturating( 2 ) );  // elevation = 26.565 degree

		TScalar hAngle1 = (-TScalar.Pi / TScalar.CreateSaturating( 2 )) - (H_ANGLE / TScalar.CreateSaturating( 2 ));  // start from -126 deg at 1st row
		TScalar hAngle2 = -TScalar.Pi / TScalar.CreateSaturating( 2 );                // start from -90 deg at 2nd row

		for (int i = 1; i <= 5; ++i) {
			int i1 = i;

			TScalar y = TScalar.Sin( V_ANGLE );
			TScalar xz = TScalar.Cos( V_ANGLE );

			vectors.Add( new Vector3<TScalar>( xz * TScalar.Cos( hAngle1 ), y, xz * TScalar.Sin( hAngle1 ) ) );

			hAngle1 += H_ANGLE;
		}

		for (int i = 1; i <= 5; ++i) {
			int i2 = i + 5;

			TScalar y = TScalar.Sin( V_ANGLE );
			TScalar xz = TScalar.Cos( V_ANGLE );

			vectors.Add( new Vector3<TScalar>( xz * TScalar.Cos( hAngle2 ), -y, xz * TScalar.Sin( hAngle2 ) ) );

			hAngle2 += H_ANGLE;
		}

		vectors.Add( new Vector3<TScalar>( TScalar.CreateSaturating( 0 ), TScalar.CreateSaturating( -1 ), TScalar.CreateSaturating( 0 ) ) );

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

	public static void GenerateIcosphereVectors<TScalar>( int subdivisions, out List<Vector3<TScalar>> vectors, out List<List<uint>> indices )
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		CreateIcosphere( out vectors, out indices );
		for (int i = 0; i < subdivisions; i++)
			Subdivide( vectors, indices );
		NormalizeVectors( vectors );
	}

	private static void Subdivide<TScalar>( List<Vector3<TScalar>> vectors, List<List<uint>> indices )
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		List<uint> preSubdivisionIndices = indices[ ^1 ];
		List<uint> subDivisionIndices = [];
		for (int ind = 0; ind < preSubdivisionIndices.Count; ind += 3) {

			uint ia = preSubdivisionIndices[ ind ];
			Vector3<TScalar> a = vectors[ (int) ia ];
			uint ib = preSubdivisionIndices[ ind + 1 ];
			Vector3<TScalar> b = vectors[ (int) ib ];
			uint ic = preSubdivisionIndices[ ind + 2 ];
			Vector3<TScalar> c = vectors[ (int) ic ];

			Vector3<TScalar> ab = (a + b) / TScalar.CreateSaturating( 2 );
			Vector3<TScalar> bc = (b + c) / TScalar.CreateSaturating( 2 );
			Vector3<TScalar> ca = (c + a) / TScalar.CreateSaturating( 2 );

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

	private static void NormalizeVectors<TScalar>( List<Vector3<TScalar>> vectors )
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		for (int i = 0; i < vectors.Count; i++) {
			vectors[ i ] = vectors[ i ].Normalize<Vector3<TScalar>, TScalar>();
		}
	}

	//Project centers of triangles to polar coordinates. Use subdivision 2 or 3 here, as these will be the baseline for the icosphere anyways.
	//Place them in an octree.
	//When trying to figure out which triangles should be most subdivided use a projection of the camera view to the octree. This means take the coordinates of the camera, and see where the projection edges (near to far each corner) intersects the sphere (line segment - sphere intersection (and find the intersection closest to the camera per edge)).
	//Now travel the octree to find which triangles should be rendered at what LOD. (not sure how to do this yet)
	//When hovering over a tile, the tile should bounce out and have an outline under to prevent holes in the earth
}
