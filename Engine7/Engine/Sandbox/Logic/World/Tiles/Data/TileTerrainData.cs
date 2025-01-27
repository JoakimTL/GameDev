namespace Sandbox.Logic.World.Tiles.Data;

public sealed class TileTerrainData : TileDataBase, ITileData {
	private const string _dataCode = "000T";
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
	/// The height above, or below, sea level.
	/// </summary>
	public short Height { get; set; }
}

//public sealed  class IronMetal() : ResourceBase( 0x0001_0001, "IRON_METAL", "Iron Metal", "Refined iron metal" );
//public sealed class Steel() : ResourceBase( 0x0001_0002, "STEEL", "Steel", "Iron alloyed with carbon" );
//public sealed class Coal() : ResourceBase( 0x0001_0003, "COAL", "Coal", "Carbon-rich rock" );
//public sealed class CopperOre() : ResourceBase( 0x0001_0004, "COPPER_ORE", "Copper Ore", "Rock containing high amounts of copper metal" );
//public sealed class CopperMetal() : ResourceBase( 0x0001_0005, "COPPER_METAL", "Copper Metal", "Refined copper metal" );
//public sealed class TinOre() : ResourceBase( 0x0001_0006, "TIN_ORE", "Tin Ore", "Rock containing high amounts of tin metal" );
//public sealed class TinMetal() : ResourceBase( 0x0001_0007, "TIN_METAL", "Tin Metal", "Refined tin metal" );
//public sealed class Bronze() : ResourceBase( 0x0001_0008, "BRONZE", "Bronze", "Copper alloyed with tin" );
//public sealed class SilverOre() : ResourceBase( 0x0001_0009, "SILVER_ORE", "Silver Ore", "Rock containing high amounts of silver metal" );
//public sealed class SilverMetal() : ResourceBase( 0x0001_000A, "SILVER_METAL", "Silver Metal", "Refined silver metal" );
//public sealed class GoldOre() : ResourceBase( 0x0001_000B, "GOLD_ORE", "Gold Ore", "Rock containing high amounts of gold metal" );
//public sealed class GoldMetal() : ResourceBase( 0x0001_000C, "GOLD_METAL", "Gold Metal", "Refined gold metal" );
