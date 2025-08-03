namespace Civlike.World;

public sealed class Globe {

	internal Globe( Guid globeId, ReadOnlyGlobe readonlyGlobe, double radius ) {
		this.Id = globeId;
		this.Model = readonlyGlobe;
		this.Tiles = this.Model.Faces.Select( f => new Tile( this, f ) ).ToList().AsReadOnly();

		this.Radius = radius;
		this.TileArea = this.Area / this.Tiles.Count;
		this.ApproximateTileLength = 2 * this.Radius * double.Sin( double.Acos( 1 / ((1 + double.Sqrt( 5 )) / 2) ) / double.Pow( 2, readonlyGlobe.Subdivisions ) * 0.5 );
	}

	public Guid Id { get; }
	public ReadOnlyGlobe Model { get; }
	public IReadOnlyList<Tile> Tiles { get; }
	public double Radius { get; }
	public double Area => 4 * double.Pi * this.Radius * this.Radius;
	public double TileArea { get; }
	public double ApproximateTileLength { get; }

	public Tile GetTile( ReadOnlyFace face ) => this.Tiles[ face.Id ];
}
