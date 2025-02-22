using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Setup;
public abstract class ResourceBase : SelfIdentifyingBase {
	protected ResourceBase(string name) {
		this.Name = name;
	}

	public string Name { get; }
}
