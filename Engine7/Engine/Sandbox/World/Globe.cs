namespace Sandbox.World;
public sealed class Globe {
}

public sealed class Tile {
	//public Globe Globe { get; }
	//public TileRenderModel RenderModel { get; }
	//public TileDataModel DataModel { get; }
	//public PopulationCenter? PopulationCenter { get; set; }
	//public PlayerComponent? Owner { get; set; }
	//private readonly TileNeighboursModel _neighbours;
	//public event Action<Tile>? RenderingChanged;
	//public Tile( Globe globe, TriangleIndices indices, Vector4<float> color ) {
	//	this.Globe = globe;
	//	this.RenderModel = new( this, indices, color );
	//	this._neighbours = new( this );
	//	this.DataModel = new( this );
	//	Area = RenderModel.Area * Globe.GeneratedSurfaceArea / (4 * double.Pi);
	//}
}

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.