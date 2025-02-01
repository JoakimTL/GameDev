namespace Sandbox.Logic.World.Tiles.Data;

public sealed class TileTerrainData : TileDataBase, ITileData {
	private const string _dataCode = "0TER";
	public static int CodeId { get; } = _dataCode.ToIntCode();

	/// <summary>
	/// 0 means the tile is completely flat. 255 means the tile is so rugged you can pierce yourself on the bumps.
	/// </summary>
	public byte Ruggedness { get; set; }

	/// <summary>
	/// 0 means the tile is completely flat. 255 means the tile is so steep you can't walk on it.
	/// </summary>
	public byte Slope { get; set; }

	/// <summary>
	/// The elevation above, or below, sea level in meters.
	/// </summary>
	public short Elevation { get; set; }
}

public sealed class TileRiverFlowData : TileDataBase, ITileData {
	private const string _dataCode = "0RIV";
	public static int CodeId { get; } = _dataCode.ToIntCode();

	//Rivers can be seen as stable. Although in real life they change course and flow, for the sake of not burning down any CPUs we'll consider them stable.
	//Rivers are only affected by human activity. This means the flow rate of rivers can change because of human activity. To simulate this we'll have the accumulation rate affect the negative flow rates. The system is stable when all the flow rates sum to 0.

	/// <summary>
	/// The rate of flow of water from neighbour index 0 in m^3/s
	/// </summary>
	private float _flowRate0;
	/// <summary>
	/// The rate of flow of water from neighbour index 1 in m^3/s
	/// </summary>
	private float _flowRate1;
	/// <summary>
	/// The rate of flow of water from neighbour index 2 in m^3/s
	/// </summary>
	private float _flowRate2;
	/// <summary>
	/// The rate of water accumulation in m^3/s. This rate affects the 
	/// </summary>
	private float _accumulationRate;

}
