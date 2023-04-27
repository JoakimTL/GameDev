using Engine.Datatypes.Colors;
using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure.Attributes;
using OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Objects.Meshes.Vertices;

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

    public Vertex2(in Vector2 translation, in Vector2 uv, in Vector4 color)
    {
        Translation = translation;
        UV = uv;
        Color = color;
    }

    public Vertex2(in Vector2 translation, in Vector2 uv, in Color16x4 color)
    {
        Translation = translation;
        UV = uv;
        Color = color;
    }
}
