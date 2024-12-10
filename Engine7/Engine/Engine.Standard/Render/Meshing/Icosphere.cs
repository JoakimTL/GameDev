using Engine.Module.Render.Ogl.Scenes;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Standard.Render.Meshing;
public sealed class IcosphereService( MeshService meshService ) {
	private readonly MeshService _meshService = meshService;

	//public VertexMesh<Vertex3> GenerateComplete( int subdivisions ) {

	//	GenerateIcosphereVectors( subdivisions, out List<Vector3<float>> vectors, out List<uint> indices );

	//	List<Vertex3> vertices = new();

	//	for (int i = 0; i < vectors.Count; i++) {
	//		Vertex3 v = new( vectors[ i ], 0, vectors[ i ], 255 );
	//		vertices.Add( v );
	//	}

	//	_meshService.CreateMesh( vertices.ToArray(), indices.ToArray() );
	//}

	//Project centers of triangles to polar coordinates. Use subdivision 2 or 3 here, as these will be the baseline for the icosphere anyways.
	//Place them in an octree.
	//When trying to figure out which triangles should be most subdivided use a projection of the camera view to the octree. This means take the coordinates of the camera, and see where the projection edges (near to far each corner) intersects the sphere (line segment - sphere intersection (and find the intersection closest to the camera per edge)).
	//Now travel the octree to find which triangles should be rendered at what LOD. (not sure how to do this yet)
	//When hovering over a tile, the tile should bounce out and have an outline under to prevent holes in the earth
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
			0, 1, 3,
			0, 2, 3,
			1, 2, 3,
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

