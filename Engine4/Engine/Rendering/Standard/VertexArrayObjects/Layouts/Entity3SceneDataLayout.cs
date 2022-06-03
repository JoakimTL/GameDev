using System.Runtime.InteropServices;

namespace Engine.Rendering.Standard.VertexArrayObjects.Layouts;

[VertexLayoutBinding( typeof( Entity3SceneData ) )]
public class Entity3SceneDataLayout : CompositeVertexArrayObjectDataLayout {
	public Entity3SceneDataLayout() : base( nameof( Entity3SceneData ), Resources.Render.VBOs.Get<Entity3SceneData>(), 0, Marshal.SizeOf<Entity3SceneData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 0 ) ); //mat4x4 1
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 16 ) ); //mat4x4 2
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 32 ) ); //mat4x4 3
		AddAttribute( new( OpenGL.VertexAttribType.Float, 4, 48 ) ); //mat4x4 4
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 4, 64, normalized: true ) ); //color
		AddAttribute( new( OpenGL.VertexAttribType.Float, 1, 72 ) ); //normalMapped
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 76, AttributeType.LARGE ) ); //diffuse texture handle
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 84, AttributeType.LARGE ) ); //normal texture handle
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 92, AttributeType.LARGE ) ); //lighting texture handle
		AddAttribute( new( (OpenGL.VertexAttribType) 5135, 1, 100, AttributeType.LARGE ) ); //glow texture handle
	}
}