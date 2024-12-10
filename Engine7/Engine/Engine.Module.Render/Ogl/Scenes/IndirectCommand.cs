using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Creates a new command
/// </summary>
/// <param name="count">The element count (indices)</param>
/// <param name="instancecount">The instance count</param>
/// <param name="firstIndex">The first element index.</param>
/// <param name="baseVertex">A constant offset added to the vertex index. This uses vertex stride.</param>
/// <param name="baseInstance">The starting instance number, to get the correct instance data from the data buffer.</param>
[StructLayout( LayoutKind.Explicit )]
public readonly struct IndirectCommand( uint count, uint instanceCount, uint firstIndex, uint baseVertex, uint baseInstance ) {
	/// <summary>
	/// Number of indices in the rendered mesh. Akin to <c>glDrawElements</c> count parameter.
	/// </summary>
	[FieldOffset( 0 )]
	public readonly uint Count = count;
	/// <summary>
	/// Number of instances to draw in this command.
	/// </summary>
	[FieldOffset( 4 )]
	public readonly uint InstanceCount = instanceCount;
	/// <summary>
	/// First element index, the start of the element segment. This is in n-bytes, as the size per element is decided using the draw call. Usually <see cref="DrawElementsType.UnsignedInt"/> is used.
	/// </summary>
	[FieldOffset( 8 )]
	public readonly uint FirstIndex = firstIndex;
	/// <summary>
	/// First Vertex location in buffer. This is in n-bytes, as the size of the instance is decided using the VAO.
	/// </summary>
	[FieldOffset( 12 )]
	public readonly uint BaseVertex = baseVertex;
	/// <summary>
	/// First instance location in buffer. This is in n-bytes, as the size of the instance is decided using the VAO.
	/// </summary>
	[FieldOffset( 16 )]
	public readonly uint BaseInstance = baseInstance;

	public override string ToString() => $"Command: {Count} indices, {InstanceCount} instances, starting at index {FirstIndex}, vertex {BaseVertex}, instance {BaseInstance}";
}
