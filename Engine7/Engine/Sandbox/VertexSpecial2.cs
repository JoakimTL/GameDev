using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;

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

[Identity( nameof( LetterVertex ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct LetterVertex( Vector2<float> translation, Vector2<float> uv, Vector4<byte> color, bool fill, bool flip ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 8 )]
	public Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public Vector4<byte> Color = color;
	[VAO.Data( VertexAttribType.UnsignedByte, 2, normalized: false ), FieldOffset( 20 )]
	public Vector2<byte> LetterInformation = new((byte)(fill ? 1 : 0), (byte)(flip ? 1 : 0));
}
[Identity( nameof( Vertex2UV ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Vertex2UV( Vector2<float> translation, Vector2<float> uv, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 8 )]
	public Vector2<float> UV = uv;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public Vector4<byte> Color = color;
}

[Identity( nameof( VertexSpecial2 ) )]
[VAO.Setup( 0, 0, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct VertexSpecial2( Vector2<float> translation, Vector4<byte> color ) {
	[VAO.Data( VertexAttribType.Float, 2 ), FieldOffset( 0 )]
	public Vector2<float> Translation = translation;
	[VAO.Data( VertexAttribType.UnsignedByte, 4, normalized: true ), FieldOffset( 16 )]
	public Vector4<byte> Color = color;
}
