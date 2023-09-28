namespace Engine;

public class InstantiableUid32Tracker {

	private int _count = 0;

	public uint Peek => unchecked((uint) this._count);

	/// <summary>
	/// Returns a unique uint.
	/// Does never return 0, meaning an ID of 0 qualifies as invalid.
	/// </summary>
	public uint Next {
		get {
			int newCount = Interlocked.Increment( ref this._count );
			return unchecked((uint) newCount);
		}
	}

	public InstantiableUid32Tracker( uint start = 0 ) {
		this._count = unchecked((int) start);
	}
}