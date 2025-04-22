using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine;
using OpenGL;
using System.Runtime.InteropServices;

namespace Civs.Render.World.Lines;

[Identity( nameof( LineVertex ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct LineVertex( Vector2<float> translation, Vector2<float> uv, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public readonly Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 8 )]
	public readonly Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public readonly Vector4<byte> Color = color;
}
