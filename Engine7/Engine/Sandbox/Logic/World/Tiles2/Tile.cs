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

public sealed class GlobeTile {

	public Globe Globe { get; }

	public int IndexA { get; }
	public int IndexB { get; }
	public int IndexC { get; }

	public Vector3<float> VectorA => Globe.Vertices[ IndexA ];
	public Vector3<float> VectorB => Globe.Vertices[ IndexB ];
	public Vector3<float> VectorC => Globe.Vertices[ IndexC ];

	public GlobeTile(Globe globe, int indexA, int indexB, int indexC ) {
		this.Globe = globe;
		this.IndexA = indexA;
		this.IndexB = indexB;
		this.IndexC = indexC;
	}
}

public sealed class Globe {

	//TODO maybe a light version of composite pattern, where "Tiles" have components, such that we don't need many versions of the "same" class just to have different tile types.
	public uint Layers { get; }
	public int RootLayer { get; }

	public IReadOnlyList<Vector3<float>> Vertices { get; }
	private List<GlobeTile> _rootTiles;

	public Globe(uint layers = 8, int rootLayer = 4) {
		this.Layers = layers;
		this.RootLayer = rootLayer;

		Icosphere icosphere = new( layers, rootLayer );
		this.Vertices = icosphere.Vertices.ToList().AsReadOnly();

		GenerateMosaic(icosphere);
	}

	private void GenerateMosaic( Icosphere icosphere ) {

	}

}