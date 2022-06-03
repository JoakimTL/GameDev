namespace Engine;

/// <summary>
/// A instantiable version of <see cref="UID64"/> that gives a uint rather than ulong. Used for objects that need a fast hashcode or equals.
/// </summary>
public class InstantiableUIDTrackerUInt32 {

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

	public InstantiableUIDTrackerUInt32( uint start = 0 ) {
		this._count = unchecked((int) start);
	}
}
