using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Nations;
public abstract class TaxationBase(string name) {
	public string Name { get; set; } = name;
	public abstract void Collect( double time, double deltaTime );
}
