using Engine.Module.Entities.Container;

namespace Civs.Logic.Nations;
public sealed class PopulationCenterArchetype : ArchetypeBase {
	//public PopulationCenterComponent PopulationCenterComponent { get; set; } = null!;
	public FaceOwnershipComponent TileOwnershipComponent { get; set; } = null!;
	public FaceOwnershipRenderComponent TileOwnershipRenderComponent { get; set; } = null!;
}
