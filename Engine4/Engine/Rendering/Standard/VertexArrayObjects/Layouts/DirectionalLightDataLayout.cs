using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( DirectionalLightData ) )]
public class DirectionalLightDataLayout : CompositeVertexArrayObjectDataLayout {
	public DirectionalLightDataLayout() : base( nameof( DirectionalLightData ), Resources.Render.VBOs.Get<DirectionalLightData>(), 0, Marshal.SizeOf<DirectionalLightData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 3, 0, normalized: true ) ); //color
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 6 ) ); //intensity
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 10 ) ); //direction
	}
}
