using Civlike.World.Geometry;

namespace Civlike.World.State;

public sealed class Globe : StateContainerBase<Globe> {

	internal Globe( Guid globeId, ReadOnlyGlobe readonlyGlobe, double radius ) {
		this.Id = globeId;
		this.Model = readonlyGlobe;
		this.Nodes = this.Model.Vertices
			.Select( f => new Node( this, f ) )
			.ToList()
			.AsReadOnly();
		this.Tiles = this.Model.Faces
			.Select( f => new Tile( this, f, f.Vertices.Select( v => this.Nodes[ v.Id ] ) ) )
			.ToList()
			.AsReadOnly();
		this.Clusters = this.Model.Clusters
			.Select( c => new BoundedTileCluster( this, c, c.Faces.Select( f => this.Tiles[ f.Id ] ).ToList().AsReadOnly() ) )
			.ToList()
			.AsReadOnly();

		HashSet<Node> neighbouringNodes = [];
		foreach (Node n in Nodes) {
			neighbouringNodes.Clear();
			foreach (ReadOnlyFace f in n.Vertex.ConnectedFaces) 
				foreach (Node neighbour in f.Vertices.Where( v => v.Id != n.Vertex.Id ).Select( v => Nodes[ v.Id ] ))
					neighbouringNodes.Add( neighbour );
			n.NeighbouringNodes = neighbouringNodes.ToList().AsReadOnly();
		}

		this.Radius = radius;
		this.TileArea = this.Area / this.Tiles.Count;
		this.ApproximateTileLength = 2 * this.Radius * double.Sin( double.Acos( 1 / ((1 + double.Sqrt( 5 )) / 2) ) / double.Pow( 2, readonlyGlobe.Subdivisions ) * 0.5 );
	}

	public Guid Id { get; }
	public ReadOnlyGlobe Model { get; }
	public IReadOnlyList<Node> Nodes { get; }
	public IReadOnlyList<Tile> Tiles { get; }
	public IReadOnlyList<BoundedTileCluster> Clusters { get; }
	public double Radius { get; }
	public double Area => 4 * double.Pi * this.Radius * this.Radius;
	public double TileArea { get; }
	public double ApproximateTileLength { get; }

	public Tile GetTile( ReadOnlyFace face ) => this.Tiles[ face.Id ];
}
