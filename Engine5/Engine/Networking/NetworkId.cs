using System.Runtime.InteropServices;

namespace Engine.Networking;

[StructLayout( LayoutKind.Explicit )]
public readonly struct NetworkId {

	[FieldOffset( 0 )]
	public readonly uint Id;

	public NetworkId( uint id ) {
		this.Id = id;
	}

	public override int GetHashCode()
		=> Id.GetHashCode();

	public override bool Equals( object? obj )
		=> obj is NetworkId other && Id == other.Id;

	public override string ToString()
		=> $"{Id >> 16 & 0xffff:x4}'{Id & 0xffff:x4}";

	public static bool operator ==( NetworkId left, NetworkId right )
		=> left.Equals( right );

	public static bool operator !=( NetworkId left, NetworkId right )
		=> !( left == right );

	public static implicit operator uint( NetworkId id )
		=> id.Id;
}
