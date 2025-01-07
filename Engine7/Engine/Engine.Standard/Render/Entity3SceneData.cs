using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[ Identity( nameof( Entity3SceneData ) )]
[VAO.Setup( 0, 1, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Entity3SceneData( Matrix4x4<float> modelMatrix, Vector4<ushort> color ) {
	[VAO.Data( VertexAttribType.Float, 16 ), FieldOffset( 0 )]
	public readonly Matrix4x4<float> ModelMatrix = modelMatrix;
	[VAO.Data( VertexAttribType.UnsignedShort, 4, normalized: true ), FieldOffset( 64 )]
	public readonly Vector4<ushort> Color = color;

	public override readonly string ToString() => $"{this.ModelMatrix} {Color}";
}
