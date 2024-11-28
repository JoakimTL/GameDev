using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Module.Render.Ogl;

/// <summary>
/// Represents one instance in a scene.
/// </summary>
public interface ISceneInstance : IListenableDisposable {
	event Action<ISceneInstance> OnPropertiesChanged;
	OglVertexArrayObjectBase? VertexArrayObject { get; }
	ShaderBundleBase? ShaderBundle { get; }
	IMesh? Mesh { get; }
	ulong BindIndex { get; }
	bool Valid { get; }
	/// <summary>
	/// The layers control when a sceneobject is rendered. Layers are rendered lowest to highest. Beware of utilizing too many layers for no reason, each layer causes a DrawCall per unique sceneobject.
	/// </summary>
	uint RenderLayer { get; }
}
/// <summary>
/// Represents one instance in a scene.
/// </summary>
public interface IMeshedSceneObject : ISceneObject {
	IMesh? Mesh { get; }
}

public interface IMesh : IListenableDisposable {
	public const uint ElementSizeBytes = sizeof( uint );
	Type VertexType { get; }
	/// <summary>
	/// Number of indices in the mesh.
	/// </summary>
	uint ElementCount { get; }
	/// <summary>
	/// Offset for the first element index. This is an element-size offset, not byte offset.
	/// </summary>
	uint ElementOffset { get; }
	/// <summary>
	/// Offset of the first vertex. This is an element-size offset, not byte offset.
	/// </summary>
	uint VertexOffset { get; }
	event Action? Changed;
}

public abstract class SceneInstanceBase : DisposableIdentifiable, IMeshedSceneObject {
	public event Action<ISceneObject>? OnPropertiesChanged;
	public OglVertexArrayObjectBase? VertexArrayObject { get; protected set; }
	public ShaderBundleBase? ShaderBundle { get; protected set; }
	public IMesh? Mesh { get; private set; }
	protected readonly BufferSegment _instanceDataSegment;
	public bool Valid { get; protected set; }
	public uint RenderLayer { get; protected set; }

	protected override bool InternalDispose() {
		_instanceDataSegment.Dispose();
		return true;
	}


}

public sealed class SceneObject : DisposableIdentifiable {

	public OglVertexArrayObjectBase? VertexArrayObject { get; protected set; }
	public ShaderBundleBase? ShaderBundle { get; protected set; }
	public IMesh? Mesh { get; private set; }
	public ulong BindIndex { get; private set; }
	public bool Valid { get; private set; }
	public uint RenderLayer { get; }

	protected override bool InternalDispose() {
		throw new NotImplementedException();
	}
}

public sealed class Scene {
	public string SceneName { get; }
}

public sealed class SceneService {



}
public sealed class MeshService( BufferService bufferService ) : DisposableIdentifiable {

	private readonly List<IMesh> _meshes = [];

	//public VertexMesh CreateEmptyMesh( Type vertexType, uint vertexCount, uint elementCount ) {
	//	bufferService.Get( typeof( TVertex ) ).TryAllocate( vertexCount * TypeManager.SizeOf<TVertex>(), out BufferSegment? vertexSegment );
	//	bufferService.ElementBuffer.TryAllocate( elementCount * IMesh.ElementSizeBytes, out BufferSegment? elementSegment );
	//}

	public VertexMesh<TVertex> CreateEmptyMesh<TVertex>( uint vertexCount, uint elementCount ) where TVertex : unmanaged {
		if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( vertexCount * (uint) Marshal.SizeOf<TVertex>(), out BufferSegment? vertexSegment ))
			throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		if (!bufferService.ElementBuffer.TryAllocate( elementCount * IMesh.ElementSizeBytes, out BufferSegment? elementSegment ))
			throw new InvalidOperationException( "Failed to allocate element buffer" );
		VertexMesh<TVertex> mesh = new( vertexSegment, elementSegment );
		_meshes.Add( mesh );
		return mesh;
	}

	public VertexMesh<TVertex> CreateMesh<TVertex>( Span<TVertex> vertices, Span<uint> elements ) where TVertex : unmanaged {
		if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( (uint)vertices.Length * (uint) Marshal.SizeOf<TVertex>(), out BufferSegment? vertexSegment ))
			throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		if (!bufferService.ElementBuffer.TryAllocate( (uint) elements.Length * IMesh.ElementSizeBytes, out BufferSegment? elementSegment ))
			throw new InvalidOperationException( "Failed to allocate element buffer" );
		VertexMesh<TVertex> mesh = new( vertexSegment, elementSegment );
		mesh.VertexBufferSegment.WriteRange( vertices, 0 );
		mesh.ElementBufferSegment.WriteRange( elements, 0 );
		_meshes.Add( mesh );
		return mesh;
	}

	protected override bool InternalDispose() {
		foreach (IMesh mesh in _meshes)
			mesh.Dispose();
		return true;
	}
}

public class VertexMesh<TVertex> : DisposableIdentifiable, IMesh where TVertex : unmanaged {

	public BufferSegment VertexBufferSegment { get; }
	public BufferSegment ElementBufferSegment { get; }

	public Type VertexType { get; }
	public uint ElementCount { get; }
	public uint ElementOffset { get; }
	public uint VertexOffset { get; }

	public event Action? Changed;

	internal VertexMesh(BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment) {
		this.VertexBufferSegment = vertexBufferSegment;
		this.ElementBufferSegment = elementBufferSegment;
		VertexType = typeof( TVertex );
		ElementCount = (uint) elementBufferSegment.LengthBytes / IMesh.ElementSizeBytes;
		ElementOffset = (uint) elementBufferSegment.OffsetBytes / IMesh.ElementSizeBytes;
		VertexOffset = (uint) vertexBufferSegment.OffsetBytes / (uint) Marshal.SizeOf<TVertex>();

		vertexBufferSegment.OffsetChanged += OnOffsetChanged;
		elementBufferSegment.OffsetChanged += OnOffsetChanged;
	}

	private void OnOffsetChanged( IBufferSegment<ulong> segment ) => Changed?.Invoke();

	protected override bool InternalDispose() {
		VertexBufferSegment.Dispose();
		ElementBufferSegment.Dispose();
		return true;
	}
}
/// <summary>
/// Represents a collection of identical instances in a scene.
/// </summary>
public interface ISceneObjectCollection {
	bool TryGetIndirectCommand( out IndirectCommand command );
}


public interface ISceneRender {
	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim = PrimitiveType.Triangles );
}

public interface IDataBlockCollection {
	void BindShader( OglShaderPipelineBase s );
	void UnbindBuffers();
}

[StructLayout( LayoutKind.Explicit )]
public readonly struct IndirectCommand {
	/// <summary>
	/// Number of indices in the rendered mesh. Akin to <c>glDrawElements</c> count parameter.
	/// </summary>
	[FieldOffset( 0 )]
	public readonly uint Count;
	/// <summary>
	/// Number of instances to draw in this command.
	/// </summary>
	[FieldOffset( 4 )]
	public readonly uint InstanceCount;
	/// <summary>
	/// First element index, the start of the element segment. This is in n-bytes, as the size per element is decided using the draw call. Usually <see cref="DrawElementsType.UnsignedInt"/> is used.
	/// </summary>
	[FieldOffset( 8 )]
	public readonly uint FirstIndex;
	/// <summary>
	/// First Vertex location in buffer. This is in n-bytes, as the size of the instance is decided using the VAO.
	/// </summary>
	[FieldOffset( 12 )]
	public readonly uint BaseVertex;
	/// <summary>
	/// First instance location in buffer. This is in n-bytes, as the size of the instance is decided using the VAO.
	/// </summary>
	[FieldOffset( 16 )]
	public readonly uint BaseInstance;

	/// <summary>
	/// Creates a new command
	/// </summary>
	/// <param name="count">The element count (indices)</param>
	/// <param name="instancecount">The instance count</param>
	/// <param name="firstIndex">The first element index.</param>
	/// <param name="baseVertex">A constant offset added to the vertex index. This uses vertex stride.</param>
	/// <param name="baseInstance">The starting instance number, to get the correct instance data from the data buffer.</param>
	public IndirectCommand( uint count, uint instanceCount, uint firstIndex, uint baseVertex, uint baseInstance ) {
		Count = count;
		InstanceCount = instanceCount;
		FirstIndex = firstIndex;
		BaseVertex = baseVertex;
		BaseInstance = baseInstance;
	}
}
