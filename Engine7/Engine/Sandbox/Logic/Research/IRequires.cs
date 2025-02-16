namespace Sandbox.Logic.Research;

public interface IRequires {
	Type RequiredTechnology { get; }
}

public sealed class TechnologyRequirement;

public sealed class ResourceRequirement;

public sealed class BuildingRequirement;