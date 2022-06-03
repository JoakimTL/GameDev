using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Vector3 ) )]
public class Vector3Layout : CompositeVertexArrayObjectDataLayout {
	public Vector3Layout() : base( nameof( Vector3 ), Resources.Render.VBOs.Get<Vector3>(), 0, Marshal.SizeOf<Vector3>(), 0 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 3, 0 ) ); //translation
	}
}
