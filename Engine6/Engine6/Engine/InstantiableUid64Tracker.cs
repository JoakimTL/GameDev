namespace Engine;

public class InstantiableUid64Tracker {

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

	public InstantiableUid64Tracker( ulong start = 0 ) {
		this._count = unchecked((long) start);
	}
}
