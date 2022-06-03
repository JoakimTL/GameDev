using OpenGL;

namespace Engine.Rendering.Standard.VertexArrayObjects;

public readonly struct AttributeDataLayout {

	public readonly VertexAttribType VertexAttributeType;
	public readonly int VertexCount;
	public readonly uint RelativeOffsetBytes;
	public readonly AttributeType AttributeType;
	public readonly bool Normalized;

	public AttributeDataLayout( VertexAttribType vertexAttributeType, int vertexCount, uint relativeOffsetBytes, AttributeType attributeType = AttributeType.DEFAULT, bool normalized = false ) {
		this.VertexAttributeType = vertexAttributeType;
		this.VertexCount = vertexCount;
		this.RelativeOffsetBytes = relativeOffsetBytes;
		this.AttributeType = attributeType;
		this.Normalized = normalized;
	}
}