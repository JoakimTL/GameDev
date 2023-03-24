using Engine.Datatypes.Colors;
using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure.Attributes;
using OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Datatypes.Vertices;

[Identity(nameof(Vertex2))]
[VAO.Setup(0, 0, 0), StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct Vertex2
{
	[VAO.Data(VertexAttribType.Float, 2), FieldOffset(0)]
	public Vector2 Translation;
	[VAO.Data(VertexAttribType.Float, 2), FieldOffset(8)]
	public Vector2 UV;
	[VAO.Data(VertexAttribType.UnsignedShort, 4), FieldOffset(16)]
	public Color16x4 Color;

	public Vertex2(Vector2 translation, Vector2 uv, Vector4 color)
	{
		this.Translation = translation;
		this.UV = uv;
		this.Color = color;
	}

	public Vertex2(Vector2 translation, Vector2 uv, Color16x4 color)
	{
		this.Translation = translation;
		this.UV = uv;
		this.Color = color;
	}
}