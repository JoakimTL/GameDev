using Engine.Datatypes.Colors;
using Engine.Rendering.Objects.VAOs;
using OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace StandardPackage.Rendering.VertexArrayLayouts;


[VAO.Setup( 0, 1, 4 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Entity3SceneData {
	[VAO.Data( VertexAttribType.Float, 16 ), FieldOffset( 0 )]
	public Matrix4x4 ModelMatrix;
	[VAO.Data( VertexAttribType.UnsignedShort, 4 ), FieldOffset( 64 )]
	public Color16x4 Color;
	[VAO.Data( VertexAttribType.UnsignedByte, 1, normalized: true ), FieldOffset( 72 )]
	public byte NormalMapped;
}