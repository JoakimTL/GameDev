﻿namespace Engine.Standard.Render.Meshing;
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

public sealed class Icosphere {

	public readonly List<Vector3<double>> _vertices;
	private readonly List<List<uint>> _indicesAtEarlierSubdivisions;
	private readonly Dictionary<(int, int), int> _edgeMidpointCache;

	private bool _locked = false;
	public IReadOnlyList<Vector3<double>> Vertices => _vertices;

	public int Subdivisions { get; }

	public Icosphere( uint subdivisions, int normalizeUpTo = -1 ) {
		Subdivisions = (int) subdivisions;
		_locked = false;
		IcosphereGenerator.CreateIcosphere( out _vertices, out _indicesAtEarlierSubdivisions );
		_edgeMidpointCache = [];
		for (int i = 0; i < subdivisions; i++) {
			Subdivide();
			if (i < normalizeUpTo || normalizeUpTo == -1)
				NormalizeVectors( _vertices );
		}
		_locked = true;
	}

	public IReadOnlyList<uint> GetIndices( int level ) => _indicesAtEarlierSubdivisions[ level ];

	private static void NormalizeVectors( List<Vector3<double>> vectors ) {
		for (int i = 0; i < vectors.Count; i++)
			vectors[ i ] = vectors[ i ].Normalize<Vector3<double>, double>();
	}

	public int GetMidpoint( uint v1, uint v2 ) {
		// Ensure (min, max) ordering for the edge key
		(int, int) edgeKey = ((int) Math.Min( v1, v2 ), (int) Math.Max( v1, v2 ));

		if (_edgeMidpointCache.TryGetValue( edgeKey, out int midpointIndex ))
			return midpointIndex;

		if (_locked)
			throw new InvalidOperationException( "Cannot subdivide a locked icosphere" );

		// Calculate and normalize the midpoint
		Vector3<double> midpoint = (_vertices[ (int) v1 ] + _vertices[ (int) v2 ]) * 0.5f;

		// Add midpoint to vertex list and cache it
		midpointIndex = _vertices.Count;
		_vertices.Add( midpoint );
		_edgeMidpointCache[ edgeKey ] = midpointIndex;

		return midpointIndex;
	}

	private void Subdivide() {
		List<uint> newIndices = [];
		List<uint> currentIndices = _indicesAtEarlierSubdivisions[ ^1 ];
		_indicesAtEarlierSubdivisions.Add( newIndices );
		for (int i = 0; i < currentIndices.Count; i += 3) {
			uint v1 = currentIndices[ i ];
			uint v2 = currentIndices[ i + 1 ];
			uint v3 = currentIndices[ i + 2 ];

			int m1 = GetMidpoint( v1, v2 );
			int m2 = GetMidpoint( v2, v3 );
			int m3 = GetMidpoint( v3, v1 );

			// Add the four new triangles
			newIndices.AddRange( [ v1, (uint) m1, (uint) m3 ] );
			newIndices.AddRange( [ v2, (uint) m2, (uint) m1 ] );
			newIndices.AddRange( [ v3, (uint) m3, (uint) m2 ] );
			newIndices.AddRange( [ (uint) m1, (uint) m2, (uint) m3 ] );
		}
	}

	public IReadOnlyList<uint> GetSubdivision( uint indexA, uint indexB, uint indexC ) {
		int m1 = GetMidpoint( indexA, indexB );
		int m2 = GetMidpoint( indexB, indexC );
		int m3 = GetMidpoint( indexC, indexA );
		return [ 
			indexA, (uint) m1, (uint) m3, 
			indexB, (uint) m2, (uint) m1, 
			indexC, (uint) m3, (uint) m2, 
			(uint) m1, (uint) m2, (uint) m3 ];
	}
}

public sealed class TetrahedraSphereService() {

	public void CreateTetrahedra( out List<Vector3<double>> vectors, out List<uint> indices ) {
		vectors = [
			new Vector3<double>( 1, 1, 1 ),
			new Vector3<double>( 1, -1, -1 ),
			new Vector3<double>( -1, 1, -1 ),
			new Vector3<double>( -1, -1, 1 ),
		];
		indices = [
			0, 1, 2,
			3, 1, 0,
			0, 2, 3,
			3, 2, 1,
		];
	}

	public void GenerateTetrahedraVectors( int subdivisions, out List<Vector3<double>> vectors, out List<uint> indices ) {
		CreateTetrahedra( out vectors, out indices );
		NormalizeVectors( vectors );
		List<uint> tempIndices = [];
		for (int i = 0; i < subdivisions; i++) {
			Subdivide( vectors, indices, tempIndices );
			NormalizeVectors( vectors );
		}
	}

	private void Subdivide( List<Vector3<double>> vectors, List<uint> indices, List<uint> tempIndices ) {
		tempIndices.Clear();
		for (int ind = 0; ind < indices.Count; ind += 3) {

			uint ia = indices[ ind ];
			Vector3<double> a = vectors[ (int) ia ];
			uint ib = indices[ ind + 1 ];
			Vector3<double> b = vectors[ (int) ib ];
			uint ic = indices[ ind + 2 ];
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

			tempIndices.Add( ia );
			tempIndices.Add( iab );
			tempIndices.Add( ica );

			tempIndices.Add( ib );
			tempIndices.Add( ibc );
			tempIndices.Add( iab );

			tempIndices.Add( ic );
			tempIndices.Add( ica );
			tempIndices.Add( ibc );

			tempIndices.Add( iab );
			tempIndices.Add( ibc );
			tempIndices.Add( ica );

		}

		indices.Clear();
		indices.AddRange( tempIndices );
	}

	private void NormalizeVectors( List<Vector3<double>> vectors ) {
		for (int i = 0; i < vectors.Count; i++) {
			vectors[ i ] = vectors[ i ].Normalize<Vector3<double>, double>();
		}
	}
}
