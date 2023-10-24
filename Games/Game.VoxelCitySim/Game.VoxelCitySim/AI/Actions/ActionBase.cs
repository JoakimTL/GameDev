using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.AI.Actions;
public abstract class ActionBase( object target ) {
	protected readonly object _target = target;
}

public sealed class MoveToAction( Vector3 target ) : ActionBase( target );
