using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Vector2 ) )]
public class Vector2Layout : CompositeVertexArrayObjectDataLayout {
	public Vector2Layout() : base( nameof(Vector2), Resources.Render.VBOs.Get<Vector2>(), 0, Marshal.SizeOf<Vector2>(), 0 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 2, 0 ) ); //translation
	}
}
