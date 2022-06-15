using System.Runtime.InteropServices;
using Engine.Rendering.Standard.Scenes.VerletParticles.Systems;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( VerletParticle3Data ) )]
public class VerletParticle3DataLayout : CompositeVertexArrayObjectDataLayout {
	public VerletParticle3DataLayout() : base( nameof( VerletParticle3Data ), Resources.Render.VBOs.Get<VerletParticle3Data>(), 0, Marshal.SizeOf<VerletParticle3Data>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 12 ) ); //velocity
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 24 ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 28, normalized: true ) ); //color

	}
}