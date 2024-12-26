using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision.D3 {
	public static class EPA3 {

		/// <summary>
		/// Because of floating point errors, a sufficiently small bias is used to counteract the errors. <br/>
		/// If two shapes are very close to colliding and this bias triggers a collision check the human eye couldn't spot the difference anyways<br/>
		/// In this engine, this bias is <b>~0.061mm (2^-14)</b>
		/// </summary>
		public const float EPSILON = 0.00006103515f;

		public struct EPAFace3 {
			public float distance;
			public Vector3 normal;
			public int faceId;
		}

		public struct EPAEdge3 {
			public int a, b;
		}

		//This implementation of EPA is not as efficient as it can be, but it works well.
		/// <summary>
		/// Executes one step of the EPA
		/// </summary>
		/// <param name="hull">The hull from the GJK run</param>
		/// <param name="faces">The faces on the hull</param>
		/// <param name="shape1">The first shape, the order must be the same as for the GJK run</param>
		/// <param name="shape2">The second shape, the order must be the same as for the GJK run</param>
		/// <param name="res">The result of the step.</param>
		/// <returns>Wether this step is terminal or not.</returns>
		public static bool PerformStep( Hull3 hull, ref Hull3Faces faces, PhysicsShape<Transform3, Vector3> shape1, PhysicsShape<Transform3, Vector3> shape2, ref CollisionResult<Vector3>.CollisionData res ) {
			//If the hullfaces haven't been created, do so before we use them
			if( faces is null )
				faces = new Hull3Faces();

			//Find the closest face to the origin in the hull.
			EPAFace3 face = FindClosestFace( hull, faces );
			//Check if there is a support point further away from this face.
			Support3 newSupp = hull.GetSupport( shape1, shape2, face.normal, out _ );
			//Get the distance between the new support point the origin
			float dist = Vector3.Dot( newSupp.Sum, face.normal );

			if( dist - face.distance < EPSILON ) {
				//The face is the closest, but also the furthest away in the direction it is facing.
				//This means this face is the point of contact.

				Hull3Face closestFace = faces[ face.faceId ];
				//Find the barycentric coordinates of the origin on the face triangle.
				Vector3 bary = Vector3.ToBartcentricProjectedTriangle( hull[ closestFace.A ].Sum, hull[ closestFace.B ].Sum, hull[ closestFace.C ].Sum, 0 );

				//Find the points on each shape that represent the point of contact for both shapes.
				Vector3 closestA = hull[ closestFace.A ].A * bary.X + hull[ closestFace.B ].A * bary.Y + hull[ closestFace.C ].A * bary.Z;
				Vector3 closestB = hull[ closestFace.A ].B * bary.X + hull[ closestFace.B ].B * bary.Y + hull[ closestFace.C ].B * bary.Z;

				Vector3 aAB = ( hull[ closestFace.B ].A - hull[ closestFace.A ].A );
				float aABlensq = aAB.LengthSquared;
				Vector3 aAC = ( hull[ closestFace.C ].A - hull[ closestFace.A ].A );
				float aAClensq = aAC.LengthSquared;
				Vector3 bAB = ( hull[ closestFace.B ].B - hull[ closestFace.A ].B );
				float bABlensq = bAB.LengthSquared;
				Vector3 bAC = ( hull[ closestFace.C ].B - hull[ closestFace.A ].B );
				float bAClensq = bAC.LengthSquared;

				bool parallel = false;
				if( !(aABlensq == 0 || aAClensq == 0 || bABlensq == 0 || bAClensq == 0) ) {
					Vector3 crossA = Vector3.Cross( aAB / (float) Math.Sqrt( aABlensq ), aAC / (float) Math.Sqrt( aAClensq ) );
					Vector3 crossB = Vector3.Cross( bAB / (float) Math.Sqrt( bABlensq ), bAC / (float) Math.Sqrt( bAClensq ) );

					parallel = ( 1 - Math.Abs( Vector3.Dot( crossA, crossB ) ) ) < EPSILON;
				}

				//Return the resulting depth, normal and points of contact.
				res = new CollisionResult<Vector3>.CollisionData( face.distance, face.normal, closestB, closestA, parallel );
				return true;
			}

			//The face was the closest in the hull, another point was further away, thus this was not the closest and also the furthest away.
			//We add the new support to the hull
			hull.AddSupport( newSupp );
			//And reconstruct the faces to make sure all the faces are winding correctly and no internal faces exists.
			Reconstruct( hull, faces, newSupp );
			return false;
		}

		private static void Reconstruct( Hull3 simplex, Hull3Faces faces, Support3 newSupp ) {
			List<int> facesToRemove = new List<int>();

			//Removes all the faces which can "see" the new support point, meaning the normal vector of the face is aligned (dot > 0) with the support.
			for( int i = 0; i < faces.Count; i++ ) {
				Hull3Face face = faces[ i ];

				Vector3 ab = simplex[ face.B ].Sum - simplex[ face.A ].Sum;
				Vector3 ac = simplex[ face.C ].Sum - simplex[ face.A ].Sum;
				Vector3 norm = Vector3.Cross( ab, ac ).Normalized;

				//newSupp.Sum - simplex[ face.A ].Sum because this is relative to the face, not absolute.
				if( Vector3.Dot( norm, newSupp.Sum - simplex[ face.A ].Sum ) > 0 )
					facesToRemove.Add( i );
			}

			List<EPAEdge3> edges = new List<EPAEdge3>();
			for( int i = 0; i < facesToRemove.Count; i++ ) {
				var face = faces[ facesToRemove[ i ] ];
				//The new edges has correct winding
				EPAEdge3 edgeAB = new EPAEdge3() { a = face.A, b = face.B };
				EPAEdge3 edgeCA = new EPAEdge3() { a = face.C, b = face.A };
				EPAEdge3 edgeBC = new EPAEdge3() { a = face.B, b = face.C };

				{
					int k = EdgeInEdges( edges, edgeAB );
					if( k != -1 )
						edges.RemoveAt( k );
					else
						edges.Add( edgeAB );
				}
				{
					int k = EdgeInEdges( edges, edgeCA );
					if( k != -1 )
						edges.RemoveAt( k );
					else
						edges.Add( edgeCA );
				}
				{
					int k = EdgeInEdges( edges, edgeBC );
					if( k != -1 )
						edges.RemoveAt( k );
					else
						edges.Add( edgeBC );
				}
			}

			for( int i = facesToRemove.Count - 1; i >= 0; i-- ) {
				faces.RemoveFace( facesToRemove[ i ] );
			}

			for( var i = 0; i < edges.Count; i++ ) {
				faces.AddFace( new Hull3Face( edges[ i ].a, edges[ i ].b, simplex.Count - 1 ) );
			}
		}

		private static int EdgeInEdges( List<EPAEdge3> edges, EPAEdge3 edge ) {
			for( int i = 0; i < edges.Count; i++ ) {
				if( ( edges[ i ].a == edge.a && edges[ i ].b == edge.b ) || ( edges[ i ].a == edge.b && edges[ i ].b == edge.a ) )
					return i;
			}

			return -1;
		}

		private static EPAFace3 FindClosestFace( Hull3 simplex, Hull3Faces faces ) {
			EPAFace3 closest = new EPAFace3() { distance = float.PositiveInfinity };

			for( var i = 0; i < faces.Count; i++ ) {
				Hull3Face face = faces[ i ];

				Vector3 ab = simplex[ face.B ].Sum - simplex[ face.A ].Sum;
				Vector3 ac = simplex[ face.C ].Sum - simplex[ face.A ].Sum;
				Vector3 norm = Vector3.Cross( ab, ac ).Normalized;

				Vector3 a0 = -simplex[ face.A ].Sum;
				if( Vector3.Dot( a0, norm ) > 0 )
					norm = -norm;

				float dist = Vector3.Dot( simplex[ face.A ].Sum, norm );

				if( dist < closest.distance ) {
					closest = new EPAFace3() {
						faceId = i,
						distance = dist,
						normal = norm
					};
				}
			}

			return closest;
		}

	}
}
