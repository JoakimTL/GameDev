using Engine;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using OpenGL;
using System.Runtime.InteropServices;

namespace Sandbox;

[Identity( nameof( Entity2SceneData ) )]
[VAO.Setup( 0, 1, 0 ), StructLayout( LayoutKind.Explicit, Pack = 1 )]
public struct Entity2SceneData {
	[VAO.Data( VertexAttribType.Float, 16 ), FieldOffset( 0 )]
	public Matrix4x4<float> ModelMatrix;
}
