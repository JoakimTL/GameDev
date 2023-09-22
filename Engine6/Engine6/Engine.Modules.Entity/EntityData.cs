using System.Runtime.InteropServices;

namespace Engine.Modules.Entity;

[StructLayout( LayoutKind.Explicit )]
internal readonly struct EntityData {
	[FieldOffset( 0 )]
	public readonly Guid EntityId;
	[FieldOffset( 16 )]
	public readonly Guid OwnerId;
	[FieldOffset( 32 )]
	public readonly Guid ParentId;
}
