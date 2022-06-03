using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Vertex2 ) )]
public class Vertex2Layout : CompositeVertexArrayObjectDataLayout {
	public Vertex2Layout() : base( nameof( Vertex2 ), Resources.Render.VBOs.Get<Vertex2>(), 0, Marshal.SizeOf<Vertex2>(), 0 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 12 ) ); //texture coordinates
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 20, normalized: true ) ); //color
	}
}
