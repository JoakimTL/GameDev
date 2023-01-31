using OpenGL;

namespace Engine.Rendering.Objects.VAOs;

public class VertexArrayLayoutFieldData {

	public readonly VertexAttribType VertexAttributeType;
	public readonly uint VertexCount;
	public readonly int RelativeOffsetBytesOverride;
	public readonly VertexArrayAttributeType AttributeType;
	public readonly bool Normalized;

	internal VertexArrayLayoutFieldData( VertexAttribType vertexAttributeType, uint vertexCount, int relativeOffsetBytesOverride, VertexArrayAttributeType attributeType, bool normalized ) {
		this.VertexAttributeType = vertexAttributeType;
		this.VertexCount = vertexCount;
		this.RelativeOffsetBytesOverride = relativeOffsetBytesOverride;
		this.AttributeType = attributeType;
		this.Normalized = normalized;
	}
}

