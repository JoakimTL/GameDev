using System.Runtime.InteropServices;
using Engine.Rendering.Standard.VertexArrayObjects;

namespace Engine.Rendering.Standard.UI.Standard.Text;

[VertexLayoutBinding( typeof( TextGlyphRenderData ) )]
public class TextGlyphRenderDataLayout : CompositeVertexArrayObjectDataLayout {
	public TextGlyphRenderDataLayout() : base( nameof( TextGlyphRenderData ), Resources.Render.VBOs.Get<TextGlyphRenderData>(), 0, Marshal.SizeOf<TextGlyphRenderData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 0 ) ); //mat4x4 p1
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 16 ) ); //mat4x4 p1
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 32 ) ); //mat4x4 p1
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 48 ) ); //mat4x4 p1
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 64, AttributeType.LARGE ) ); //font texture handle
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 72 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 80 ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 88 ) ); //rotation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 96 ) ); //uv
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 112, normalized: true ) ); //color
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 120 ) ); //glyphData
	}
}