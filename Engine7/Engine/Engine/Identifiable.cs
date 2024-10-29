namespace Engine;

public abstract class Identifiable {

	private static ulong _uniqueIdCounter;
	private static ulong GetNextUniqueId() => Interlocked.Increment( ref _uniqueIdCounter );

	public ulong UniqueId { get; } = GetNextUniqueId();

	public override bool Equals( object? obj ) => obj is Identifiable identifiable && identifiable.UniqueId == this.UniqueId;
	public override int GetHashCode() => this.UniqueId.GetHashCode();
	public override string ToString() => $"{GetType().Name}#{this.UniqueId}";
}
