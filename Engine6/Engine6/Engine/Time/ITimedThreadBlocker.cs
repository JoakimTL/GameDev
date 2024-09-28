namespace Engine.Time;

public interface ITimedThreadBlocker : ICancellable {
	/// <summary>
	/// Resets the internal timer of the blocker.<br/>
	/// Can be called before execution in case a desync has happened.
	/// </summary>
	void Set();

	/// <summary>
	/// Assumes blocking happens after execution, to ensure the time between each execution is constant if possible.
	/// </summary>
	/// <returns>The state of the the timed blocker.<br/>
	TimedBlockerState Block();
}

public enum TimedBlockerState {
	Blocking,
	NonBlocking,
	Skipping,
	Cancelled
}