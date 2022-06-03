using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Vertex3 ) )]
public class Vertex3Layout : CompositeVertexArrayObjectDataLayout {
	public Vertex3Layout() : base( nameof( Vertex3 ), Resources.Render.VBOs.Get<Vertex3>(), 0, Marshal.SizeOf<Vertex3>(), 0 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 12 ) ); //texture coordinates
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 20 ) ); //normal vectors
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 32, normalized: true ) ); //color
	}
}
