namespace Engine.Structure.Uid;

/// <summary>
/// A instantiable version of <see cref="UID64"/>. Used for objects that need a fast hashcode or equals.
/// </summary>
public class InstantiableUIDTrackerUInt64 {

    private long _count = 0;

    public ulong Peek => unchecked((ulong) _count);

    /// <summary>
    /// Returns a unique ulong.
    /// Does never return 0, meaning an ID of 0 qualifies as invalid.
    /// </summary>
    public ulong Next {
        get {
            long newCount = Interlocked.Increment( ref _count );
            return unchecked((ulong) newCount);
        }
    }

    public InstantiableUIDTrackerUInt64( ulong start = 0 ) {
        _count = unchecked((long) start);
    }
}
