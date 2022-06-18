using System.Runtime.InteropServices;
using Engine.Rendering.Standard.VertexArrayObjects;

namespace Engine.Rendering.Standard.UI.Standard.Text;

[VertexLayoutBinding( typeof( TextGlyphRenderData ) )]
public class TextGlyphRenderDataLayout : CompositeVertexArrayObjectDataLayout {
	public TextGlyphRenderDataLayout() : base( nameof( TextGlyphRenderData ), Resources.Render.VBOs.Get<TextGlyphRenderData>(), 0, Marshal.SizeOf<TextGlyphRenderData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 0 ) ); //translation (yes, 3d) (text are basically particles, and can be displayed in 3d or 2d.)
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 12 ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 20 ) ); //rotation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 24 ) ); //uv
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 40, normalized: true ) ); //color
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 48, AttributeType.LARGE ) ); //font texture handle
	}
}