namespace Engine;

public static class Uid64 {

	private static readonly InstantiableUid64Tracker _tracker = new();

	/// <summary>
	/// Returns a unique ulong, used to identify parts in the engine. This allows for easier debugging if needed.<br/>
	/// <b>0 is never returned.</b>
	/// </summary>
	public static ulong Next => _tracker.Next;

}