using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;
public sealed class PopulationCenterArchetype : ArchetypeBase {
	public PopulationCenterComponent PopulationCenterComponent { get; set; } = null!;
	//public TileOwnershipComponent TileOwnershipComponent { get; set; } = null!;
	public TileOwnershipRenderComponent TileOwnershipRenderComponent { get; set; } = null!;
}
