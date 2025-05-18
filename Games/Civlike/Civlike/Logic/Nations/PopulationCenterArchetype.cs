using Engine.Module.Entities.Container;

namespace Civlike.Logic.Nations;
public sealed class PopulationCenterArchetype : ArchetypeBase {
	public PopulationCenterComponent PopulationCenter { get; set; } = null!;
	public FaceOwnershipComponent TileOwnership { get; set; } = null!;
}
