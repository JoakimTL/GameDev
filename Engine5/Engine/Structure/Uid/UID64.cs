namespace Engine.Structure.Uid;

/// <summary>
/// A Unique IDentification handout class.
/// </summary>
public static class UID64
{

    private static readonly InstantiableUIDTrackerUInt64 _tracker = new();

    /// <summary>
    /// Returns a unique ulong, used to identify parts in the engine. This allows for easier debugging if needed.<br/>
    /// <b>0 is never returned.</b>
    /// </summary>
    public static ulong Next => _tracker.Next;

}