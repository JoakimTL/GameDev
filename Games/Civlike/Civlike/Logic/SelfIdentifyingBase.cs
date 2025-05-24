using Engine;
using System.Runtime.InteropServices;

namespace Civlike.Logic;

public abstract class SelfIdentifyingBase {
	/// <summary>
	/// This value should never be used for persistent storage. It is only used to reference the object in memory. If you intend to store any value related to this id in storage, use the <see cref="Id"/> property.
	/// </summary>
	public uint MemoryId { get; private set; }
	public Guid Id { get; }
	protected SelfIdentifyingBase() {
		GuidAttribute guidAttribute = TypeManager.ResolveType( GetType() ).GetAttribute<GuidAttribute>();
		string guidString = guidAttribute.Value;
		this.Id = new( guidString );
		this.MemoryId = 0;
	}

	public override string ToString() => GetType().Name;
	public override bool Equals( object? obj ) => obj is SelfIdentifyingBase other && other.Id == this.Id;
	public override int GetHashCode() => this.Id.GetHashCode();
}
