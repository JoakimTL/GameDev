using Engine;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Sandbox;

//public sealed class TestVertexArrayObject : OglVertexArrayObjectBase {
//	private readonly OglBufferBase _vertexBuffer;
//	private readonly OglBufferBase _elementBuffer;

//	public TestVertexArrayObject( OglBufferBase vertexBuffer, OglBufferBase elementBuffer ) {
//		this._vertexBuffer = vertexBuffer;
//		this._elementBuffer = elementBuffer;
//	}
//	protected override void Setup() {
//		uint binding = BindBuffer( this._vertexBuffer.BufferId, 0, 2 * sizeof( float ) );
//		this.SetupAttrib( binding, 0, 2, VertexAttribType.Float, false, 0 );
//		SetElementBuffer( this._elementBuffer );
//	}
//}

[Identity( nameof( Vertex2 ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Vertex2( Vector2<float> translation, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 8 )]
	public Vector4<byte> Color = color;
}
