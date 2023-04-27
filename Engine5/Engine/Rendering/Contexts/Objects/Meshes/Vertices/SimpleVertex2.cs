using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure.Attributes;
using OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Objects.Meshes.Vertices;

[Identity(nameof(SimpleVertex2))]
[VAO.Setup(0, 0, 0), StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct SimpleVertex2
{
    [VAO.Data(VertexAttribType.Float, 2), FieldOffset(0)]
    public Vector2 Translation;

    public SimpleVertex2(in Vector2 translation)
    {
        Translation = translation;
    }

    public static implicit operator SimpleVertex2(Vector2 vector) => new(vector);
}