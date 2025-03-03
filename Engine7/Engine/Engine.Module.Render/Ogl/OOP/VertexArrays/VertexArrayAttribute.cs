﻿using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.VertexArrays;

public readonly struct VertexArrayAttribute {

	public readonly VertexAttribType VertexAttributeType;
	public readonly int VertexCount;
	public readonly uint RelativeOffsetBytes;
	public readonly VertexArrayAttributeType AttributeType;
	public readonly bool Normalized;

	public VertexArrayAttribute( VertexAttribType vertexAttributeType, int vertexCount, uint relativeOffsetBytes, VertexArrayAttributeType attributeType = VertexArrayAttributeType.DEFAULT, bool normalized = false ) {
		this.VertexAttributeType = vertexAttributeType;
		this.VertexCount = vertexCount;
		this.RelativeOffsetBytes = relativeOffsetBytes;
		this.AttributeType = attributeType;
		this.Normalized = normalized;
	}
}
