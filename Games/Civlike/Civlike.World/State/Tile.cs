using Civlike.World.Geometry;

namespace Civlike.World.State;

public sealed class Tile : StateContainerBase<Tile> {

	internal Tile( Globe globe, ReadOnlyFace face, IEnumerable<Node> nodes ) {
		this.Globe = globe;
		this.Face = face;
		this.Nodes = nodes.ToList().AsReadOnly();
	}

	public Globe Globe { get; }
	public ReadOnlyFace Face { get; }
	public IReadOnlyList<Node> Nodes { get; }

	public int Id => this.Face.Id;
	public IEnumerable<Tile> Neighbours => this.Face.Neighbours.Select( n => this.Globe.Tiles[ n.Id ] );

}
