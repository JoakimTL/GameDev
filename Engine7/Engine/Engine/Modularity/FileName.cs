using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Modularity;
internal class FileName {
}


public abstract class ModuleBase {

	public IInstanceProvider InstanceProvider { get; }

	public ModuleBase() {
		this.InstanceProvider = InstanceManagement.CreateProvider();
	}

}