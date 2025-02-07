namespace Sandbox.Logic.World.Tiles.Data;

public sealed class TilePopulationData : TileDataBase, ITileData {
	private const string _dataCode = "0POP";
	public static int CodeId { get; } = _dataCode.ToIntCode();
	/// <summary>
	/// The current population of the tile.
	/// </summary>
	public ulong Population { get; private set; }

}