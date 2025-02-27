namespace Sandbox.Logic.Setup;

public abstract class ProfessionTypeBase : SelfIdentifyingBase {
	protected ProfessionTypeBase( VocationTypeBase vocation, string name ) {
		this.Vocation = vocation;
		this.Name = name;
	}
	public VocationTypeBase Vocation { get; }
	public string Name { get; }
}

public abstract class ProfessionTypeBase<T>( string name ) : ProfessionTypeBase( Definitions.Vocations.Get<T>(), name ) where T : VocationTypeBase;
