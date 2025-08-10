using Civlike.World.Geometry;

namespace Civlike.World.State;

public sealed class Node : StateContainerBase<Node> {

	public Node( Globe globe, ReadOnlyVertex vertex ) {
		this.Globe = globe;
		this.Vertex = vertex;
	}

	public Globe Globe { get; }
	public ReadOnlyVertex Vertex { get; }
	public IReadOnlyList<Node> NeighbouringNodes { get; internal set; }
}