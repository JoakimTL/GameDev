using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Society;
public abstract class SocietalValueBase( string displayName ) {
	public string DisplayName { get; } = displayName;
}
