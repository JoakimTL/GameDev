using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.AI;
public abstract class ProcessorBase {



	public abstract void Process( IEnumerable<Agent> agents );
}
