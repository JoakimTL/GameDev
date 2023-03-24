using Engine.Datatypes.Colors;
using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure.Attributes;
using OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Datatypes.Vertices;

[Identity(nameof(Vertex3))]
[VAO.Setup(0, 0, 0), StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct Vertex3
{
	[VAO.Data(VertexAttribType.Float, 3), FieldOffset(0)]
	public Vector3 Translation;
	[VAO.Data(VertexAttribType.Float, 2), FieldOffset(12)]
	public Vector2 UV;
	[VAO.Data(VertexAttribType.Float, 3), FieldOffset(20)]
	public Vector3 Normal;
	[VAO.Data(VertexAttribType.UnsignedShort, 4), FieldOffset(32)]
	public Color16x4 Color;

	public Vertex3(in Vector3 position)
	{
		this.Translation = position;
		this.UV = default;
		this.Normal = default;
		this.Color = Color16x4.White;
	}

	public Vertex3(in Vector3 position, in Vector2 uv)
	{
		this.Translation = position;
		this.UV = uv;
		this.Normal = default;
		this.Color = Color16x4.White;
	}

	public Vertex3(in Vector3 position, in Vector2 uv, in Vector3 normal, in Vector4 color)
	{
		this.Translation = position;
		this.UV = uv;
		this.Normal = normal;
		this.Color = color;
	}

	public Vertex3(in Vector3 position, in Vector2 uv, in Vector3 normal, in Color16x4 color)
	{
		this.Translation = position;
		this.UV = uv;
		this.Normal = normal;
		this.Color = color;
	}
}
