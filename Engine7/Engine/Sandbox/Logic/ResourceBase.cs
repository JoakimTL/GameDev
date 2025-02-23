using System.Runtime.InteropServices;

namespace Sandbox.Logic;
public abstract class SelfIdentifyingBase {
	public Guid Id { get; }
	protected SelfIdentifyingBase() {
		GuidAttribute guidAttribute = TypeManager.ResolveType( GetType() ).GetAttribute<GuidAttribute>();
		string guidString = guidAttribute.Value;
		this.Id = new( guidString );
	}
}