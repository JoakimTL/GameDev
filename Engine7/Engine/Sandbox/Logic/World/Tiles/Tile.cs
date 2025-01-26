using Sandbox.Logic.OldWorld.Tiles.Generation;
using Sandbox.Logic.Research.Technologies;
using Sandbox.Logic.World.Tiles.Terrain;

namespace Sandbox.Logic.World.Tiles;

public sealed class Tile : IOcTreeLeaf<float> {
	public GlobeComponent Globe { get; }
	public readonly TileRenderModel RenderModel;
	private readonly TileNeighboursModel _neighbours;

	public Tile( GlobeComponent globe, TriangleIndices indices, Vector4<float> color ) {
		this.Globe = globe;
		this.RenderModel = new( this, indices, color );
		this._neighbours = new( this );
		Area = RenderModel.Area * Globe.GeneratedSurfaceArea / (4 * double.Pi);
	}

	public IReadOnlyList<Edge> Edges => _neighbours.Edges;
	public IReadOnlyList<Tile> Neighbours => _neighbours.GetNeighbours();
	public AABB<Vector3<float>> Bounds => RenderModel.GetBounds();
	public double Area { get; }

	internal void AddEdge( Edge edge ) {
		_neighbours.AddEdge( edge );
	}

	internal void SetTerrain( TerrainBase type ) {

	}
}



public sealed class TileDataModel {

}

public abstract class TileDataBase {
	public abstract int CodeId { get; }
	public Tile Tile { get; private set; }

	protected TileDataBase() {
		Tile = null!;
	}

	internal void SetTile(Tile tile) {
		if (Tile is not null)
			throw new InvalidOperationException( "Tile already set." );
		Tile = tile;
	}
}

public sealed class TileTerrainData : TileDataBase {
	private const string _dataCode = "000T";
	private static readonly int _dataCodeId = _dataCode.ToIntCode();

	public override int CodeId => _dataCodeId;

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

public sealed class TileResourceData : TileDataBase {
	private const string _dataCode = "000R";
	private static readonly int _dataCodeId = _dataCode.ToIntCode();
	public override int CodeId => _dataCodeId;

	/// <summary>
	/// The current quantity of resources on the tile.
	/// </summary>
	private Dictionary<int, float> _resourceQuantity = [];
	/// <summary>
	/// The rate at which resources are renewed on the tile. (Think inflow of water from a river, or the growth of plants.)
	/// </summary>
	private Dictionary<int, float> _resourceRenewal = [];
	/// <summary>
	/// The capacity of the tile to hold resources. If there is no entry then the capacity is "infinite".
	/// </summary>
	private Dictionary<int, float> _resourceMaxQuantity = [];

	public void Set( int resourceId, float abundance ) => _resourceAbundance[ resourceId ] = abundance;

}

public static class ResourceList {

	private static readonly Dictionary<int, ResourceBase> _resources;
	private static readonly Dictionary<string, ResourceBase> _resourcesByResourceIdentifier;
	private static readonly Dictionary<string, IReadOnlyList<ResourceBase>> _resourcesByTag;

	static ResourceList() {
		_resources = [];
		_resourcesByResourceIdentifier = [];

		IEnumerable<Type> resourceTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.BaseType == typeof( TerrainBase ) );

		foreach (Type resourceType in resourceTypes) {
			ResourceBase instance = resourceType.CreateInstance( null ) as ResourceBase ?? throw new InvalidOperationException( "Failed to construct terrain instance. Must have a parameterless constructor." );
			if (instance is IInitializable initializable)
				initializable.Initialize();
			if (_resources.TryGetValue( instance.Id, out ResourceBase? existing ))
				throw new InvalidOperationException( $"Resource type with id {instance.Id} already exists: {existing.ResourceIdentifier}." );
			_resources.Add( instance.Id, instance );
			if (_resourcesByResourceIdentifier.TryGetValue( instance.ResourceIdentifier, out ResourceBase? existingByResourceId ))
				throw new InvalidOperationException( $"Resource type with resource identifier {instance.ResourceIdentifier} already exists: {existingByResourceId.ResourceIdentifier}." );
			_resourcesByResourceIdentifier.Add( instance.ResourceIdentifier, instance );
		}

		_resourcesByTag = _resources.Values.SelectMany( p => p.Tags.Select( t => (Tag: t, Resource: p) ) ).GroupBy(p => p.Tag).ToDictionary(p => p.Key, p => p.Select(p => p.Resource).ToList().AsReadOnly());
	}

	public static ResourceBase GetResource( int id ) {
		if (!_resources.TryGetValue( id, out ResourceBase? resource ))
			throw new InvalidOperationException( $"Resource with id {id} not found." );
		return resource;
	}

	public static ResourceBase GetResource( string resourceIdentifier ) {
		if (!_resourcesByResourceIdentifier.TryGetValue( resourceIdentifier, out ResourceBase? resource ))
			throw new InvalidOperationException( $"Resource with resource identifier {resourceIdentifier} not found." );
		return resource;
	}

	public static IReadOnlyList<ResourceBase> GetResourcesByTag( string tag ) {
		if (!_resourcesByTag.TryGetValue( tag, out IReadOnlyList<ResourceBase>? resources ))
			throw new InvalidOperationException( $"No resources found with tag {tag}." );
		return resources;
	}
}

public static class ResourceTags {
	public const string ORE = "ORE";
	public const string METAL = "METAL";
	public const string IRON = "IRON";
	public const string WATER = "WATER";
}

public abstract class ResourceBase {
	public int Id { get; }
	public string ResourceIdentifier { get; }
	public IReadOnlySet<string> Tags { get; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="id">Internal numerical id</param>
	/// <param name="resourceIdentifier">Internal text identifier, used to get the resource instance from the <see cref="ResourceList"/></param>
	protected ResourceBase( int id, string resourceIdentifier, params IEnumerable<string> tags ) {
		Id = id;
		ResourceIdentifier = resourceIdentifier;
		Tags = tags.ToHashSet();
	}

	public abstract (string Name, string Description) GetDisplayName( PlayerComponent player );
}

public sealed class IronOre() : ResourceBase( 0x0001_0000, "IRON_ORE", ResourceTags.ORE, ResourceTags.IRON) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<Metallurgy>())
			return ("Iron Ore", "High-grade hematite ore, containing 70% iron, ideal for steelmaking.");
		if (player.HasTechnology<SteelWorking>())
			return ("Iron Ore", "High-grade hematite ore, ideal for steelmaking.");
		if (player.HasTechnology<IronWorking>())
			return ("Iron Ore", "Rocks containing veins of a metallic substance that can be smelted into tools.");
		return ("Rock", "A heavy, rust-colored stone that stains your hands red when handled.");
	}
}
public sealed class IronMetal() : ResourceBase( 0x0001_0001, "IRON_METAL", ResourceTags.METAL, ResourceTags.IRON ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<SteelWorking>())
			return ("Iron", "A shiny gray metal prone to rusting. Alloyed with carbon to make steel.");
		if (player.HasTechnology<IronWorking>() && player.HasTechnology<BronzeWorking>())
			return ("Iron", "A shiny gray metal prone to rusting, with some work hardening it can be as hard as bronze.");
		if (player.HasTechnology<IronWorking>())
			return ("Iron", "A shiny gray metal prone to rusting, with some work hardening it can made into useful tools.");
		return ("Gray Metal", "A shiny gray metal prone to rusting and too malleable to be useful.");
	}
}
public sealed class FreshWater() : ResourceBase( 0x0001_0002, "FRESH_WATER", ResourceTags.WATER ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<Micobiology>())
			return ("Fresh Water", "While generally safe for basic uses, this water may still harbor unseen contaminants.");
		return ("Water", "The source of life.");
	}
}
public sealed class DrinkingWater() : ResourceBase( 0x0001_0003, "DRINKING_WATER", ResourceTags.WATER ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<Micobiology>())
			return ("Drinking Water", "Water treated to ensure it is safe and free of harmful microbes.");
		return ("Water", "The source of life.");
	}
}
public sealed class GroundWater() : ResourceBase( 0x0001_0004, "GROUND_WATER", ResourceTags.WATER ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<Micobiology>())
			return ("Groundwater", "While generally safe for basic uses, this water may still harbor unseen contaminants.");
		return ("Water", "The source of life.");
	}
}
public sealed class SeaWater() : ResourceBase( 0x0001_0005, "SEA_WATER", ResourceTags.WATER ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<Micobiology>())
			return ("Drinking Water", "Water treated to ensure it is safe and free of harmful microbes.");
		return ("Sea Water", "The source of life spoiled by .");
	}
}
public sealed class SewageWater() : ResourceBase( 0x0001_0006, "SEWAGE_WATER", ResourceTags.WATER ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		if (player.HasTechnology<Micobiology>())
			return ("Sewage", "Must be treated before safe consumption.");
		return ("Dirty Water", "The source of life spoiled by dirty materials.");
	}
}
public sealed class HolyWater() : ResourceBase( 0x0001_0007, "HOLY_WATER", ResourceTags.WATER ) {
	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
		return ("Holy Water", $"Water imbued with the gift of {(player.Religions.MainReligion.Polytheistic ? "the gods" : "God")}.");
	}
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
