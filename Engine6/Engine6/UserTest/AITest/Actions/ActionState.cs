namespace UserTest.AITest.Actions;

public enum ActionState {
	Performing,
	Completed,
	/// <summary>
	/// If the agent is unable to perform the action, this state is returned.
	/// </summary>
	Failed,
	/// <summary>
	/// Whenever the current action is no longer valid, and the current approach needs to be reevaluated.
	/// </summary>
	Reevaluate,
	NoAction,
}