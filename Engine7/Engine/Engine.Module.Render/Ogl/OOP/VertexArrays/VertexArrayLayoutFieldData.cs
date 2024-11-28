using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.VertexArrays;

public class VertexArrayLayoutFieldData {

	public readonly VertexAttribType VertexAttributeType;
	public readonly uint VertexCount;
	public readonly uint RelativeOffsetBytes;
	public readonly VertexArrayAttributeType AttributeType;
	public readonly bool Normalized;

	internal VertexArrayLayoutFieldData( VertexAttribType vertexAttributeType, uint vertexCount, uint relativeOffsetBytes, VertexArrayAttributeType attributeType, bool normalized ) {
		this.VertexAttributeType = vertexAttributeType;
		this.VertexCount = vertexCount;
		this.RelativeOffsetBytes = relativeOffsetBytes;
		this.AttributeType = attributeType;
		this.Normalized = normalized;
	}
}
