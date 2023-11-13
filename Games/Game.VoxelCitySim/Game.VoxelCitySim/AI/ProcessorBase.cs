using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.AI;
public abstract class ProcessorBase {



	public abstract void Process( IEnumerable<Agent> agents );
}

public abstract class GoalBase : IUpdateable {

	/// <summary>
	/// How high of a priority this goal is. 0 means the goal is not active. There are no upper bounds.
	/// </summary>
	public abstract float Priority { get; }
	public bool IsActive => this.Priority > 0;

	public abstract void Update( in double time, in double deltaTime );
}

public abstract class StateBase { }
public abstract class StateBase<T>( T value ) : StateBase {
	public T Value { get; protected set; } = value;
}