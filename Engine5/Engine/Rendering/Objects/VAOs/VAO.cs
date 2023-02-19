using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Objects.VAOs;

public static class VAO {
	[AttributeUsage( AttributeTargets.Struct, AllowMultiple = false )]
	public sealed class SetupAttribute : Attribute {
		public uint OffsetBytes { get; private set; }

		/// <summary>
		/// Overrides the stride completely, this value will be used instead of the generated value. The generated value is the size of the struct in bytes + (texture count * 2) bytes.
		/// </summary>
		public int StrideBytesOverride { get; private set; }

		/// <summary>
		/// The instance divisor for this layout. A divisor of 0 or 1 is recommended.
		/// </summary>
		public uint InstanceDivisor { get; private set; }

		/// <summary>
		/// The number of textures this layout has. They are added at the end of the layout, so the fields in the struct comes first.
		/// </summary>
		public ushort TextureCount { get; private set; }
		public SetupAttribute( uint offsetBytes, uint instanceDivisor, ushort textureCount, int strideBytesOverride = -1 ) {
			OffsetBytes = offsetBytes;
			InstanceDivisor = instanceDivisor;
			TextureCount = textureCount;
			StrideBytesOverride = strideBytesOverride;
		}
	}

	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
	public sealed class DataAttribute : Attribute {

		public readonly VertexAttribType VertexAttributeType;
		public readonly uint VertexCount;
		public readonly int RelativeOffsetBytesOverride;
		public readonly VertexArrayAttributeType AttributeType;
		public readonly bool Normalized;

		/// <param name="vertexAttributeType"></param>
		/// <param name="vertexCount">The vertex count of any attribute in the shader can only have 4 elements. If this value is greater than 4 another attribute will be made until the vertex count is filled. A 4x4 float matrix should have a vertexCount of 16, and the system will do the rest.</param>
		/// <param name="attributeType"></param>
		/// <param name="normalized"></param>
		/// <param name="relativeOffsetBytesOverride">Override of the relative offset. This value should usually not need to be tampered with. The <see cref="FieldOffsetAttribute"/> is usually enough to indicate the relative offset.</param>
		public DataAttribute( VertexAttribType vertexAttributeType, uint vertexCount, VertexArrayAttributeType attributeType = VertexArrayAttributeType.DEFAULT, bool normalized = false, int relativeOffsetBytesOverride = -1 ) {
			VertexAttributeType = vertexAttributeType;
			VertexCount = vertexCount;
			RelativeOffsetBytesOverride = relativeOffsetBytesOverride;
			AttributeType = attributeType;
			Normalized = normalized;
		}
	}
}

