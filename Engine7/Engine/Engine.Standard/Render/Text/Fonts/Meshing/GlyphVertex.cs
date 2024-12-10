using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Standard.Render.Text.Fonts.Meshing;

[Identity( nameof( GlyphVertex ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct GlyphVertex( Vector2<float> translation, Vector2<float> uv, Vector4<byte> color, bool fill, bool flip ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public readonly Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 8 )]
	public readonly Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public readonly Vector4<byte> Color = color;
	[VAO.Data( VertexAttribType.UnsignedByte, 2, normalized: false ), FieldOffset( 20 )]
	public readonly Vector2<byte> LetterInformation = new( (byte) (fill ? 1 : 0), (byte) (flip ? 1 : 0) );

	public override string ToString() => $"GlyphVertex: {Translation}, {UV}, {Color}, {LetterInformation}";
}