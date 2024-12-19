using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[Identity( nameof( Entity2SceneData ) )]
[VAO.Setup( 0, 1, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public readonly struct Entity2SceneData( Matrix4x4<float> modelMatrix ) {
	[VAO.Data( VertexAttribType.Float, 16 ), FieldOffset( 0 )]
	public readonly Matrix4x4<float> ModelMatrix = modelMatrix;

	public override readonly string ToString() => $"{this.ModelMatrix}";
}
