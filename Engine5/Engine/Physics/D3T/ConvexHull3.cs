using Engine.Datatypes;
using Engine.Datatypes.Vectors;
using System.Numerics;

namespace Engine.Physics.D3T;

public class ConvexHull3 {
	private readonly Box<Vector3>[] _vertices;
	private readonly List<Face> _faces;
	private readonly List<Tetrahedron> _tetrahedrons;

	public Box<Vector3> _centerOfMass { get; }
	public Vector3 CenterOfMass => _centerOfMass.Value;
	public Box<Vector3> _centroid { get; }
	public Vector3 Centroid => _centroid.Value;

	private AABB3 _outerHull;

	public ConvexHull3( Vector3[] vertices, float mass ) {
		//http://www.kwon3d.com/theory/moi/triten.html
		_vertices = vertices.Select( p => new Box<Vector3>( p ) ).ToArray();
		_faces = new();
		_tetrahedrons = new();
		_centroid = new( Vector3.Zero );
		for ( int i = 0; i < vertices.Length; i++ )
			_centroid.Value += vertices[ i ];
		_centroid.Value /= vertices.Length;
		_centerOfMass = new( Vector3.Zero );

		GenerateFaces();
		GenerateTetrahedrons();

		float totalVolume = 0;
		for ( int i = 0; i < _tetrahedrons.Count; i++ ) {
			var tetrahedron = _tetrahedrons[ i ];
			var volume = tetrahedron.Volume;
			_centerOfMass.Value += tetrahedron.Centroid * volume;
			totalVolume += volume;
		}
		_centerOfMass.Value /= totalVolume;
		Console.WriteLine( totalVolume );
		this.Mass = mass;
	}

	private void GenerateTetrahedrons() {
		for ( int i = 0; i < _faces.Count; i++ ) {
			var face = _faces[ i ];
			_tetrahedrons.Add( new( face.A, face.B, face.C, _centroid ) );
		}
	}

	public float Volume => _tetrahedrons.Sum( p => p.Volume );

	public float Mass { get; }

	public Matrix4x4 GetInertiaTensor() {
		Matrix4x4 I = new Matrix4x4();
		for ( int i = 0; i < _tetrahedrons.Count; i++ ) {
			var tetrahedron = _tetrahedrons[ i ];
			I += tetrahedron.GetInertiaTensor( CenterOfMass );
		}
		I *= Mass / Volume;
		return I;
	}

	private void GenerateFaces() {
		List<Box<Vector3>> vertices = new();

		var orderedVertices = _vertices.OrderBy( v => v.Value.X ).ThenBy( v => v.Value.Y ).ThenBy( v => v.Value.Z ).ToList();
		// Find the vertex with the maximum x, y, and z coordinates
		var v1 = orderedVertices.First();

		// Find the vertex with the minimum x, y, and z coordinates
		var v2 = orderedVertices.Last();

		// Find the vertex with the maximum distance to the line segment connecting vertexMax and vertexMin
		var v3 = _vertices.OrderByDescending( v => {
			var line = v1.Value - v2.Value;
			var projection = Vector3.Dot( v.Value - v2.Value, line ) / line.LengthSquared();
			return Vector3.Distance( v.Value, v2.Value + projection * line );
		} ).First();

		// Calculate the normal vector of the plane formed by vertexMax, vertexMin, and vertexFarthest
		var normal = Vector3.Cross( v2.Value - v1.Value, v3.Value - v1.Value );

		// Find the vertex with the maximum distance to the plane formed by vertexMax, vertexMin, and vertexFarthest
		var v4 = _vertices.OrderByDescending( v => {
			var d = Vector3.Dot( v.Value - v1.Value, normal ) / normal.Length();
			return Math.Abs( d );
		} ).First();

		bool windingRight = Vector3.Dot( v4.Value - v1.Value, normal ) / normal.Length() > 0;

		// Add the initial faces to the convex hull
		if ( windingRight ) {
			_faces.Add( new Face { A = v1, B = v2, C = v3 } );
			_faces.Add( new Face { A = v1, B = v4, C = v2 } );
			_faces.Add( new Face { A = v1, B = v3, C = v4 } );
			_faces.Add( new Face { A = v4, B = v3, C = v2 } );
		} else {
			_faces.Add( new Face { A = v1, B = v3, C = v2 } );
			_faces.Add( new Face { A = v1, B = v2, C = v4 } );
			_faces.Add( new Face { A = v1, B = v4, C = v3 } );
			_faces.Add( new Face { A = v4, B = v2, C = v3 } );
		}

		// Add the initial face to the list of processed faces
		var processedFaces = _faces.ToList();
		vertices.Add( v1 );
		vertices.Add( v2 );
		vertices.Add( v3 );
		vertices.Add( v4 );

		// Initialize the list of unprocessed vertices
		var unprocessedVertices = _vertices.Except( vertices ).ToList();

		// Iterate until all vertices have been processed
		while ( unprocessedVertices.Count > 0 ) {
			// Find the unprocessed vertex that is farthest from the convex hull
			var farthestVertex = unprocessedVertices.OrderByDescending( v => {
				// Find the face of the convex hull that is closest to the vertex
				var closestFace = _faces.OrderBy( f => {
					// Calculate the distance from the vertex to the plane of the face
					var n = Vector3.Cross( f.B.Value - f.A.Value, f.C.Value - f.A.Value );
					var d = Vector3.Dot( n, f.A.Value - v.Value );
					return Math.Abs( d ) / n.Length();
				} ).First();

				// Calculate the distance from the vertex to the closest face
				var n = Vector3.Cross( closestFace.B.Value - closestFace.A.Value, closestFace.C.Value - closestFace.A.Value );
				var d = Vector3.Dot( n, closestFace.A.Value - v.Value );
				return Math.Abs( d ) / n.Length();
			} ).First();

			// Remove the farthest vertex from the list of unprocessed vertices
			unprocessedVertices.Remove( farthestVertex );

			// Find the faces of the convex hull that can see the farthest vertex
			var visibleFaces = _faces.Where( f => Vector3.Dot( Vector3.Cross( f.B.Value - f.A.Value, f.C.Value - f.A.Value ), f.A.Value - farthestVertex.Value ) > 0 ).ToList();

			foreach ( var f in _faces ) {
				var fnormal = Vector3.Cross( f.B.Value - f.A.Value, f.C.Value - f.A.Value );
				var fa = f.A.Value - farthestVertex.Value;
				var dot = Vector3.Dot( fnormal, fa );
				var visible = dot > 0;
			}

			// Remove the visible faces from the convex hull
			foreach ( var face in visibleFaces )
				_faces.Remove( face );

			// Add the farthest vertex to the convex hull
			vertices.Add( farthestVertex );

			var allEdges = visibleFaces.SelectMany( p => {
				return new Edge[] { new( p.A, p.B ), new( p.B, p.C ), new( p.C, p.A ) };
			} ).ToList();

			var singularEdges = allEdges.ToHashSet();

			var excemptEdges = allEdges.ToList();
			foreach ( var edge in singularEdges )
				excemptEdges.Remove( edge );

			var validEdges = singularEdges.Except( excemptEdges );

			foreach ( var edge in validEdges )
				_faces.Add( new Face { A = edge.A, B = edge.B, C = farthestVertex } );
		}

	}

}