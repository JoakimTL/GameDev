namespace Engine.Modules.Entity;

public class Class1 {

}

public abstract class SystemBase  {

	/// <summary>
	/// Types this system requires to function, or even exists.
	/// </summary>
	internal protected IReadOnlySet<Type> RequiredTypes { get; }
	/// <summary>
	/// Which part of the multiplayer network this system is allowed to run on.<br/>
	/// <b>SERVER</b> means it will only run on the server<br/>
	/// <b>OWNER</b> means it will run on the server and the client that owns the entity<br/>
	/// <b>EVERYONE</b> means it will run on the server and all clients.
	/// </summary>
	internal protected NetworkAllowance ExecutionAllowanceLevel { get; }

	protected SystemBase( NetworkAllowance executionAllowanceLevel, params Type[] requiredTypes ) {
		this.ExecutionAllowanceLevel = executionAllowanceLevel;
		this.RequiredTypes = requiredTypes.ToHashSet();
	}

}