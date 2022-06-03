using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Lighting.Directional;

[StructLayout( LayoutKind.Explicit )]
public struct CascadeData {
	public const int NUMCASCADES = 4;
	[FieldOffset( 0 )]
	public ulong UnfilteredTextureHandle;
	[FieldOffset( 8 )]
	public ulong FilteredTextureHandle;
	[FieldOffset( 16 )]
	public ulong TransparencyColorTextureHandle;
	[FieldOffset( 24 )]
	public ulong TransparencyRevealTextureHandle;
	[FieldOffset( 32 )]
	public Matrix4x4 ViewProjectionMatrix;
	[FieldOffset( 96 )]
	public Vector2 TextureSize;
	[FieldOffset( 104 )]
	public float Depth;
}