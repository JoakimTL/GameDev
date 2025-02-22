using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic;
public abstract class SelfIdentifyingBase {
	public Guid Id { get; }
	protected SelfIdentifyingBase() {
		GuidAttribute guidAttribute = TypeManager.ResolveType( GetType() ).GetAttribute<GuidAttribute>();
		string guidString = guidAttribute.Value;
		this.Id = new( guidString );
	}
}