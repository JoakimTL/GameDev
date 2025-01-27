using Sandbox.Logic;
using Sandbox.Logic.Research.Technologies;
using Sandbox.Logic.World.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Render.Ui.ResourceDisplay;
public abstract class ResourceDisplayBase {

	protected ResourceDisplayBase( int resourceId ) {
		this.ResourceId = resourceId;
	}

	public int ResourceId { get; }

	public abstract (string TexturePath, string Name, string Description) GetDisplayInfo(PlayerComponent player);
}

public sealed class IronOreDisplay : ResourceDisplayBase {
	public IronOreDisplay() : base( 0x0001_0000 ) {	}

	public override (string TexturePath, string Name, string Description) GetDisplayInfo( PlayerComponent player ) {
		//if (player.HasTechnology<Metallurgy>())
		//	return ("ironOre", "Iron Ore", "High-grade hematite ore, containing 70% iron, ideal for steelmaking.");
		//if (player.HasTechnology<SteelWorking>())
		//	return ("ironOre", "Iron Ore", "High-grade hematite ore, ideal for steelmaking.");
		//if (player.HasTechnology<IronWorking>())
		//	return ("ironOre", "Iron Ore", "Rocks containing veins of a metallic substance that can be smelted into tools.");
		return ("ironOre", "Rock", "A heavy, rust-colored stone that stains your hands red when handled.");
	}
}
//public sealed class IronMetal() : ResourceBase( 0x0001_0001, "IRON_METAL", ResourceTags.METAL, ResourceTags.IRON ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		if (player.HasTechnology<SteelWorking>())
//			return ("Iron", "A shiny gray metal prone to rusting. Alloyed with carbon to make steel.");
//		if (player.HasTechnology<IronWorking>() && player.HasTechnology<BronzeWorking>())
//			return ("Iron", "A shiny gray metal prone to rusting, with some work hardening it can be as hard as bronze.");
//		if (player.HasTechnology<IronWorking>())
//			return ("Iron", "A shiny gray metal prone to rusting, with some work hardening it can made into useful tools.");
//		return ("Gray Metal", "A shiny gray metal prone to rusting and too malleable to be useful.");
//	}
//}
//public sealed class FreshWater() : ResourceBase( 0x0001_0002, "FRESH_WATER", ResourceTags.WATER ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		if (player.HasTechnology<Micobiology>())
//			return ("Fresh Water", "While generally safe for basic uses, this water may still harbor unseen contaminants.");
//		return ("Water", "The source of life.");
//	}
//}
//public sealed class DrinkingWater() : ResourceBase( 0x0001_0003, "DRINKING_WATER", ResourceTags.WATER ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		if (player.HasTechnology<Micobiology>())
//			return ("Drinking Water", "Water treated to ensure it is safe and free of harmful microbes.");
//		return ("Water", "The source of life.");
//	}
//}
//public sealed class GroundWater() : ResourceBase( 0x0001_0004, "GROUND_WATER", ResourceTags.WATER ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		if (player.HasTechnology<Micobiology>())
//			return ("Groundwater", "While generally safe for basic uses, this water may still harbor unseen contaminants.");
//		return ("Water", "The source of life.");
//	}
//}
//public sealed class SeaWater() : ResourceBase( 0x0001_0005, "SEA_WATER", ResourceTags.WATER ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		if (player.HasTechnology<Micobiology>())
//			return ("Drinking Water", "Water treated to ensure it is safe and free of harmful microbes.");
//		return ("Sea Water", "The source of life spoiled by .");
//	}
//}
//public sealed class SewageWater() : ResourceBase( 0x0001_0006, "SEWAGE_WATER", ResourceTags.WATER ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		if (player.HasTechnology<Micobiology>())
//			return ("Sewage", "Must be treated before safe consumption.");
//		return ("Dirty Water", "The source of life spoiled by dirty materials.");
//	}
//}
//public sealed class HolyWater() : ResourceBase( 0x0001_0007, "HOLY_WATER", ResourceTags.WATER ) {
//	public override (string Name, string Description) GetDisplayName( PlayerComponent player ) {
//		return ("Holy Water", $"Water imbued with the gift of {(player.Religions.MainReligion.Polytheistic ? "the gods" : "God")}.");
//	}
//}