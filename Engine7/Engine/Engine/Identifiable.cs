using System.Xml;

namespace Engine;

public abstract class Identifiable {

	private static ulong _uniqueIdCounter;
	private static ulong GetNextUniqueId() => Interlocked.Increment( ref _uniqueIdCounter );

	public ulong UniqueId { get; }
	public string? Nickname { get; protected set; }
	private readonly int _hashCode;

	protected Identifiable() {
		this.UniqueId = GetNextUniqueId();
		this.Nickname = null;
		this._hashCode = this.UniqueId.GetHashCode();
	}

	public override bool Equals( object? obj ) => obj is Identifiable identifiable && identifiable.UniqueId == this.UniqueId;
	public override int GetHashCode() => this._hashCode;
	public override string ToString() => string.IsNullOrEmpty( this.Nickname ) ? $"{GetType().Name}#{this.UniqueId}" : $"{this.Nickname}#{this.UniqueId}";
}
