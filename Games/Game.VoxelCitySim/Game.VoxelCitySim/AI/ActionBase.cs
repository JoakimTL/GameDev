using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.AI;
public abstract class ActionBase { }
public abstract class ActionBase<T>( T value ) : ActionBase {
	public T Value { get; protected set; } = value;
}
