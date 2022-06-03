using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Entity3SceneData {
	[FieldOffset( 0 )]
	public Matrix4x4 ModelMatrix;
	[FieldOffset( 64 )]
	public Color16x4 Color;
	[FieldOffset( 72 )]
	public float NormalMapped;
	[FieldOffset( 76 )]
	public ulong DiffuseTextureHandle;
	[FieldOffset( 84 )]
	public ulong NormalTextureHandle;
	[FieldOffset( 92 )]
	public ulong LightingTextureHandle;
	[FieldOffset( 100 )]
	public ulong GlowTextureHandle;
}
