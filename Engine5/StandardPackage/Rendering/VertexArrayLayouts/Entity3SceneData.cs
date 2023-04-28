using Engine.Datatypes.Colors;
using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure.Attributes;
using OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace StandardPackage.Rendering.VertexArrayLayouts;

[Identity(nameof(Entity3SceneData))]
[VAO.Setup(0, 1, 4), StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct Entity3SceneData
{
    [VAO.Data(VertexAttribType.Float, 16), FieldOffset(0)]
    public Matrix4x4 ModelMatrix;
    [VAO.Data(VertexAttribType.UnsignedShort, 4), FieldOffset(64)]
    public Color16x4 Color;
    [VAO.Data(VertexAttribType.UnsignedByte, 1, normalized: true), FieldOffset(72)]
    public byte NormalMapped;

    public static Entity3SceneData Interpolate(Entity3SceneData a, Entity3SceneData b, float interpolation) => new()
    {
        ModelMatrix = a.ModelMatrix * interpolation + b.ModelMatrix * (1 - interpolation),
        Color = (Vector4)a.Color * interpolation + (Vector4)b.Color * (1 - interpolation),
        NormalMapped = a.NormalMapped
    };
}