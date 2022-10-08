using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.SceneObjects;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Entity2SceneData {
	[FieldOffset( 0 )]
	public Matrix4x4 ModelMatrix;
	[FieldOffset( 64 )]
	public Color16x4 Color;
	[FieldOffset( 72 )]
	[Texture]
	public ulong DiffuseTextureHandle;
}
