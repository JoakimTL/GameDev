using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[Identity( nameof( Entity2TexturedSceneData ) )]
[VAO.Setup( 0, 1, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Entity2TexturedSceneData( Matrix4x4<float> modelMatrix, Vector4<ushort> color, ulong textureId ) {
	[VAO.Data( VertexAttribType.Float, 16 ), FieldOffset( 0 )]
	public readonly Matrix4x4<float> ModelMatrix = modelMatrix;
	[VAO.Data( VertexAttribType.UnsignedShort, 4, normalized: true ), FieldOffset( 64 )]
	public readonly Vector4<ushort> Color = color;
	[VAO.Data( (OpenGL.VertexAttribType) 5135, 1, VertexArrayAttributeType.LARGE ), FieldOffset( 80 )]
	public readonly ulong TextureId = textureId;

	public override readonly string ToString() => $"{this.ModelMatrix} {Color} {TextureId}";
}
