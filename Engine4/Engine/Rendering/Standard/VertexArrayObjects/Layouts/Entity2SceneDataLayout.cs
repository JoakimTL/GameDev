using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Entity2SceneData ) )]
public class Entity2SceneDataLayout : CompositeVertexArrayObjectDataLayout {
	public Entity2SceneDataLayout() : base( nameof( Entity2SceneData ), Resources.Render.VBOs.Get<Entity2SceneData>(), 0, Marshal.SizeOf<Entity2SceneData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 0 ) ); //mat4x4 1
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 16 ) ); //mat4x4 2
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 32 ) ); //mat4x4 3
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 48 ) ); //mat4x4 4
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 64, normalized: true ) ); //color
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 72, AttributeType.LARGE ) ); //diffuse texture handle
	}
}
