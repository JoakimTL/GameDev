namespace Sandbox.Logic.Setup;

public abstract class VocationTypeBase : SelfIdentifyingBase {
	protected VocationTypeBase( SectorTypeBase sector, string name ) {
		this.Sector = sector;
		this.Name = name;
	}

	public SectorTypeBase Sector { get; }
	public string Name { get; }
}

public abstract class VocationTypeBase<TSector>( string name ) : VocationTypeBase( Definitions.Sectors.Get<TSector>(), name ) where TSector : SectorTypeBase;
