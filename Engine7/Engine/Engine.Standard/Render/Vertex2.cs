using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[Identity( nameof( Vertex2 ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Vertex2( Vector2<float> translation, Vector2<float> uv, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 8 )]
	public readonly Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public Vector4<byte> Color = color;
}