using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[Identity( nameof( Vertex3 ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Vertex3( Vector3<float> translation, Vector2<float> uv, Vector3<float> normal, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 3 ), FieldOffset( 0 )]
	public readonly Vector3<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 12 )]
	public readonly Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.Float, 3 ), FieldOffset( 20 )]
	public readonly Vector3<float> Normal = normal;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 32 )]
	public readonly Vector4<byte> Color = color;
}
