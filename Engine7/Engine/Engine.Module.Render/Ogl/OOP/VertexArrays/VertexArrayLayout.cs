using Engine.Module.Render.Ogl.OOP.Buffers;

namespace Engine.Module.Render.Ogl.OOP.VertexArrays;

public sealed class VertexArrayLayout {
	public Type BoundTo { get; private set; }
	public OglBufferBase Buffer { get; private set; } = default!;
	public uint OffsetBytes { get; private set; }
	public int StrideBytes { get; private set; }
	public uint InstanceDivisor { get; private set; }
	public IReadOnlyList<VertexArrayLayoutFieldData> Attributes { get; }

	internal VertexArrayLayout( Type bind, OglBufferBase vbo, uint offsetBytes, int strideBytes, uint instanceDivisor, IEnumerable<VertexArrayLayoutFieldData> fields ) {
		BoundTo = bind;
		Buffer = vbo;
		OffsetBytes = offsetBytes;
		StrideBytes = strideBytes;
		InstanceDivisor = instanceDivisor;
		Attributes = fields.ToList().AsReadOnly();
	}
}
