using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( PointLightData ) )]
public class PointLightDataLayout : CompositeVertexArrayObjectDataLayout {
	public PointLightDataLayout() : base( nameof( PointLightData ), Resources.Render.VBOs.Get<PointLightData>(), 0, Marshal.SizeOf<PointLightData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 0, normalized: true ) ); //color
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 6 ) ); //intensity
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 10 ) ); //radius
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 14 ) ); //translation
	}
}
