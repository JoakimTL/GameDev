using OpenGL;

namespace Engine.Rendering.Contexts.Objects.VAOs;

public class VertexArrayLayoutFieldData
{

    public readonly VertexAttribType VertexAttributeType;
    public readonly uint VertexCount;
    public readonly uint RelativeOffsetBytes;
    public readonly VertexArrayAttributeType AttributeType;
    public readonly bool Normalized;

    internal VertexArrayLayoutFieldData(VertexAttribType vertexAttributeType, uint vertexCount, uint relativeOffsetBytes, VertexArrayAttributeType attributeType, bool normalized)
    {
        VertexAttributeType = vertexAttributeType;
        VertexCount = vertexCount;
        RelativeOffsetBytes = relativeOffsetBytes;
        AttributeType = attributeType;
        Normalized = normalized;
    }
}

