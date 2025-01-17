using Engine.Standard.Render.Meshing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sandbox.Logic.World.Tiles2;
public interface INode {
	//TileRenderModel RenderModel { get; }
}

public abstract class NodeBase : INode {
	//public TileRenderModel RenderModel { get; }
}

public sealed class Province : NodeBase {

	public Region Container { get; }

}

public sealed class Region : NodeBase {

	public Cluster Container { get; }
	public IReadOnlyList<Province> ChildProvinces { get; }

}

public sealed class Cluster : NodeBase {

	public Cluster? Container { get; }
	public IReadOnlyList<INode> ChildNodes { get; }

}

public sealed class TileCluster {
	private readonly List<Tile> _clusteredTiles;

	public IReadOnlyList<Tile> ClusteredTiles => _clusteredTiles;

	public TileCluster( List<Tile> tiles ) {
		_clusteredTiles = tiles.ToList();
	}
}

public sealed class Tile : OcTree<Tile, float>.ILeaf {
	public Globe Globe { get; }
	public TriangleIndexes Indices { get; }

	public Tile( Globe globe, TriangleIndexes indices ) {
		this.Globe = globe;
		this.Indices = indices;
	}

	public Vector3<float> VectorA => Globe.Vertices[ Indices.A ];
	public Vector3<float> VectorB => Globe.Vertices[ Indices.B ];
	public Vector3<float> VectorC => Globe.Vertices[ Indices.C ];
	public AABB<Vector3<float>> Bounds => AABB.Create( [ VectorA, VectorB, VectorC ] );
}

public readonly struct TriangleIndexes( int a, int b, int c ) {
	public int A { get; } = a;
	public int B { get; } = b;
	public int C { get; } = c;
}

public sealed class Globe {

	//TODO maybe a light version of composite pattern, where "Tiles" have components, such that we don't need many versions of the "same" class just to have different tile types.
	public uint Layers { get; }

	public IReadOnlyList<Vector3<float>> Vertices { get; }
	private readonly OcTree<Tile, float> _tileTree;
	private readonly List<Tile> _allTiles;

	public Globe( uint layers = 9 ) {
		this.Layers = layers;

		_tileTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		Icosphere icosphere = new( layers );
		this.Vertices = icosphere.Vertices.ToList().AsReadOnly();

		GenerateMosaic( icosphere );
	}

	private void GenerateMosaic( Icosphere icosphere ) {
		//Generate world tiles.
		//Here we can try to reduce how many tiles we have. Oceans for example might not need as much detail as land.

		var indices = icosphere.GetIndices( icosphere.Subdivisions - 1 );

		for (int i = 0; i < indices.Count; i += 3) {
			TriangleIndexes triangle = new( (int) indices[ i ], (int) indices[ i + 1 ], (int) indices[ i + 2 ] );
			Tile tile = new( this, triangle );
			_tileTree.Add( tile );
			_allTiles.Add( tile );
		}

		//Display octree branches as meshes. Only have them active when the camera is on the right side of the globe. Meaning when the center of the bounds dotted with the camera translation (globe is a origin) is positive.

	}

}