namespace Sandbox.Logic.OldWorld.Tiles.Generation;

public sealed class TerrainType {

	private static readonly List<TerrainType> _terrainTypes;
	public static readonly TerrainType Water;
	public static readonly TerrainType ShallowWater;
	public static readonly TerrainType Grass;
	public static readonly TerrainType Sand;
	public static readonly TerrainType Rock;
	public static readonly TerrainType Forest;

	static TerrainType() {
		_terrainTypes = [];
		Water = new( new( 0, 0, 1 ), true );
		ShallowWater = new( new( .33f, .33f, 1 ), true );
		Grass = new( new( 0, 1, 0 ), false );
		Sand = new( new( 1, 1, 0 ), false );
		Rock = new( new( .5f, .5f, .5f ), false );
		Forest = new( new( 0, .25f, 0 ), false );
	}

	public static void Initialize() {
		TerrainAdjacencyRules.Initialize();
		TerrainAdjacencyRules.SetAdjacencyWeight( Water.Id, Rock.Id, 100 );
		TerrainAdjacencyRules.SetAdjacencyWeight( ShallowWater.Id, Sand.Id, 95 );
		TerrainAdjacencyRules.SetAdjacencyWeight( ShallowWater.Id, Rock.Id, 5 );

		TerrainAdjacencyRules.SetAdjacencyWeight( Grass.Id, Grass.Id, 100 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Grass.Id, Sand.Id, 6 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Grass.Id, Rock.Id, 3 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Grass.Id, Forest.Id, 16 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Sand.Id, Sand.Id, 10 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Sand.Id, Rock.Id, 2 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Rock.Id, Rock.Id, 100 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Rock.Id, Forest.Id, 6 );
		TerrainAdjacencyRules.SetAdjacencyWeight( Forest.Id, Forest.Id, 40 );
	}

	public static TerrainType Get( int id ) {
		return _terrainTypes[ id ];
	}

	public static IReadOnlyList<TerrainType> AllTerrainTypes => _terrainTypes;
	public static IEnumerable<TerrainType> AllLandTerrainTypes => _terrainTypes.Where( p => !p.IsWater );

	public int Id { get; }
	public Vector4<float> Color { get; }
	public bool IsWater { get; }

	public TerrainType( Vector3<float> color, bool isWater ) {
		this.Id = _terrainTypes.Count;
		_terrainTypes.Add( this );
		this.Color = new( color.X, color.Y, color.Z, 1 );
		this.IsWater = isWater;
	}

}
