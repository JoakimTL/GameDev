using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Particle3Data ) )]
public class Particle3DataLayout : CompositeVertexArrayObjectDataLayout {
	public Particle3DataLayout() : base( nameof( Particle3Data ), Resources.Render.VBOs.Get<Particle3Data>(), 0, Marshal.SizeOf<Particle3Data>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 12 ) ); //rotation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 20 ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 28, normalized: true ) ); //color
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 36, AttributeType.LARGE ) ); //texture handle 1
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 44, AttributeType.LARGE ) ); //texture handle 2
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 52 ) ); //blend

	}
}