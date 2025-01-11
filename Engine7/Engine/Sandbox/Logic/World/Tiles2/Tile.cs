using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sandbox.Logic.World.Tiles2;
public interface INode {
	TileRenderModel RenderModel { get; }
}

public abstract class NodeBase : INode {
	public TileRenderModel RenderModel { get; }
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

public sealed class TileRenderModel {

}