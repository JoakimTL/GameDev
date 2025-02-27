namespace Sandbox.Logic.Setup;

public static class Definitions {
	public static DefinitionList<ResourceTypeBase> Resources { get; } = new();
	public static DefinitionList<BuildingTypeBase> BuildingTypes { get; } = new();
	public static DefinitionList<VocationTypeBase> Vocations { get; } = new();
	public static DefinitionList<ProfessionTypeBase> Professions { get; } = new();
	public static DefinitionList<TechnologyTypeBase> Technologies { get; } = new();
}