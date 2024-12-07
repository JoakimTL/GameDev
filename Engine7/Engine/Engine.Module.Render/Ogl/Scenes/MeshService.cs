using Engine.Buffers;
using Engine.Module.Render.Ogl.Services;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Scenes;

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
		this._meshes.Add( mesh );
		return mesh;
	}

	public VertexMesh<TVertex> CreateMesh<TVertex>( Span<TVertex> vertices, Span<uint> elements ) where TVertex : unmanaged {
		if (!bufferService.Get( typeof( TVertex ) ).TryAllocate( (uint) vertices.Length * (uint) Marshal.SizeOf<TVertex>(), out BufferSegment? vertexSegment ))
			throw new InvalidOperationException( "Failed to allocate vertex buffer" );
		if (!bufferService.ElementBuffer.TryAllocate( (uint) elements.Length * IMesh.ElementSizeBytes, out BufferSegment? elementSegment ))
			throw new InvalidOperationException( "Failed to allocate element buffer" );
		VertexMesh<TVertex> mesh = new( vertexSegment, elementSegment );
		mesh.VertexBufferSegment.WriteRange( vertices, 0 );
		mesh.ElementBufferSegment.WriteRange( elements, 0 );
		this._meshes.Add( mesh );
		return mesh;
	}

	protected override bool InternalDispose() {
		foreach (IMesh mesh in this._meshes)
			mesh.Dispose();
		return true;
	}
}
