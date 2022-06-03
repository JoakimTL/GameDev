namespace Engine;

/// <summary>
/// A instantiable version of <see cref="UID64"/>. Used for objects that need a fast hashcode or equals.
/// </summary>
public class InstantiableUIDTrackerUInt64 {

	private long _count = 0;

	public ulong Peek => unchecked((ulong) this._count);

	/// <summary>
	/// Returns a unique ulong.
	/// Does never return 0, meaning an ID of 0 qualifies as invalid.
	/// </summary>
	public ulong Next {
		get {
			long newCount = Interlocked.Increment( ref this._count );
			return unchecked((ulong) newCount);
		}
	}

	public InstantiableUIDTrackerUInt64( ulong start = 0 ) {
		this._count = unchecked((long) start);
	}
}

/// <summary>
/// A Unique IDentification handout class.
/// </summary>
public static class UID64 {

	private static readonly InstantiableUIDTrackerUInt64 _tracker = new();

	/// <summary>
	/// Returns a unique ulong, used to identify parts in the engine. This allows for easier debugging if needed.
	/// Does never return 0, meaning an ID of 0 qualifies as invalid.
	/// </summary>
	public static ulong Next => _tracker.Next;

}
