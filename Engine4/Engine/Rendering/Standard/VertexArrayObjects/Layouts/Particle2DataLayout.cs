using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Particle2Data ) )]
public class Particle2DataLayout : CompositeVertexArrayObjectDataLayout {
	public Particle2DataLayout() : base( nameof( Particle2Data ), Resources.Render.VBOs.Get<Particle2Data>(), 0, Marshal.SizeOf<Particle2Data>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 8 ) ); //rotation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 16 ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 24, normalized: true ) ); //color
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 32, AttributeType.LARGE ) ); //texture handle 1
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 40, AttributeType.LARGE ) ); //texture handle 2
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 48 ) ); //blend

	}
}
