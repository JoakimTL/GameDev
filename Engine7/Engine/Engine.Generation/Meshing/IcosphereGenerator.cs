using System.Numerics;

namespace Engine.Generation.Meshing;

public static class IcosphereGenerator {
	public const double GoldenRatio = 1.618033988749894;

	public static void CreateIcosphere<TScalar>( out List<Vector3<TScalar>> vectors, out List<uint> rootIndices )
		where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

		vectors = [];
		rootIndices = [];

		TScalar phi = TScalar.CreateSaturating( GoldenRatio );
		TScalar scaling = TScalar.Sqrt( TScalar.One + phi * phi );
		TScalar scalingInverse = TScalar.One / scaling;

		vectors.Add( (phi, TScalar.One, TScalar.Zero) );  // 0			|
		vectors.Add( (phi, -TScalar.One, TScalar.Zero) ); // 1			|
		vectors.Add( (-phi, TScalar.One, TScalar.Zero) ); // 2			|
		vectors.Add( (-phi, -TScalar.One, TScalar.Zero) );// 3			|
		vectors.Add( (TScalar.One, TScalar.Zero, phi) );  // 4			|
		vectors.Add( (-TScalar.One, TScalar.Zero, phi) ); // 5			|
		vectors.Add( (TScalar.One, TScalar.Zero, -phi) ); // 6			|
		vectors.Add( (-TScalar.One, TScalar.Zero, -phi) );// 7			|
		vectors.Add( (TScalar.Zero, phi, TScalar.One) );  // 8			|
		vectors.Add( (TScalar.Zero, phi, -TScalar.One) ); // 9			|
		vectors.Add( (TScalar.Zero, -phi, TScalar.One) ); // 10			|
		vectors.Add( (TScalar.Zero, -phi, -TScalar.One) );// 11			|

		for (int i = 0; i < vectors.Count; i++)
			vectors[ i ] *= scalingInverse;

		rootIndices.AddRange( [ 8, 0, 9 ] );        // 1
		rootIndices.AddRange( [ 8, 9, 2 ] );        // 2
		rootIndices.AddRange( [ 11, 1, 10 ] );      // 3
		rootIndices.AddRange( [ 11, 10, 3 ] );      // 4
		rootIndices.AddRange( [ 0, 1, 6 ] );        // 5
		rootIndices.AddRange( [ 0, 4, 1 ] );        // 6
		rootIndices.AddRange( [ 3, 2, 7 ] );        // 7
		rootIndices.AddRange( [ 3, 5, 2 ] );        // 8
		rootIndices.AddRange( [ 4, 5, 10 ] );       // 9
		rootIndices.AddRange( [ 4, 8, 5 ] );        // 10
		rootIndices.AddRange( [ 7, 6, 11 ] );       // 11
		rootIndices.AddRange( [ 7, 9, 6 ] );        // 12
		
		rootIndices.AddRange( [ 8, 4, 0 ] );        // 13
		rootIndices.AddRange( [ 8, 2, 5 ] );        // 14
		rootIndices.AddRange( [ 10, 1, 4 ] );       // 15
		rootIndices.AddRange( [ 10, 5, 3 ] );       // 16
		rootIndices.AddRange( [ 9, 0, 6 ] );        // 17
		rootIndices.AddRange( [ 9, 7, 2 ] );        // 18
		rootIndices.AddRange( [ 11, 6, 1 ] );       // 19
		rootIndices.AddRange( [ 11, 3, 7 ] );       // 20

		//vectors.Add( new Vector3<TScalar>( TScalar.CreateSaturating( 0 ), TScalar.CreateSaturating( 1 ), TScalar.CreateSaturating( 0 ) ) );

		//TScalar H_ANGLE = TScalar.Pi / TScalar.CreateSaturating( 180 ) * TScalar.CreateSaturating( 72 );    // 72 degree = 360 / 5
		//TScalar V_ANGLE = TScalar.Atan( TScalar.CreateSaturating( 1 ) / TScalar.CreateSaturating( 2 ) );  // elevation = 26.565 degree

		//TScalar hAngle1 = (-TScalar.Pi / TScalar.CreateSaturating( 2 )) - (H_ANGLE / TScalar.CreateSaturating( 2 ));  // start from -126 deg at 1st row
		//TScalar hAngle2 = -TScalar.Pi / TScalar.CreateSaturating( 2 );                // start from -90 deg at 2nd row

		//for (int i = 1; i <= 5; ++i) {
		//	int i1 = i;

		//	TScalar y = TScalar.Sin( V_ANGLE );
		//	TScalar xz = TScalar.Cos( V_ANGLE );

		//	vectors.Add( new Vector3<TScalar>( xz * TScalar.Cos( hAngle1 ), y, xz * TScalar.Sin( hAngle1 ) ) );

		//	hAngle1 += H_ANGLE;
		//}

		//for (int i = 1; i <= 5; ++i) {
		//	int i2 = i + 5;

		//	TScalar y = TScalar.Sin( V_ANGLE );
		//	TScalar xz = TScalar.Cos( V_ANGLE );

		//	vectors.Add( new Vector3<TScalar>( xz * TScalar.Cos( hAngle2 ), -y, xz * TScalar.Sin( hAngle2 ) ) );

		//	hAngle2 += H_ANGLE;
		//}

		//vectors.Add( new Vector3<TScalar>( TScalar.CreateSaturating( 0 ), TScalar.CreateSaturating( -1 ), TScalar.CreateSaturating( 0 ) ) );

		//for (uint i = 0; i < 5; i++) {
		//	rootIndices.Add( 0 );
		//	rootIndices.Add( ((i + 1) % 5) + 1 );
		//	rootIndices.Add( i + 1 );
		//}

		//for (uint i = 0; i < 5; i++) {
		//	rootIndices.Add( ((i + 1) % 5) + 1 );
		//	rootIndices.Add( ((i + 1) % 5) + 6 );
		//	rootIndices.Add( i + 6 );
		//}

		//for (uint i = 0; i < 5; i++) {
		//	rootIndices.Add( i + 6 );
		//	rootIndices.Add( i + 1 );
		//	rootIndices.Add( ((i + 1) % 5) + 1 );
		//}

		//for (uint i = 0; i < 5; i++) {
		//	rootIndices.Add( 11 );
		//	rootIndices.Add( 5 + i + 1 );
		//	rootIndices.Add( 5 + ((i + 1) % 5) + 1 );
		//}
	}

	//public static void GenerateSubdividedIcosphere<TScalar>( int subdivisions, out List<Vector3<TScalar>> vectors, out List<uint> indices )
	//	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
	//	CreateIcosphere( out vectors, out indices );
	//	for (int i = 0; i < subdivisions; i++)
	//		Subdivide( vectors, indices );
	//	NormalizeVectors( vectors );
	//}

	//private static void Subdivide<TScalar>( List<Vector3<TScalar>> vectors, List<List<uint>> indices )
	//	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
	//	List<uint> preSubdivisionIndices = indices[ ^1 ];
	//	List<uint> subDivisionIndices = [];
	//	for (int ind = 0; ind < preSubdivisionIndices.Count; ind += 3) {

	//		uint ia = preSubdivisionIndices[ ind ];
	//		Vector3<TScalar> a = vectors[ (int) ia ];
	//		uint ib = preSubdivisionIndices[ ind + 1 ];
	//		Vector3<TScalar> b = vectors[ (int) ib ];
	//		uint ic = preSubdivisionIndices[ ind + 2 ];
	//		Vector3<TScalar> c = vectors[ (int) ic ];

	//		Vector3<TScalar> ab = (a + b) / TScalar.CreateSaturating( 2 );
	//		Vector3<TScalar> bc = (b + c) / TScalar.CreateSaturating( 2 );
	//		Vector3<TScalar> ca = (c + a) / TScalar.CreateSaturating( 2 );

	//		uint iab = (uint) vectors.Count;
	//		vectors.Add( ab );
	//		uint ibc = (uint) vectors.Count;
	//		vectors.Add( bc );
	//		uint ica = (uint) vectors.Count;
	//		vectors.Add( ca );

	//		subDivisionIndices.Add( ia );
	//		subDivisionIndices.Add( iab );
	//		subDivisionIndices.Add( ica );

	//		subDivisionIndices.Add( ib );
	//		subDivisionIndices.Add( ibc );
	//		subDivisionIndices.Add( iab );

	//		subDivisionIndices.Add( ic );
	//		subDivisionIndices.Add( ica );
	//		subDivisionIndices.Add( ibc );

	//		subDivisionIndices.Add( iab );
	//		subDivisionIndices.Add( ibc );
	//		subDivisionIndices.Add( ica );

	//	}
	//	indices.Add( subDivisionIndices );
	//}

	//private static void NormalizeVectors<TScalar>( List<Vector3<TScalar>> vectors )
	//	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
	//	for (int i = 0; i < vectors.Count; i++) {
	//		vectors[ i ] = vectors[ i ].Normalize<Vector3<TScalar>, TScalar>();
	//	}
	//}

	//Project centers of triangles to polar coordinates. Use subdivision 2 or 3 here, as these will be the baseline for the icosphere anyways.
	//Place them in an octree.
	//When trying to figure out which triangles should be most subdivided use a projection of the camera view to the octree. This means take the coordinates of the camera, and see where the projection edges (near to far each corner) intersects the sphere (line segment - sphere intersection (and find the intersection closest to the camera per edge)).
	//Now travel the octree to find which triangles should be rendered at what LOD. (not sure how to do this yet)
	//When hovering over a tile, the tile should bounce out and have an outline under to prevent holes in the earth
}
