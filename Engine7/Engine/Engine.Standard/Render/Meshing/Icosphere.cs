namespace Engine.Standard.Render.Meshing;

public sealed class Icosphere {

	public readonly List<Vector3<float>> _vertices;
	private readonly List<List<uint>> _indicesAtEarlierSubdivisions;
	private readonly Dictionary<(int, int), int> _edgeMidpointCache;

	private bool _locked = false;
	public IReadOnlyList<Vector3<float>> Vertices => _vertices;

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

	private static void NormalizeVectors( List<Vector3<float>> vectors ) {
		for (int i = 0; i < vectors.Count; i++)
			vectors[ i ] = vectors[ i ].Normalize<Vector3<float>, float>();
	}

	public int GetMidpoint( uint v1, uint v2 ) {
		// Ensure (min, max) ordering for the edge key
		(int, int) edgeKey = ((int) Math.Min( v1, v2 ), (int) Math.Max( v1, v2 ));

		if (_edgeMidpointCache.TryGetValue( edgeKey, out int midpointIndex ))
			return midpointIndex;

		if (_locked)
			throw new InvalidOperationException( "Cannot subdivide a locked icosphere" );

		// Calculate and normalize the midpoint
		Vector3<float> midpoint = (_vertices[ (int) v1 ] + _vertices[ (int) v2 ]) * 0.5f;

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
