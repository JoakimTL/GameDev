namespace Sandbox.Logic.Old.OldCiv.Research;

public interface IRequires {
	Type RequiredTechnology { get; }
}

public abstract class ResearchRequirementBase;

public sealed class TechnologyRequirement : ResearchRequirementBase;

public sealed class ResourceRequirement : ResearchRequirementBase;

public sealed class BuildingRequirement : ResearchRequirementBase;