namespace Engine.Modules.ECS;

public abstract class SystemBase {

	/// <summary>
	/// Types this system requires to function, or even exists.
	/// </summary>
	public IReadOnlySet<Type> RequiredTypes { get; }
	/// <summary>
	/// Which part of the multiplayer network this system is allowed to run on.<br/>
	/// <b>SERVER</b> means it will only run on the server<br/>
	/// <b>OWNER</b> means it will run on the server and the client that owns the entity<br/>
	/// <b>EVERYONE</b> means it will run on the server and all clients.
	/// </summary>
	public NetworkAllowance ExecutionAllowanceLevel { get; }

	protected SystemBase( NetworkAllowance executionAllowanceLevel, params Type[] requiredTypes ) {
		this.ExecutionAllowanceLevel = executionAllowanceLevel;
		this.RequiredTypes = requiredTypes.ToHashSet();
	}

	protected internal abstract void Update( IReadOnlyCollection<Entity> eligibleEntities, in double time, in double deltaTime );
}