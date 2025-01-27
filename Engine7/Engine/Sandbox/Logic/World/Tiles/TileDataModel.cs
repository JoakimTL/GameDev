using System.Reflection;

namespace Sandbox.Logic.World.Tiles;

public sealed class TileDataModel {
	private static readonly List<(int, ResolvedType)> _tileDataTypes;

	static TileDataModel() {
		var tileDataTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.BaseType == typeof( TileDataBase ) && p.IsAssignableTo( typeof( ITileData ) ) ).Select( TypeManager.ResolveType ).ToList();

		_tileDataTypes = [];
		HashSet<int> codeIds = [];
		foreach (ResolvedType tileDataType in tileDataTypes) {
			int codeId = tileDataType.GetPropertyAccessor( BindingFlags.Static | BindingFlags.Public, "CodeId" ).ReadProperty<int>( null );
			if (!codeIds.Add( codeId ))
				throw new InvalidOperationException( $"Tile data type with code id {codeId} already exists." );
			_tileDataTypes.Add( (codeId, tileDataType) );
		}
	}

	private readonly Dictionary<int, TileDataBase> _tileData;

	public TileDataModel( Tile tile ) {
		_tileData = [];
		foreach ((int codeId, ResolvedType tileDataType) in _tileDataTypes) {
			TileDataBase tileData = tileDataType.CreateInstance( null ) as TileDataBase ?? throw new InvalidOperationException();
			tileData.SetDataModel( this );
			_tileData.Add( codeId, tileData );
		}
	}

	public T GetData<T>() where T : TileDataBase, ITileData
		=> _tileData[ T.CodeId ] as T ?? throw new InvalidOperationException();

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
