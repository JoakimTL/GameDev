namespace Sandbox.Logic.Setup;

public abstract class BuildingTypeBase : SelfIdentifyingBase {
	protected BuildingTypeBase( string name, ResourceTable constructionCost, bool permanentBuilding = true ) {
		this.Name = name;
		this.ConstructionCost = constructionCost;
		this.IsPermanent = permanentBuilding;
	}

	public string Name { get; }
	public ResourceTable ConstructionCost { get; }
	public bool IsPermanent { get; }
}
