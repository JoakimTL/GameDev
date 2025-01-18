using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Research;
public abstract class TechnologyBase {

	protected TechnologyBase( string displayName ) {
		this.DisplayName = displayName;
	}

	public string DisplayName { get; }
}
