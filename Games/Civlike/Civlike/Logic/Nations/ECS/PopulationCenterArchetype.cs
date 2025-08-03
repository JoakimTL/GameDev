using Engine.Module.Entities.Container;

namespace Civlike.Logic.Nations.ECS;
public sealed class PopulationCenterArchetype : ArchetypeBase {
	public PopulationCenterComponent PopulationCenter { get; set; } = null!;
	public FaceOwnershipComponent TileOwnership { get; set; } = null!;
}
