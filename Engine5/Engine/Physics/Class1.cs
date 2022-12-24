using Engine.Datatypes;
using Engine.Datatypes.Vectors;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Physics;
internal class Class1 {

}

//public interface ICollisionShape<V> where V : System.Numerics.

public class Face {
	public Box<Vector3> A, B, C;

	public override string ToString() {
		return $"[{A.ID},{B.ID},{C.ID}] {A.Value}, {B.Value}, {C.Value}";
	}
}

public class Edge {
	public Box<Vector3> A, B;

	public Edge( Box<Vector3> a, Box<Vector3> b ) {
		this.A = a;
		this.B = b;
	}

	public override bool Equals( object? obj ) {
		if ( obj is not Edge e )
			return false;

		return ( e.A == A && e.B == B ) || ( e.B == A && e.A == B );
	}

	public override int GetHashCode() {
		return A.GetHashCode() + B.GetHashCode();
	}
	public override string ToString() {
		return $"[{A.ID},{B.ID}] {A.Value}, {B.Value}";
	}
}

public class Tetrahedron {
	public Box<Vector3> A, B, C, P;

	public Vector3 Centroid => ( A.Value + B.Value + C.Value + P.Value ) / 4;

	public Tetrahedron( Box<Vector3> a, Box<Vector3> b, Box<Vector3> c, Box<Vector3> p ) {
		this.A = a;
		this.B = b;
		this.C = c;
		this.P = p;
	}

	//V = (1/6) * |(p1 - p4) * ((p2 - p4) × (p3 - p4))|
	public float Volume => 1f / 6 * MathF.Abs( Vector3.Dot( A.Value - P.Value, Vector3.Cross( B.Value - A.Value, C.Value - A.Value ) ) );

	/*
		∑[V_T * (r^2 * I_3 - r * r^T)]
	 */

	public Matrix4x4 GetInertiaTensor( Vector3 centerOfMass ) {
		var P1 = A.Value - centerOfMass;
		var P2 = B.Value - centerOfMass;
		var P3 = C.Value - centerOfMass;
		var P4 = P.Value - centerOfMass;
		var P11 = P1 * P1;
		var P22 = P2 * P2;
		var P33 = P3 * P3;
		var P44 = P4 * P4;
		var P12 = P1 * P2;
		var P13 = P1 * P3;
		var P14 = P1 * P4;
		var P23 = P2 * P3;
		var P24 = P2 * P4;
		var P34 = P3 * P4;

		//	1 | 0 | 0 | 0
		//	1 | 1 | 0 | 0
		//	1 | 1 | 1 | 0
		//	1 | 1 | 1 | 1

		var det = 6 * Volume;
		var xx = det / 60 * (
			P11.Y +
			P12.Y + P22.Y +
			P13.Y + P23.Y + P33.Y +
			P14.Y + P24.Y + P34.Y + P44.Y +
			P11.Z +
			P12.Z + P22.Z +
			P13.Z + P23.Z + P33.Z +
			P14.Z + P24.Z + P34.Z + P44.Z
		);
		var yy = det / 60 * (
			P11.X +
			P12.X + P22.X +
			P13.X + P23.X + P33.X +
			P14.X + P24.X + P34.X + P44.X +
			P11.Z +
			P12.Z + P22.Z +
			P13.Z + P23.Z + P33.Z +
			P14.Z + P24.Z + P34.Z + P44.Z
		);
		var zz = det / 60 * (
			P11.X +
			P12.X + P22.X +
			P13.X + P23.X + P33.X +
			P14.X + P24.X + P34.X + P44.X +
			P11.Y +
			P12.Y + P22.Y +
			P13.Y + P23.Y + P33.Y +
			P14.Y + P24.Y + P34.Y + P44.Y
		);
		var yz = -det / 120 * (
			( 2 * P1.Y + P2.Y + P3.Y + P4.Y ) * P1.Z +
			( P1.Y + 2 * P2.Y + P3.Y + P4.Y ) * P2.Z +
			( P1.Y + P2.Y + 2 * P3.Y + P4.Y ) * P3.Z +
			( P1.Y + P2.Y + P3.Y + 2 * P4.Y ) * P4.Z );
		var xz = -det / 120 * (
			( 2 * P1.X + P2.X + P3.X + P4.X ) * P1.Z +
			( P1.X + 2 * P2.X + P3.X + P4.X ) * P2.Z +
			( P1.X + P2.X + 2 * P3.X + P4.X ) * P3.Z +
			( P1.X + P2.X + P3.X + 2 * P4.X ) * P4.Z );
		var xy = -det / 120 * (
			( 2 * P1.X + P2.X + P3.X + P4.X ) * P1.Y +
			( P1.X + 2 * P2.X + P3.X + P4.X ) * P2.Y +
			( P1.X + P2.X + 2 * P3.X + P4.X ) * P3.Y +
			( P1.X + P2.X + P3.X + 2 * P4.X ) * P4.Y );
		//xz and xy might need to swap positions in this matrix
		return new Matrix4x4(
			xx, xz, xy, 0, 
			xz, yy, yz, 0, 
			xy, yz, zz, 0,
			0, 0, 0, 0
		);
	}
}


public class ConvexHull3 {
	private readonly Box<Vector3>[] _vertices;
	private readonly List<Face> _faces;
	private readonly List<Tetrahedron> _tetrahedrons;

	public Box<Vector3> _centerOfMass { get; }
	public Vector3 CenterOfMass => _centerOfMass.Value;

	public ConvexHull3( Vector3[] vertices ) {
		this._vertices = vertices.Select( p => new Box<Vector3>( p ) ).ToArray();
		_faces = new();
		_tetrahedrons = new();
		_centerOfMass = new( Vector3.Zero );
		for ( int i = 0; i < vertices.Length; i++ )
			_centerOfMass.Value += vertices[ i ];
		_centerOfMass.Value /= vertices.Length;

		GenerateFaces();
		GenerateTetrahedrons();
	}

	private void GenerateTetrahedrons() {
		for ( int i = 0; i < _faces.Count; i++ ) {
			var face = _faces[ i ];
			_tetrahedrons.Add( new( face.A, face.B, face.C, _centerOfMass ) );
		}
	}

	public float Volume => _tetrahedrons.Sum( p => p.Volume );

	public Matrix4x4 GetInertiaTensor() {
		Matrix4x4 I = new Matrix4x4();
		for ( int i = 0; i < _tetrahedrons.Count; i++ ) {
			var tetrahedron = _tetrahedrons[ i ];
			I += tetrahedron.GetInertiaTensor( CenterOfMass );
			Console.WriteLine( i );
			Console.WriteLine( I );
		}
		I *= 1 / Volume;
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
			foreach ( var edge in singularEdges ) {
				excemptEdges.Remove( edge );
			}

			var validEdges = singularEdges.Except( excemptEdges );

			foreach ( var edge in validEdges )
				_faces.Add( new Face { A = edge.A, B = edge.B, C = farthestVertex } );
		}

	}

}