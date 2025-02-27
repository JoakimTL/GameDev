namespace Sandbox.Logic.Setup;
public abstract class TechnologyTypeBase : SelfIdentifyingBase {
	protected TechnologyTypeBase( string name, double researchProgressRequired ) {
		this.Name = name;
		this.ResearchProgressRequired = researchProgressRequired;
		Requirements = new( GetType() );
	}

	public string Name { get; }
	public double ResearchProgressRequired { get; }
	public TechnologyTypeRequirementList Requirements { get; }
}
