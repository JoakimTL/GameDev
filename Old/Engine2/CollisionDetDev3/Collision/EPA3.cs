using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollisionDetDev3.Collision {
	public class EPA3 {

		public const float EPSILON = 0.00006103515f; //2^-14

		public struct EPAFace3 {
			public float distance;
			public Vector3 normal;
			public int faceId;
		}

		public struct EPAEdge3 {
			public int a, b;
		}

		public static int PerformStep( Simplex3 simplex, ref Simplex3Faces faces, PhysicsShape<Transform3, Vector3> shape1, PhysicsShape<Transform3, Vector3> shape2, int it, out Collision3Result res, out Simplex3Faces his2, bool log ) {
			if( log )
				Console.WriteLine( "EPA it: " + it );

			res = null;
			his2 = null;
			if( simplex.Count < 4 ) {
				if( simplex.Count == 2 ) {
					Vector3 ab = ( simplex[ 1 ].Sum - simplex[ 0 ].Sum ).Normalized;
					Vector3 a0 = -simplex[ 0 ].Sum.Normalized;
					if( Vector3.Dot( ab, a0 ) >= 1 - EPSILON ) {
						res = new Collision3Result( it, ab, simplex[ 0 ].Sum.Length, simplex[ 0 ].A, simplex[ 0 ].B );
						return 1;
					}
				}
				for( int i = 0; i < simplex.Count; i++ ) {
					if( simplex[ i ].Sum == 0 ) {
						res = new Collision3Result( it, 0, 0, 0, 0 );
						return 1;
					}
				}
				FillSimplex( simplex, shape1, shape2 );
				return -1;
			}

			if( faces is null )
				faces = new Simplex3Faces( simplex );

			EPAFace3 face = FindClosestFace( simplex, faces );
			Support3 newSupp = simplex.GetSupport( shape1, shape2, face.normal, out _ );
			float dist = Vector3.Dot( newSupp.Sum, face.normal );

			if( dist - face.distance < EPSILON ) {
				if( log )
					Console.WriteLine( "wow: " + face.normal + " .. " + face.distance );
				//ret = { axis: face.norm, dist: dist};
				Face3 closestFace = faces[ face.faceId ];
				Vector3 bary = Vector3.ToBartcentricProjectedTriangle( simplex[ closestFace.A ].Sum, simplex[ closestFace.B ].Sum, simplex[ closestFace.C ].Sum, 0 );
				if( log )
					Console.WriteLine(bary);
				Vector3 closestA = simplex[ closestFace.A ].A * bary.X + simplex[ closestFace.B ].A * bary.Y + simplex[ closestFace.C ].A * bary.Z;
				Vector3 closestB = simplex[ closestFace.A ].B * bary.X + simplex[ closestFace.B ].B * bary.Y + simplex[ closestFace.C ].B * bary.Z;
				res = new Collision3Result( it, face.normal, face.distance, closestB, closestA );
				if( !( faces is null ) )
					his2 = new Simplex3Faces( faces );
				return 1;
			}

			if( log )
				Console.WriteLine( newSupp );

			simplex.AddSupport( newSupp );
			Reconstruct( simplex, faces, newSupp, log );
			if( !( faces is null ) )
				his2 = new Simplex3Faces( faces );
			return 0;
		}

		private static void FillSimplex( Simplex3 simplex, PhysicsShape<Transform3, Vector3> shape1, PhysicsShape<Transform3, Vector3> shape2 ) {
			switch( simplex.Count ) {
				case 1: {
						simplex.SearchDirection = -simplex[ 0 ].Sum;
						simplex.SearchOrigin = simplex[ 0 ].Sum;
						break;
					}
				case 2: {
						Vector3 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
						Vector3 a0 = -simplex[ 0 ].Sum + (1, 1, 1);
						simplex.SearchDirection = Vector3.Cross( Vector3.Cross( ab, a0 ), ab ); // From the line towards the origin.
						simplex.SearchOrigin = ( simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 2;
						break;
					}
				case 3: {
						Vector3 a0 = -simplex[ 0 ].Sum;
						Vector3 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
						Vector3 ac = simplex[ 2 ].Sum - simplex[ 0 ].Sum;
						Vector3 abc = Vector3.Cross( ab, ac );
						if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
							simplex.SearchDirection = abc;
							simplex.SearchOrigin = ( simplex[ 2 ].Sum + simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 3;
						} else {
							simplex.SearchDirection = -abc;
							simplex.SearchOrigin = ( simplex[ 2 ].Sum + simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 3;
							simplex.Switch( 1, 2 );
						}
						break;
					}
				case 4: {
						Vector3 a0 = -simplex[ 0 ].Sum;
						Vector3 ab = simplex[ 1 ].Sum - simplex[ 0 ].Sum;
						Vector3 ac = simplex[ 2 ].Sum - simplex[ 0 ].Sum;
						Vector3 ad = simplex[ 3 ].Sum - simplex[ 0 ].Sum;
						Vector3 abc = Vector3.Cross( ab, ac );
						Vector3 acd = Vector3.Cross( ac, ad );
						Vector3 adb = Vector3.Cross( ad, ab );
						if( Vector3.Dot( abc, a0 ) > -EPSILON ) {
							simplex.SearchDirection = abc;
							simplex.SearchOrigin = ( simplex[ 2 ].Sum + simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 3;
							break;
						}
						if( Vector3.Dot( acd, a0 ) > -EPSILON ) {
							simplex.SearchDirection = acd;
							simplex.SearchOrigin = ( simplex[ 3 ].Sum + simplex[ 2 ].Sum + simplex[ 0 ].Sum ) / 3;
							break;
						}
						if( Vector3.Dot( adb, a0 ) > -EPSILON ) {
							simplex.SearchDirection = adb;
							simplex.SearchOrigin = ( simplex[ 3 ].Sum + simplex[ 1 ].Sum + simplex[ 0 ].Sum ) / 3;
							break;
						}

						Vector3 b0 = -simplex[ 1 ].Sum;
						Vector3 bc = simplex[ 2 ].Sum - simplex[ 1 ].Sum;
						Vector3 bd = simplex[ 3 ].Sum - simplex[ 1 ].Sum;
						Vector3 bcd = Vector3.Cross( bc, bd );
						if( Vector3.Dot( bcd, b0 ) > -EPSILON ) {
							simplex.SearchDirection = bcd;
							simplex.SearchOrigin = ( simplex[ 3 ].Sum + simplex[ 2 ].Sum + simplex[ 1 ].Sum ) / 3;
							break;
						}
						simplex.SearchDirection = -bcd;
						simplex.SearchOrigin = ( simplex[ 3 ].Sum + simplex[ 2 ].Sum + simplex[ 1 ].Sum ) / 3;
						break;
					}
			}
			Support3 s = simplex.GetSupport( shape1, shape2, simplex.SearchDirection, out _ );
			simplex.InsertSupport( 0, s );
		}

		private static void Reconstruct( Simplex3 simplex, Simplex3Faces faces, Support3 newSupp, bool log ) {
			List<int> facesToRemove = new List<int>();

			for( int i = 0; i < faces.Count; i++ ) {
				Face3 face = faces[ i ];

				Vector3 ab = simplex[ face.B ].Sum - simplex[ face.A ].Sum;
				Vector3 ac = simplex[ face.C ].Sum - simplex[ face.A ].Sum;
				Vector3 norm = Vector3.Cross( ab, ac ).Normalized;

				if( log )
					Console.WriteLine( face.A + "/" + face.B + "/" + face.C );
				if( log )
					Console.WriteLine( norm + ", " + ( newSupp.Sum - simplex[ face.A ].Sum ) + ", " + Vector3.Dot( norm, newSupp.Sum - simplex[ face.A ].Sum ) );
				if( Vector3.Dot( norm, newSupp.Sum - simplex[ face.A ].Sum ) > 0 )
					facesToRemove.Add( i );
			}

			List<EPAEdge3> edges = new List<EPAEdge3>();
			for( int i = 0; i < facesToRemove.Count; i++ ) {
				var face = faces[ facesToRemove[ i ] ];
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

			if( log )
				Console.WriteLine( facesToRemove.Count + ", " + faces.Count + ", " + edges.Count );

			for( var i = 0; i < edges.Count; i++ ) {
				faces.AddFace( new Face3( edges[ i ].a, edges[ i ].b, simplex.Count - 1 ) );
			}

			/*
			 * //I do realize that this function can be done more efficietly
		var removalFaces = [];
		for(var i = 0; i < simplexFaces.length; i++) {
		var face = simplexFaces[i];

		var ab = simplex[face.b].clone().sub(simplex[face.a]);
		var ac = simplex[face.c].clone().sub(simplex[face.a]);
		var norm = ab.cross(ac).normalize();

		var a0 = new THREE.Vector3().sub(simplex[face.a]);
		if(a0.dot(norm) > 0)
			norm.negate();

		if(norm.clone().dot(extendPoint.clone().sub(simplex[face.a])) > 0)
			removalFaces.push(i);
		}

		//get the edges that are not shared between the faces that should be removed
		var edges = [];
		for(var i = 0; i < removalFaces.length; i++) {
		var face = simplexFaces[removalFaces[i]];
		var edgeAB = {a: face.a, b: face.b};
		var edgeAC = {a: face.a, b: face.c};
		var edgeBC = {a: face.b, b: face.c};

		var k = edgeInEdges(edges, edgeAB);
		if(k != -1)
			edges.splice(k, 1);
		else
			edges.push(edgeAB);

		k = edgeInEdges(edges, edgeAC);
		if(k != -1)
			edges.splice(k, 1);
		else
			edges.push(edgeAC);

		k = edgeInEdges(edges, edgeBC);
		if(k != -1)
			edges.splice(k, 1);
		else
			edges.push(edgeBC);
		}

		//remove the faces from the polytope
		for(var i = removalFaces.length - 1; i >= 0; i--) {
		simplexFaces.splice(removalFaces[i], 1);
		}

		//form new faces with the edges and new point
		for(var i = 0; i < edges.length; i++) {
		simplexFaces.push({a: edges[i].a, b: edges[i].b, c: simplex.length - 1});
		}
			 */
		}
		private static int EdgeInEdges( List<EPAEdge3> edges, EPAEdge3 edge ) {
			for( int i = 0; i < edges.Count; i++ ) {
				if( ( edges[ i ].a == edge.a && edges[ i ].b == edge.b ) || ( edges[ i ].a == edge.b && edges[ i ].b == edge.a ) )
					return i;
			}

			return -1;
		}

		private static EPAFace3 FindClosestFace( Simplex3 simplex, Simplex3Faces faces ) {
			EPAFace3 closest = new EPAFace3() { distance = float.PositiveInfinity };

			for( var i = 0; i < faces.Count; i++ ) {
				Face3 face = faces[ i ];

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

		/*
		 * var EPA = function(aWorldVerts, bWorldVerts, simplex) {
    var simplexFaces = [{a: 0, b: 1, c: 2},
                        {a: 0, b: 1, c: 3},
                        {a: 0, b: 2, c: 3},
                        {a: 1, b: 2, c: 3}];

    var ret = null;

    while(true) {
        var face = findClosestFace(simplex, simplexFaces);
        var point = support(aWorldVerts, bWorldVerts, face.norm);
        var dist = point.clone().dot(face.norm);

        if(dist - face.dist < 0.00001) {
            ret = {axis: face.norm, dist: dist};
            break;
        }

        simplex.push(point);
        reconstruct(simplex, simplexFaces, point);
    }

    return ret;
}

var reconstruct = function(simplex, simplexFaces, extendPoint) {
    //I do realize that this function can be done more efficietly
    var removalFaces = [];
    for(var i = 0; i < simplexFaces.length; i++) {
        var face = simplexFaces[i];

        var ab = simplex[face.b].clone().sub(simplex[face.a]);
        var ac = simplex[face.c].clone().sub(simplex[face.a]);
        var norm = ab.cross(ac).normalize();

        var a0 = new THREE.Vector3().sub(simplex[face.a]);
        if(a0.dot(norm) > 0)
            norm.negate();

        if(extendPoint.clone().dot(norm) > 0) {
            removalFaces.push(i);
        }
    }

    //get the edges that are not shared between the faces that should be removed
    var edges = [];
    for(var i = 0; i < removalFaces.length; i++) {
        var face = simplexFaces[removalFaces[i]];
        var edgeAB = {a: face.a, b: face.b};
        var edgeAC = {a: face.a, b: face.c};
        var edgeBC = {a: face.b, b: face.c};

        var k = edgeInEdges(edges, edgeAB);
        if(k != -1)
            edges.splice(k, 1);
        else
            edges.push(edgeAB);

        k = edgeInEdges(edges, edgeAC);
        if(k != -1)
            edges.splice(k, 1);
        else
            edges.push(edgeAC);

        k = edgeInEdges(edges, edgeBC);
        if(k != -1)
            edges.splice(k, 1);
        else
            edges.push(edgeBC);
    }

    //remove the faces from the polytope
    for(var i = removalFaces.length - 1; i >= 0; i--) {
        simplexFaces.splice(removalFaces[i], 1);
    }

    //form new faces with the edges and new point
    for(var i = 0; i < edges.length; i++) {
        simplexFaces.push({a: edges[i].a, b: edges[i].b, c: simplex.length - 1});
    }
}

var edgeInEdges = function(edges, edge) {
    for(var i = 0; i < edges.length; i++) {
        if(edges[i].a == edge.a && edges[i].b == edge.b)
            return i;
    }

    return -1;
}

var findClosestFace = function(simplex, simplexFaces) {
    var closest = {dist: Infinity};

    for(var i = 0; i < simplexFaces.length; i++) {
        var face = simplexFaces[i];

        var ab = simplex[face.b].clone().sub(simplex[face.a]);
        var ac = simplex[face.c].clone().sub(simplex[face.a]);
        var norm = ab.cross(ac).normalize();

        var a0 = new THREE.Vector3().sub(simplex[face.a]);
        if(a0.dot(norm) > 0)
            norm.negate();

        var dist = simplex[face.a].clone().dot(norm);

        if(dist < closest.dist) {
            closest = {index: i, dist: dist, norm: norm, a: face.a, b: face.b, c: face.c};
        }
    }

    return closest;
}

var support = function(aVerts, bVerts, dir) {
    a = getFurthestPointInDirection(aVerts, dir);
    b = getFurthestPointInDirection(bVerts, dir.clone().negate());
    return a.clone().sub(b);
}

var getFurthestPointInDirection = function(verts, dir) {
    var index = 0;
    var maxDot = verts[index].clone().dot(dir.clone().normalize());

    for(var i = 1; i < verts.length; i++) {
        var dot = verts[i].clone().dot(dir.clone().normalize());

        if(dot > maxDot) {
            maxDot = dot;
            index = i;
        }
    }

    return verts[index];
}
		 */
	}
}
